using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SocketIOClient;
using SocketIOClient.Transport;
using TagShelf.Alfred.ApiWrapper.Enumerations;

namespace TagShelf.Alfred.ApiWrapper.RealTime
{
    /// <summary>
    /// A standalone client for consuming Alfred real-time events over Socket.IO.
    /// <para>
    /// The client authenticates with an API key (passed as the <c>apiKey</c> query parameter, matching the
    /// server contract), forces the WebSocket transport (the default polling/auto-upgrade transport hangs
    /// against the Alfred socket server), and surfaces events through channel- and event-name-based handlers.
    /// </para>
    /// </summary>
    public class AlfredRealTimeClient : IAlfredRealTimeClient
    {
        private const string DefaultRealTimeUrl = "https://sockets.tagshelf.io";

        private readonly SocketIOClient.SocketIO _socket;
        private readonly RealTimeOptions _options;
        private readonly RealTimeDispatcher _dispatcher = new RealTimeDispatcher();
        private readonly HashSet<string> _wiredChannels = new HashSet<string>(StringComparer.Ordinal);
        private readonly object _sync = new object();
        private bool _disposed;

        /// <inheritdoc />
        public string Url { get; }

        /// <inheritdoc />
        public bool IsConnected => _socket != null && _socket.Connected;

        /// <inheritdoc />
        public event EventHandler Connected;

        /// <inheritdoc />
        public event EventHandler<string> Disconnected;

        /// <inheritdoc />
        public event EventHandler<string> Error;

        /// <inheritdoc />
        public event EventHandler<int> ReconnectAttempt;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlfredRealTimeClient"/> class.
        /// Instances are normally created through <see cref="Core.AlfredFactory.CreateRealTimeClient"/>.
        /// </summary>
        /// <param name="apiKey">The API key used to authenticate the socket connection.</param>
        /// <param name="environment">The target environment. Determines the default server URL.</param>
        /// <param name="options">Optional client options. When <c>null</c>, defaults are used.</param>
        internal AlfredRealTimeClient(string apiKey, EnvironmentType environment, RealTimeOptions options)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException("API key must be provided.", nameof(apiKey));
            }

            _options = options ?? new RealTimeOptions();
            Url = ResolveUrl(environment, _options);

            _socket = new SocketIOClient.SocketIO(Url, new SocketIOOptions
            {
                Query = new[] { new KeyValuePair<string, string>("apiKey", apiKey) },
                // WebSocket is REQUIRED: the default polling + auto-upgrade transport hangs silently
                // against the Alfred socket server (no sticky sessions behind the load balancer).
                Transport = TransportProtocol.WebSocket,
                Reconnection = _options.Reconnection,
                ReconnectionAttempts = _options.ReconnectionAttempts,
                ReconnectionDelay = _options.ReconnectionDelay,
                ConnectionTimeout = TimeSpan.FromMilliseconds(_options.ConnectionTimeout),
            });

            _socket.OnConnected += (sender, args) =>
            {
                Log("connected to " + Url);
                Connected?.Invoke(this, EventArgs.Empty);
            };
            _socket.OnDisconnected += (sender, reason) =>
            {
                Log("disconnected: " + reason);
                Disconnected?.Invoke(this, reason);
            };
            _socket.OnError += (sender, error) =>
            {
                Log("error: " + error);
                Error?.Invoke(this, error);
            };
            _socket.OnReconnectAttempt += (sender, attempt) =>
            {
                Log("reconnect attempt #" + attempt);
                ReconnectAttempt?.Invoke(this, attempt);
            };
        }

        /// <summary>
        /// Resolves the real-time server URL: an explicit <see cref="RealTimeOptions.RealTimeUrl"/> override
        /// takes precedence; otherwise the environment default is used.
        /// </summary>
        internal static string ResolveUrl(EnvironmentType environment, RealTimeOptions options)
        {
            if (options != null && !string.IsNullOrWhiteSpace(options.RealTimeUrl))
            {
                return options.RealTimeUrl;
            }

            // Both Production and Staging currently share the same socket endpoint.
            switch (environment)
            {
                case EnvironmentType.Production:
                case EnvironmentType.Staging:
                    return DefaultRealTimeUrl;
                default:
                    throw new ArgumentException("Unsupported environment type", nameof(environment));
            }
        }

        /// <inheritdoc />
        public void OnJobEvent(Action<RealTimeEvent> handler)
        {
            On(RealTimeChannels.JobEvent, handler);
        }

        /// <inheritdoc />
        public void OnFileEvent(Action<RealTimeEvent> handler)
        {
            On(RealTimeChannels.FileEvent, handler);
        }

        /// <inheritdoc />
        public void On(string channel, Action<RealTimeEvent> handler)
        {
            if (string.IsNullOrWhiteSpace(channel))
            {
                throw new ArgumentException("Channel must be provided.", nameof(channel));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            EnsureChannelWired(channel);
            _dispatcher.AddChannelHandler(channel, handler);
        }

        /// <inheritdoc />
        public void On(string channel, string eventName, Action<RealTimeEvent> handler)
        {
            if (string.IsNullOrWhiteSpace(channel))
            {
                throw new ArgumentException("Channel must be provided.", nameof(channel));
            }
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentException("Event name must be provided.", nameof(eventName));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            EnsureChannelWired(channel);
            _dispatcher.AddEventNameHandler(channel, eventName, handler);
        }

        /// <inheritdoc />
        public async Task ConnectAsync()
        {
            if (IsConnected)
            {
                return;
            }

            var connectTask = _socket.ConnectAsync();
            var timeoutTask = Task.Delay(_options.ConnectionTimeout);
            var finished = await Task.WhenAny(connectTask, timeoutTask).ConfigureAwait(false);

            if (finished == timeoutTask && !IsConnected)
            {
                throw new TimeoutException(
                    $"Timed out connecting to {Url} after {_options.ConnectionTimeout} ms.");
            }

            // Observe any exception thrown by the connect task (e.g. when reconnection is disabled).
            await connectTask.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task DisconnectAsync()
        {
            if (_socket != null && _socket.Connected)
            {
                await _socket.DisconnectAsync().ConfigureAwait(false);
            }
        }

        private void EnsureChannelWired(string channel)
        {
            lock (_sync)
            {
                if (_wiredChannels.Add(channel))
                {
                    _socket.On(channel, response => HandleRaw(channel, response));
                }
            }
        }

        private void HandleRaw(string channel, SocketIOResponse response)
        {
            RealTimeEvent realTimeEvent;
            try
            {
                realTimeEvent = RealTimeEvent.FromJson(channel, ExtractJson(response));
            }
            catch (Exception ex)
            {
                Log("failed to parse event on " + channel + ": " + ex.Message);
                Error?.Invoke(this, "Failed to parse event on '" + channel + "': " + ex.Message);
                return;
            }

            foreach (var handler in _dispatcher.ResolveHandlers(channel, realTimeEvent.EventName))
            {
                try
                {
                    handler(realTimeEvent);
                }
                catch (Exception ex)
                {
                    // A single misbehaving handler must not tear down the socket receive loop.
                    Log("handler threw for " + realTimeEvent + ": " + ex.Message);
                    Error?.Invoke(this, "Handler error for '" + channel + "': " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Extracts the raw JSON of the first argument of a socket response. The SocketIOClient serializer is
        /// System.Text.Json, so we take the first argument as a <see cref="JsonElement"/> and hand its raw
        /// text to Newtonsoft (the library's JSON standard) for parsing.
        /// </summary>
        private static string ExtractJson(SocketIOResponse response)
        {
            try
            {
                return response.GetValue<JsonElement>(0).GetRawText();
            }
            catch
            {
                // Fallback: the response's string form is the JSON array of arguments.
                return response.ToString();
            }
        }

        private void Log(string message)
        {
            if (_options.Verbose)
            {
                Console.WriteLine("[AlfredRealTime] " + message);
            }
        }

        /// <summary>Disconnects and releases the underlying socket resources.</summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            _socket?.Dispose();
        }
    }
}

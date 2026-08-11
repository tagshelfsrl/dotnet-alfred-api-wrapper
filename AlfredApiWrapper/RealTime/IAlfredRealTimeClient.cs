using System;
using System.Threading.Tasks;

namespace TagShelf.Alfred.ApiWrapper.RealTime
{
    /// <summary>
    /// A standalone client for consuming Alfred real-time events over Socket.IO.
    /// Create instances through <see cref="Core.AlfredFactory.CreateRealTimeClient"/>, register handlers,
    /// then call <see cref="ConnectAsync"/>.
    /// </summary>
    public interface IAlfredRealTimeClient : IDisposable
    {
        /// <summary>Gets a value indicating whether the client is currently connected.</summary>
        bool IsConnected { get; }

        /// <summary>The resolved real-time server URL the client connects to.</summary>
        string Url { get; }

        /// <summary>Raised when the client successfully connects (including after a reconnect).</summary>
        event EventHandler Connected;

        /// <summary>Raised when the client disconnects. The argument is the disconnect reason.</summary>
        event EventHandler<string> Disconnected;

        /// <summary>Raised when the client encounters an error (connection or handler). The argument is the error message.</summary>
        event EventHandler<string> Error;

        /// <summary>Raised on each reconnection attempt. The argument is the attempt number.</summary>
        event EventHandler<int> ReconnectAttempt;

        /// <summary>Registers a handler for all job events (the <c>job_event</c> channel).</summary>
        void OnJobEvent(Action<RealTimeEvent> handler);

        /// <summary>Registers a handler for all file events (the <c>file_event</c> channel).</summary>
        void OnFileEvent(Action<RealTimeEvent> handler);

        /// <summary>Registers a handler for every event on an arbitrary <paramref name="channel"/>.</summary>
        void On(string channel, Action<RealTimeEvent> handler);

        /// <summary>
        /// Registers a handler that fires only when the inner <c>event_name</c> matches
        /// <paramref name="eventName"/> on the given <paramref name="channel"/>.
        /// </summary>
        void On(string channel, string eventName, Action<RealTimeEvent> handler);

        /// <summary>
        /// Connects to the real-time server. Completes once connected, or throws
        /// <see cref="TimeoutException"/> if the connection is not established within the configured timeout.
        /// </summary>
        Task ConnectAsync();

        /// <summary>Disconnects from the real-time server. Safe to call when not connected.</summary>
        Task DisconnectAsync();
    }
}

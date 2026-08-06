using System;
using System.Collections.Generic;

namespace TagShelf.Alfred.ApiWrapper.RealTime
{
    /// <summary>
    /// Thread-safe routing table that maps incoming <see cref="RealTimeEvent"/>s to the handlers that
    /// registered interest in them — either for a whole channel or for a specific inner <c>event_name</c>.
    /// Kept free of any socket dependency so the routing logic can be unit-tested in isolation.
    /// </summary>
    internal sealed class RealTimeDispatcher
    {
        private readonly object _sync = new object();

        // channel -> handlers interested in every event on the channel
        private readonly Dictionary<string, List<Action<RealTimeEvent>>> _channelHandlers =
            new Dictionary<string, List<Action<RealTimeEvent>>>(StringComparer.Ordinal);

        // channel -> (event_name -> handlers interested only in that event_name)
        private readonly Dictionary<string, Dictionary<string, List<Action<RealTimeEvent>>>> _eventNameHandlers =
            new Dictionary<string, Dictionary<string, List<Action<RealTimeEvent>>>>(StringComparer.Ordinal);

        /// <summary>Registers a handler for every event on <paramref name="channel"/>.</summary>
        public void AddChannelHandler(string channel, Action<RealTimeEvent> handler)
        {
            lock (_sync)
            {
                if (!_channelHandlers.TryGetValue(channel, out var handlers))
                {
                    handlers = new List<Action<RealTimeEvent>>();
                    _channelHandlers[channel] = handlers;
                }
                handlers.Add(handler);
            }
        }

        /// <summary>Registers a handler that fires only for a specific inner <paramref name="eventName"/>.</summary>
        public void AddEventNameHandler(string channel, string eventName, Action<RealTimeEvent> handler)
        {
            lock (_sync)
            {
                if (!_eventNameHandlers.TryGetValue(channel, out var byName))
                {
                    byName = new Dictionary<string, List<Action<RealTimeEvent>>>(StringComparer.Ordinal);
                    _eventNameHandlers[channel] = byName;
                }
                if (!byName.TryGetValue(eventName, out var handlers))
                {
                    handlers = new List<Action<RealTimeEvent>>();
                    byName[eventName] = handlers;
                }
                handlers.Add(handler);
            }
        }

        /// <summary>
        /// Returns a snapshot of the handlers that should receive an event on <paramref name="channel"/>
        /// with the given <paramref name="eventName"/>: all channel-level handlers plus any registered for
        /// that specific event name.
        /// </summary>
        public List<Action<RealTimeEvent>> ResolveHandlers(string channel, string eventName)
        {
            var result = new List<Action<RealTimeEvent>>();
            lock (_sync)
            {
                if (_channelHandlers.TryGetValue(channel, out var channelHandlers))
                {
                    result.AddRange(channelHandlers);
                }
                if (eventName != null &&
                    _eventNameHandlers.TryGetValue(channel, out var byName) &&
                    byName.TryGetValue(eventName, out var named))
                {
                    result.AddRange(named);
                }
            }
            return result;
        }
    }
}

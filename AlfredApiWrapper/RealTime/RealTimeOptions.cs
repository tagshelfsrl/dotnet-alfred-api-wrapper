namespace TagShelf.Alfred.ApiWrapper.RealTime
{
    /// <summary>
    /// Configuration options for the <see cref="AlfredRealTimeClient"/>.
    /// All values have sensible defaults; override only what you need.
    /// </summary>
    public class RealTimeOptions
    {
        /// <summary>
        /// Overrides the real-time server URL. When <c>null</c> or empty, the URL is derived
        /// from the environment (currently <c>https://sockets.tagshelf.io</c> for all environments).
        /// </summary>
        public string RealTimeUrl { get; set; }

        /// <summary>
        /// Whether the client should automatically attempt to reconnect when the connection drops.
        /// Defaults to <c>true</c>.
        /// </summary>
        public bool Reconnection { get; set; } = true;

        /// <summary>
        /// Maximum number of reconnection attempts. Defaults to <see cref="int.MaxValue"/> (retry indefinitely).
        /// </summary>
        public int ReconnectionAttempts { get; set; } = int.MaxValue;

        /// <summary>
        /// Delay between reconnection attempts, in milliseconds. Defaults to 1000 ms.
        /// </summary>
        public int ReconnectionDelay { get; set; } = 1000;

        /// <summary>
        /// Connection handshake timeout, in milliseconds. If the client is not connected within this
        /// window, <see cref="AlfredRealTimeClient.ConnectAsync"/> throws. Defaults to 20000 ms.
        /// </summary>
        public int ConnectionTimeout { get; set; } = 20000;

        /// <summary>
        /// When <c>true</c>, emits diagnostic messages to the console (connection life-cycle, errors).
        /// Defaults to <c>false</c>.
        /// </summary>
        public bool Verbose { get; set; } = false;
    }
}

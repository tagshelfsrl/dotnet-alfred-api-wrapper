namespace TagShelf.Alfred.ApiWrapper.RealTime
{
    /// <summary>
    /// The aggregate Socket.IO channels the Alfred real-time server emits on.
    /// The server forwards every event on one of these two channels; the specific event type is
    /// carried in the payload's inner <c>event_name</c> field (exposed as <see cref="RealTimeEvent.EventName"/>).
    /// </summary>
    public static class RealTimeChannels
    {
        /// <summary>Channel carrying all job-related events.</summary>
        public const string JobEvent = "job_event";

        /// <summary>Channel carrying all file-related events.</summary>
        public const string FileEvent = "file_event";
    }

    /// <summary>
    /// Known values of the inner <c>event_name</c> field carried on the <see cref="RealTimeChannels.JobEvent"/> channel.
    /// These are the values observed emitted by the production server. The list is not guaranteed to be
    /// exhaustive — always read <see cref="RealTimeEvent.EventName"/> directly to handle any value.
    /// </summary>
    public static class JobEventNames
    {
        /// <summary>A new job has been created.</summary>
        public const string JobCreate = "alfred_event_job_create";

        /// <summary>A job has started its workflow.</summary>
        public const string JobStart = "alfred_event_job_start";

        /// <summary>A job transitioned from one workflow stage to another. Payload carries a <c>stage</c> field.</summary>
        public const string JobStageUpdate = "alfred_event_job_stage_update";
    }

    /// <summary>
    /// Known values of the inner <c>event_name</c> field carried on the <see cref="RealTimeChannels.FileEvent"/> channel.
    /// These are the values observed emitted by the production server. The list is not guaranteed to be
    /// exhaustive — always read <see cref="RealTimeEvent.EventName"/> directly to handle any value.
    /// </summary>
    public static class FileEventNames
    {
        /// <summary>A file's processing status changed. Payload carries classification/status fields.</summary>
        public const string FileStatusUpdate = "alfred_event_file_status_update";

        /// <summary>A file finished processing (<c>file_status</c> = <c>finished</c>).</summary>
        public const string FileDone = "alfred_event_file_done";
    }
}

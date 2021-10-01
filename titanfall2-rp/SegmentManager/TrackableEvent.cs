namespace titanfall2_rp.SegmentManager
{
    /// <summary>
    /// Different types of events that I want to track. Based on what event is being fired, I'll gather different info.
    /// </summary>
    public enum TrackableEvent
    {
        /// <summary>
        /// Fired whenever an error occurs that I want to look into more
        /// </summary>
        Error,
        /// <summary>
        /// Fired during normal gameplay
        /// </summary>
        Gameplay,
        /// <summary>
        /// Fired when Titanfall 2 is opened
        /// </summary>
        GameOpened,
        /// <summary>
        /// Fired when Titanfall 2 is closed
        /// </summary>
        GameClosed,
        /// <summary>
        /// Fired when some information fails to be found.
        /// I created this to get to the bottom of the "UNKNOWN GAME MODE" message.
        /// </summary>
        GameplayInfoFailure,
        /// <summary>
        /// This value is for internal use only and is fired during a Track() call if there was some sort of failure when gathering and sending info
        /// </summary>
        FailureWhenFiringEvent,
        /// <summary>
        /// This value is for internal use only and is fired when the first attempt to track a failure, itself fails. 
        /// </summary>
        DoubleFailure,
    }
}
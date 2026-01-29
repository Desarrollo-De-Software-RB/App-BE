namespace TvTracker.Notificationes
{
    public enum NotificationType
    {
        /// <summary>
        /// Triggered when the IMDB rating of a series changes significantly (e.g., > 0.2 difference).
        /// </summary>
        RatingChange,

        /// <summary>
        /// Triggered when the number of votes increases significantly (10% or 1000 votes), indicating popularity.
        /// </summary>
        VotesChange,

        /// <summary>
        /// Triggered when the poster image is updated.
        /// </summary>
        PosterChange,

        /// <summary>
        /// Triggered when the plot synopsis is updated.
        /// </summary>
        PlotChange,

        /// <summary>
        /// Triggered when the runtime of the series changes.
        /// </summary>
        RuntimeChange,

        /// <summary>
        /// Triggered when the series status changes (e.g. Ended, Renewed) or Year updates.
        /// </summary>
        StatusChange,

        /// <summary>
        /// Triggered by user actions on their watchlist (e.g. Added, Status Change).
        /// </summary>
        UserActivity,

        /// <summary>
        /// System-wide notifications (e.g. Password Changed, Profile Updated).
        /// </summary>
        System,

        /// <summary>
        /// Triggered for trending series or personalized highlights (e.g. "Dark is one of your top rated").
        /// </summary>
        Trend,

        /// <summary>
        /// Reminders for inactivity or pending series (e.g. "You haven't watched X in a while").
        /// </summary>
        Reminder,

        /// <summary>
        /// Triggered when the user rates a series.
        /// </summary>
        UserRating
    }

    public enum NotificationChannel
    {
        InApp,
        Email
    }
}

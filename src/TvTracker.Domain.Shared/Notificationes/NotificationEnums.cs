namespace TvTracker.Notificationes
{
    public enum NotificationType
    {
        RatingChange,
        VotesChange,
        PosterChange,
        PlotChange,
        AwardsChange,
        RuntimeChange,
        StatusChange,
        UserActivity,
        System,
        Trend,
        Reminder,
        UserRating
    }

    public enum NotificationChannel
    {
        InApp,
        Email
    }
}

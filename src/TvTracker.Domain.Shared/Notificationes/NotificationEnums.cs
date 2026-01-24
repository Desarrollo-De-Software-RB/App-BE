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
        Reminder
    }

    public enum NotificationChannel
    {
        InApp,
        Email
    }
}

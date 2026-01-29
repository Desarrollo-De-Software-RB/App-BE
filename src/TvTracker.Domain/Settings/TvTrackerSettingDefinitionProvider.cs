using Volo.Abp.Settings;

namespace TvTracker.Settings;

public class TvTrackerSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(TvTrackerSettings.MySetting1));

        var smtpPassword = context.GetOrNull("Abp.Mailing.Smtp.Password");
        if (smtpPassword != null)
        {
            smtpPassword.IsEncrypted = false;
            smtpPassword.DefaultValue = "tvtracker2026";
        }

        var smtpHost = context.GetOrNull("Abp.Mailing.Smtp.Host");
        if (smtpHost != null) smtpHost.DefaultValue = "smtp.gmail.com";

        var smtpPort = context.GetOrNull("Abp.Mailing.Smtp.Port");
        if (smtpPort != null) smtpPort.DefaultValue = "587";

        var smtpUser = context.GetOrNull("Abp.Mailing.Smtp.UserName");
        if (smtpUser != null) smtpUser.DefaultValue = "tvtrackerpalazzisaltoruano@gmail.com";

        var smtpEnableSsl = context.GetOrNull("Abp.Mailing.Smtp.EnableSsl");
        if (smtpEnableSsl != null) smtpEnableSsl.DefaultValue = "true";

        var smtpUseDefaultCredentials = context.GetOrNull("Abp.Mailing.Smtp.UseDefaultCredentials");
        if (smtpUseDefaultCredentials != null) smtpUseDefaultCredentials.DefaultValue = "false";
    }
}

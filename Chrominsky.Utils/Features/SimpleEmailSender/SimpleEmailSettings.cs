namespace Chrominsky.Utils.Features.SimpleEmailSender;

public class SimpleEmailSettings
{
    public string SmtpHost { get; set; }
    public int SmtpPort { get; set; }
    public string SenderEmail { get; set; }
    public string SenderPassword { get; set; }
}
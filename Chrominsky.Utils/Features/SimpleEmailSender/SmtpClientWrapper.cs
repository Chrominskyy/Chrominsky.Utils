using System.Net.Mail;

namespace Chrominsky.Utils.Features.SimpleEmailSender;

public class SmtpClientWrapper : ISmtpClient
{
    private readonly SmtpClient _smtpClient;

    public SmtpClientWrapper(SmtpClient smtpClient)
    {
        _smtpClient = smtpClient;
    }

    public void Send(MailMessage mailMessage)
    {
        _smtpClient.Send(mailMessage);
    }
}

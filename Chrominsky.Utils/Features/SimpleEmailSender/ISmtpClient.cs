using System.Net.Mail;

namespace Chrominsky.Utils.Features.SimpleEmailSender;

public interface ISmtpClient
{
    void Send(MailMessage mailMessage);
}

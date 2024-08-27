using System;
using System.Net;
using System.Net.Mail;
using Chrominsky.Utils.Features.SimpleEmailSender;
using Microsoft.Extensions.Options;

public class SimpleEmailSender
{
    private readonly SimpleEmailSettings _emailSettings;

    public SimpleEmailSender(IOptions<SimpleEmailSettings> emailSettings)
    {
        _emailSettings = emailSettings?.Value ?? throw new ArgumentNullException(nameof(emailSettings), "Email settings are not provided");

        // Validate required settings
        if (string.IsNullOrWhiteSpace(_emailSettings.SmtpHost))
            throw new ArgumentException("SMTP Host is not configured in email settings.");

        if (_emailSettings.SmtpPort <= 0)
            throw new ArgumentException("SMTP Port is not configured or invalid in email settings.");

        if (string.IsNullOrWhiteSpace(_emailSettings.SenderEmail))
            throw new ArgumentException("Sender Email is not configured in email settings.");

        if (string.IsNullOrWhiteSpace(_emailSettings.SenderPassword))
            throw new ArgumentException("Sender Password is not configured in email settings.");

        // Assign the factory method inside the constructor where it has access to instance variables
        CreateSmtpClient = () =>
        {
            var smtpClient = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword)
            };

            return new SmtpClientWrapper(smtpClient); // Return SmtpClientWrapper that implements ISmtpClient
        };
    }

    // Factory function to create ISmtpClient, allowing it to be overridden in unit tests
    public Func<ISmtpClient> CreateSmtpClient { get; set; }

    public void SendEmail(string toEmail, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
            throw new ArgumentException("Recipient email address cannot be empty.");

        try
        {
            var smtpClient = CreateSmtpClient();

            var mailMessage = new MailMessage(_emailSettings.SenderEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            smtpClient.Send(mailMessage);
            Console.WriteLine("Email sent successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            throw; // Re-throw to allow for unit testing exceptions
        }
    }
}
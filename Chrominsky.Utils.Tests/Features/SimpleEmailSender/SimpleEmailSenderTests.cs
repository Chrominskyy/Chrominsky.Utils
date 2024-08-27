using System.Net.Mail;
using Chrominsky.Utils.Features.SimpleEmailSender;
using Microsoft.Extensions.Options;

namespace Chrominsky.Utils.Tests.Features.SimpleEmailSender;

public class SimpleEmailSenderTests
{
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_IfEmailSettingsNotProvided()
    {
        // Arrange
        IOptions<SimpleEmailSettings> nullOptions = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new global::SimpleEmailSender(nullOptions));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_IfSmtpHostIsMissing()
    {
        // Arrange
        var emailSettings = new SimpleEmailSettings
        {
            SmtpHost = null,
            SmtpPort = 587,
            SenderEmail = "your-email@domain.com",
            SenderPassword = "your-password",
        };

        var optionsMock = new Mock<IOptions<SimpleEmailSettings>>();
        optionsMock.Setup(o => o.Value).Returns(emailSettings);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new global::SimpleEmailSender(optionsMock.Object));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_IfSmtpPortIsInvalid()
    {
        // Arrange
        var emailSettings = new SimpleEmailSettings
        {
            SmtpHost = "smtp.your-email-provider.com",
            SmtpPort = 0,
            SenderEmail = "your-email@domain.com",
            SenderPassword = "your-password",
        };

        var optionsMock = new Mock<IOptions<SimpleEmailSettings>>();
        optionsMock.Setup(o => o.Value).Returns(emailSettings);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new global::SimpleEmailSender(optionsMock.Object));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_IfSenderEmailIsMissing()
    {
        // Arrange
        var emailSettings = new SimpleEmailSettings
        {
            SmtpHost = "smtp.your-email-provider.com",
            SmtpPort = 587,
            SenderEmail = null,
            SenderPassword = "your-password",
        };

        var optionsMock = new Mock<IOptions<SimpleEmailSettings>>();
        optionsMock.Setup(o => o.Value).Returns(emailSettings);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new global::SimpleEmailSender(optionsMock.Object));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_IfSenderPasswordIsMissing()
    {
        // Arrange
        var emailSettings = new SimpleEmailSettings
        {
            SmtpHost = "smtp.your-email-provider.com",
            SmtpPort = 587,
            SenderEmail = "your-email@domain.com",
            SenderPassword = null,
        };

        var optionsMock = new Mock<IOptions<SimpleEmailSettings>>();
        optionsMock.Setup(o => o.Value).Returns(emailSettings);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new global::SimpleEmailSender(optionsMock.Object));
    }

    [Fact]
    public void SendEmail_ShouldThrowArgumentException_IfToEmailIsMissing()
    {
        // Arrange
        var emailSettings = new SimpleEmailSettings
        {
            SmtpHost = "smtp.your-email-provider.com",
            SmtpPort = 587,
            SenderEmail = "your-email@domain.com",
            SenderPassword = "your-password",
        };

        var optionsMock = new Mock<IOptions<SimpleEmailSettings>>();
        optionsMock.Setup(o => o.Value).Returns(emailSettings);

        var emailSender = new global::SimpleEmailSender(optionsMock.Object);

        string toEmail = null;
        string subject = "Account Confirmation";
        string body = "<h1>Confirm your account</h1><p>Click the link to confirm your account.</p>";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => emailSender.SendEmail(toEmail, subject, body));
    }

    [Fact]
    public void SendEmail_ShouldSendEmailSuccessfully()
    {
        // Arrange
        var emailSettings = new SimpleEmailSettings
        {
            SmtpHost = "smtp.your-email-provider.com",
            SmtpPort = 587,
            SenderEmail = "your-email@domain.com",
            SenderPassword = "your-password",
        };

        var optionsMock = new Mock<IOptions<SimpleEmailSettings>>();
        optionsMock.Setup(o => o.Value).Returns(emailSettings);

        // Mock ISmtpClient instead of SmtpClient
        var smtpClientMock = new Mock<ISmtpClient>();

        // Setup Send method to be verifiable
        smtpClientMock.Setup(s => s.Send(It.IsAny<MailMessage>())).Verifiable();

        // Create SimpleEmailSender with mocked ISmtpClient
        var emailSender = new global::SimpleEmailSender(optionsMock.Object)
        {
            CreateSmtpClient = () =>
                smtpClientMock.Object // Use the mock ISmtpClient in the factory
            ,
        };

        string toEmail = "recipient@domain.com";
        string subject = "Account Confirmation";
        string body = "<h1>Confirm your account</h1><p>Click the link to confirm your account.</p>";

        // Act
        emailSender.SendEmail(toEmail, subject, body);

        // Assert
        smtpClientMock.Verify(s => s.Send(It.IsAny<MailMessage>()), Times.Once);
    }

    [Fact]
    public void SendEmail_ShouldUseCorrectEmailSettings()
    {
        // Arrange
        var emailSettings = new SimpleEmailSettings
        {
            SmtpHost = "smtp.your-email-provider.com",
            SmtpPort = 587,
            SenderEmail = "your-email@domain.com",
            SenderPassword = "your-password",
        };

        var optionsMock = new Mock<IOptions<SimpleEmailSettings>>();
        optionsMock.Setup(o => o.Value).Returns(emailSettings);

        // Mock ISmtpClient
        var smtpClientMock = new Mock<ISmtpClient>();

        // Capture the MailMessage sent
        MailMessage capturedMessage = null;
        smtpClientMock
            .Setup(s => s.Send(It.IsAny<MailMessage>()))
            .Callback<MailMessage>(msg => capturedMessage = msg)
            .Verifiable();

        // Create SimpleEmailSender with mocked ISmtpClient
        var emailSender = new global::SimpleEmailSender(optionsMock.Object)
        {
            CreateSmtpClient = () => smtpClientMock.Object,
        };

        string toEmail = "recipient@domain.com";
        string subject = "Test Email Subject";
        string body = "<h1>This is a test email</h1>";

        // Act
        emailSender.SendEmail(toEmail, subject, body);

        // Assert
        smtpClientMock.Verify(s => s.Send(It.IsAny<MailMessage>()), Times.Once);
        Assert.NotNull(capturedMessage);
        Assert.Equal("your-email@domain.com", capturedMessage.From.Address);
        Assert.Equal("recipient@domain.com", capturedMessage.To[0].Address);
        Assert.Equal("Test Email Subject", capturedMessage.Subject);
        Assert.Equal(body, capturedMessage.Body);
        Assert.True(capturedMessage.IsBodyHtml);
    }
}
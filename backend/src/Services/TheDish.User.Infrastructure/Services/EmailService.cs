using System.Net;
using System.Net.Http.Json;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TheDish.User.Application.Interfaces;

namespace TheDish.User.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendPasswordResetCodeAsync(string email, string code)
    {
        var provider = _configuration["Email:Provider"] ?? "Console";
        
        try
        {
            switch (provider.ToLower())
            {
                case "smtp":
                    await SendViaSmtpAsync(email, code);
                    break;
                case "sendgrid":
                    await SendViaSendGridAsync(email, code);
                    break;
                case "ses":
                case "awsses":
                    await SendViaAwsSesAsync(email, code);
                    break;
                case "console":
                default:
                    // Fallback to console logging
                    _logger.LogInformation("Password reset code for {Email}: {Code}", email, code);
                    await Task.CompletedTask;
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}. Code: {Code}", email, code);
            // Fallback to console logging on error
            _logger.LogInformation("Password reset code for {Email}: {Code} (Email sending failed, logged to console)", email, code);
        }
    }

    private async Task SendViaSmtpAsync(string email, string code)
    {
        var smtpHost = _configuration["Email:Smtp:Host"];
        var smtpPort = _configuration.GetValue<int>("Email:Smtp:Port", 587);
        var smtpUsername = _configuration["Email:Smtp:Username"];
        var smtpPassword = _configuration["Email:Smtp:Password"];
        var smtpFromEmail = _configuration["Email:Smtp:FromEmail"] ?? smtpUsername;
        var smtpFromName = _configuration["Email:Smtp:FromName"] ?? "The Dish";
        var enableSsl = _configuration.GetValue<bool>("Email:Smtp:EnableSsl", true);

        if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
        {
            throw new InvalidOperationException("SMTP configuration is incomplete. Please check Email:Smtp settings in appsettings.json");
        }

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUsername, smtpPassword),
            EnableSsl = enableSsl
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(smtpFromEmail ?? smtpUsername ?? throw new InvalidOperationException("FromEmail cannot be null"), smtpFromName),
            Subject = "Reset Your Password - The Dish",
            Body = GetEmailBody(code),
            IsBodyHtml = true
        };

        mailMessage.To.Add(email);

        await client.SendMailAsync(mailMessage);
        _logger.LogInformation("Password reset email sent via SMTP to {Email}", email);
    }

    private async Task SendViaSendGridAsync(string email, string code)
    {
        var apiKey = _configuration["Email:SendGrid:ApiKey"];
        var fromEmail = _configuration["Email:SendGrid:FromEmail"];
        var fromName = _configuration["Email:SendGrid:FromName"] ?? "The Dish";

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(fromEmail))
        {
            throw new InvalidOperationException("SendGrid configuration is incomplete. Please check Email:SendGrid settings in appsettings.json");
        }

        // Note: Install SendGrid NuGet package: dotnet add package SendGrid
        // For now, we'll use HTTP client to call SendGrid API
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var requestBody = new
        {
            personalizations = new[]
            {
                new
                {
                    to = new[] { new { email = email } },
                    subject = "Reset Your Password - The Dish"
                }
            },
            from = new { email = fromEmail, name = fromName },
            content = new[]
            {
                new
                {
                    type = "text/html",
                    value = GetEmailBody(code)
                }
            }
        };

        var response = await httpClient.PostAsJsonAsync("https://api.sendgrid.com/v3/mail/send", requestBody);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"SendGrid API error: {response.StatusCode} - {errorContent}");
        }

        _logger.LogInformation("Password reset email sent via SendGrid to {Email}", email);
    }

    private async Task SendViaAwsSesAsync(string email, string code)
    {
        // Note: Install AWS SDK NuGet package: dotnet add package AWSSDK.SimpleEmail
        // For now, we'll provide the structure
        var region = _configuration["Email:AwsSes:Region"] ?? "us-east-1";
        var fromEmail = _configuration["Email:AwsSes:FromEmail"];
        var fromName = _configuration["Email:AwsSes:FromName"] ?? "The Dish";

        if (string.IsNullOrEmpty(fromEmail))
        {
            throw new InvalidOperationException("AWS SES configuration is incomplete. Please check Email:AwsSes settings in appsettings.json");
        }

        // AWS SES implementation would go here
        // Requires: AWSSDK.SimpleEmail package
        // Example:
        // using Amazon.SimpleEmail;
        // using Amazon.SimpleEmail.Model;
        // var client = new AmazonSimpleEmailServiceClient(region);
        // var request = new SendEmailRequest { ... };
        // await client.SendEmailAsync(request);

        _logger.LogWarning("AWS SES email sending not fully implemented. Please install AWSSDK.SimpleEmail package and complete implementation.");
        _logger.LogInformation("Password reset code for {Email}: {Code} (AWS SES not configured)", email, code);
        
        await Task.CompletedTask;
    }

    private string GetEmailBody(string code)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .code {{ background: #667eea; color: white; font-size: 32px; font-weight: bold; padding: 20px; text-align: center; letter-spacing: 8px; border-radius: 8px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>The Dish</h1>
        </div>
        <div class=""content"">
            <h2>Reset Your Password</h2>
            <p>You requested to reset your password. Use the code below to complete the process:</p>
            <div class=""code"">{code}</div>
            <p><strong>This code will expire in 15 minutes.</strong></p>
            <p>If you didn't request this password reset, please ignore this email.</p>
        </div>
        <div class=""footer"">
            <p>&copy; {DateTime.UtcNow.Year} The Dish. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
    }
}


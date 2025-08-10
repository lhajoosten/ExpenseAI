using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using ExpenseAI.Application.Interfaces;

namespace ExpenseAI.Infrastructure.External;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = CreateSmtpClient();
            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.To.Add(to);

            await client.SendMailAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to send email to {to}: {ex.Message}", ex);
        }
    }

    public async Task SendInvoiceEmailAsync(
        string clientEmail,
        string clientName,
        string invoiceNumber,
        string pdfUrl,
        CancellationToken cancellationToken = default)
    {
        var subject = $"Invoice {invoiceNumber} - ExpenseAI";
        var body = CreateInvoiceEmailBody(clientName, invoiceNumber, pdfUrl);

        await SendEmailAsync(clientEmail, subject, body, isHtml: true, cancellationToken);
    }

    private SmtpClient CreateSmtpClient()
    {
        return new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
        {
            EnableSsl = _settings.EnableSsl,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password)
        };
    }

    private static string CreateInvoiceEmailBody(string clientName, string invoiceNumber, string pdfUrl)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; margin-bottom: 30px; }}
        .logo {{ font-size: 24px; font-weight: bold; color: #3b82f6; }}
        .content {{ margin-bottom: 30px; }}
        .button {{ display: inline-block; background-color: #3b82f6; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; font-weight: bold; }}
        .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #e5e7eb; font-size: 12px; color: #6b7280; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <div class=""logo"">ExpenseAI</div>
        </div>

        <div class=""content"">
            <h2>Invoice {invoiceNumber}</h2>
            <p>Dear {clientName},</p>
            <p>Please find attached your invoice {invoiceNumber}. We appreciate your business and look forward to continuing our partnership.</p>
            <p>You can view and download your invoice using the link below:</p>
            <p style=""text-align: center; margin: 30px 0;"">
                <a href=""{pdfUrl}"" class=""button"">View Invoice</a>
            </p>
            <p>If you have any questions about this invoice, please don't hesitate to contact us.</p>
            <p>Thank you for your business!</p>
        </div>

        <div class=""footer"">
            <p>This is an automated message from ExpenseAI. Please do not reply directly to this email.</p>
        </div>
    </div>
</body>
</html>";
    }
}

public class EmailSettings
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = "ExpenseAI";
}

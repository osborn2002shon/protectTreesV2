using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;

public static class EmailService
{
    public static void SendMail(string to, string subject, string body)
    {
        var host = ConfigurationManager.AppSettings["smtpHost"];
        var portStr = ConfigurationManager.AppSettings["smtpPort"];
        var user = ConfigurationManager.AppSettings["smtpUser"];
        var password = ConfigurationManager.AppSettings["smtpPassword"];
        var enableSsl = ConfigurationManager.AppSettings["EnableSSL"];

        int port = 25;
        int.TryParse(portStr, out port);
        bool ssl = string.Equals(enableSsl, "true", StringComparison.OrdinalIgnoreCase);

        var mail = new MailMessage();
        mail.From = new MailAddress(user);
        mail.To.Add(to);
        mail.Subject = subject;
        mail.Body = body;

        var smtp = new SmtpClient(host, port)
        {
            EnableSsl = ssl,
            Credentials = new NetworkCredential(user, password)
        };
        smtp.Send(mail);
    }
}

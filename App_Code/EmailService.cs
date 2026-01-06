using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;

public static class EmailService
{
    public static void  SendMail(string to, string subject, string body)
    {
        var host = ConfigurationManager.AppSettings["MailSMTP"];
        var portStr = ConfigurationManager.AppSettings["MailPort"];
        var user = ConfigurationManager.AppSettings["MailAccount"];
        var password = ConfigurationManager.AppSettings["MailPassword"];
        var enableSsl = ConfigurationManager.AppSettings["MailEnableSsl"];

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
            UseDefaultCredentials = false,   // 非常重要
            Credentials = new NetworkCredential(user, password),
            DeliveryMethod = SmtpDeliveryMethod.Network

        };
        smtp.Send(mail);
    }
}

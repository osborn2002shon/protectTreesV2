using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;


namespace protectTreesV2 {

    public static class Mail
    {
        public static void SendMail(List<MailAddress> toList, string subject, string body)
        {
            var host = ConfigurationManager.AppSettings["MailSMTP"];
            var portStr = ConfigurationManager.AppSettings["MailPort"];
            var user = ConfigurationManager.AppSettings["MailAccount"];
            var password = ConfigurationManager.AppSettings["MailPassword"];
            var enableSsl = ConfigurationManager.AppSettings["MailEnableSsl"];

            int port = 25;
            int.TryParse(portStr, out port);
            bool ssl = string.Equals(enableSsl, "true", StringComparison.OrdinalIgnoreCase);

            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(user);
                mail.Bcc.Add(new MailAddress(user));
                foreach (var addr in toList)
                {
                    mail.To.Add(addr); //每個都可含姓名
                }

                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (var smtp = new SmtpClient(host, port))
                {
                    smtp.EnableSsl = ssl;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(user, password);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(mail);
                }
            }
        }

    }
}



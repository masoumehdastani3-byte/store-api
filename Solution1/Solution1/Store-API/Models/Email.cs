using System.Net.Mail;
namespace Api.Models
{
    public class Email
    {
        public void SendMail(string Body, string Host, string User, string pass, string ToMail, string Subject)
        {

            MailMessage objeto_mail = new MailMessage();
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.Host = Host;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            client.Credentials = new System.Net.NetworkCredential(User, pass);
            objeto_mail.From = new MailAddress(User);
            objeto_mail.To.Add(new MailAddress(ToMail));
            objeto_mail.IsBodyHtml = true;
            objeto_mail.Subject = Subject;
            objeto_mail.Body = Body;
            client.Send(objeto_mail);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Spaetzel.UtilityLibrary.Email
{
    public abstract class BaseEmail
    {
        private MailMessage _message;
        private string _sentFrom;
 

        public BaseEmail( string sentFrom )
        {
            _message = new MailMessage();
            _sentFrom = sentFrom;

            if (!Config.BccEmail.IsNullOrEmpty())
            {
                Message.Bcc.Add(Config.BccEmail);
            }
        }

        public string SentFrom
        {
            get
            {
                return _sentFrom;
            }
        }

        protected MailMessage Message
        {
            get
            {
                return _message;
            }
        }

        public MailAddressCollection To
        {
            get { return Message.To; }
        }

        public MailAddress From
        {
            get { return Message.From; }
            set { Message.From = value; }
        }
 
        public MailAddressCollection Bcc
        {
            get { return Message.Bcc; }
        }


        public MailAddressCollection Cc
        {
            get { return Message.CC; }
        }

        public string Subject
        {
            get { return Message.Subject; }
            set { Message.Subject = value; }
        }



        public MailAddress ReplyTo
        {
            get { return Message.ReplyTo; }
            set { Message.ReplyTo = value; }
        }

        public abstract string GetBody();

        public bool IsBodyHtml
        {
            get
            {
                return Message.IsBodyHtml;
            }
            set
            {
                Message.IsBodyHtml = value;
            }
        }

        public bool Send()
        {
            return Send(true);
        }

        public bool Send(bool logEmail )
        {
         

            Message.Body = GetBody();

            System.Net.Mail.SmtpClient client = new SmtpClient(Config.SmtpServer);

            if (Config.SmtpUsername != String.Empty)
            {
                client.UseDefaultCredentials = false;

                System.Net.NetworkCredential cred = new System.Net.NetworkCredential(Config.SmtpUsername, Config.StmpPassword);

                client.Credentials = cred;
            }

            //if (Config.SmtpPort != 25)
            //{
            //    client.EnableSsl = true;
            //}
            client.Port = Config.SmtpPort;

            try
            {

                client.Send(Message);

                if (logEmail)
                {
                    Emails.LogSentEmail(this);
                }

                return true;
            }
            catch (Exception ex)
            {
               // throw ex;
                try
                {
                    Emails.LogFailedEmail(this, ex);
                }
                catch { }

                throw new Exception("Failure sending e-mail", ex);
            }
        }
        
    }
}

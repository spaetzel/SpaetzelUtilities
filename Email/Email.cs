using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spaetzel.UtilityLibrary.Email
{
    public class Email : BaseEmail
    {
        private string _body;

    

        public Email( string sentFrom ) : base( sentFrom )
        {

        }

        public override string GetBody()
        {
            return Body;
        }

        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        public static void SendWarning(string subject, string codeFile, string method, string message)
        {
            SendWarning(subject, codeFile, method, new Exception(), message);
        }
        public static void SendWarning(string subject, string codeFile, string method, Exception ex, string givenBody)
        {
            Email Warning = new Email(Config.WarningEmail)
            {
                Subject = subject,
                From = new System.Net.Mail.MailAddress(Config.WarningEmail, "CastRoller Warnings"),
                IsBodyHtml = true
            };

            Warning.To.Add(Config.WarningEmail);

            string url = "";
            if (System.Web.HttpContext.Current != null)
            {
                url = System.Web.HttpContext.Current.Request.Url.ToString();
            }

            string body = String.Format("<ul><li>Subject:{0}</li><li>Code File: {1}</li><li>Method: {2}</li><li>Url: {3}</ul>", subject, codeFile, method, url);

            if (!givenBody.IsNullOrEmpty())
            {
                body += "<p>" + givenBody + "<p>";
            }

            var currentException = ex;

            body += "<p>========Exception(s)=======</p>";

            while (currentException != null)
            {
                body += currentException.ToString().Replace("\n", "<br/>");
                body += "<p>========Inner=======</p>";

                currentException = currentException.InnerException;
            }

            Warning.Body = body;

            try
            {
                Warning.Send(false);
            }
            catch (Exception ex2)
            {
                Console.WriteLine(String.Format("Error sending Warning e-mail: {0}", ex2.Message));
            }

        }

        public static void SendAlert(string subject, string codeFile, string method, Exception ex)
        {
            SendAlert(subject, codeFile, method, ex, "");
        }
        public static void SendAlert( string subject, string codeFile, string method, Exception ex, string givenBody )
        {
            Email alert = new Email(Config.AlertEmail)
            {
                Subject = subject,
                From = new System.Net.Mail.MailAddress(Config.AlertEmail, "CastRoller Alerts" ),
                IsBodyHtml = true
            };

            alert.To.Add(Config.AlertEmail);

            string url = "";
            if (System.Web.HttpContext.Current != null)
            {
                url = System.Web.HttpContext.Current.Request.Url.ToString();
            }

            string body = String.Format("<ul><li>Subject:{0}</li><li>Code File: {1}</li><li>Method: {2}</li><li>Url: {3}</ul>", subject, codeFile, method, url);

            if (!givenBody.IsNullOrEmpty())
            {
                body += "<p>" + givenBody + "<p>";
            }

            var currentException = ex;

            body += "<p>========Exception(s)=======</p>";

            while (currentException != null)
            {
                body += currentException.ToString().Replace("\n", "<br/>");
                body += "<p>========Inner=======</p>";

                currentException = currentException.InnerException;
            }

            alert.Body = body;

            try
            {
                alert.Send(false);
            }
            catch( Exception ex2)
            {
                Console.WriteLine(String.Format("Error sending alert e-mail: {0}", ex2.Message));
            }

        }
    }
}

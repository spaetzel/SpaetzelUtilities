using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spaetzel.UtilityLibrary.Email
{
    public static class Config
    {
        private static string _connectionString;
        private static string _smtpServer;
        private static string _smtpUsername = String.Empty;
        private static string _smtpPassword = String.Empty;
        private static int _smtpPort = 25;
        private static string _alertEmail = "alert@redune.com";
        private static string _bccEmail = "";
        private static string _warningEmail;

        public static string WarningEmail
        {
            get
            {
                if (_warningEmail.IsNullOrEmpty())
                {
                    _warningEmail = AlertEmail;
                }
                return _warningEmail;
            }
            set
            {
                _warningEmail = value;
            }
        }

        public static string BccEmail
        {
            get { return Config._bccEmail; }
            set { Config._bccEmail = value; }
        }

        public static string AlertEmail
        {
            get { return Config._alertEmail; }
            set { Config._alertEmail = value; }
        }

        public static string SmtpUsername
        {
            get
            {
                return _smtpUsername;
            }
        }

        public static string StmpPassword
        {
            get
            {
                return _smtpPassword;
            }
        }

        public static int SmtpPort
        {
            get
            {
                return _smtpPort;
            }
        }


        public static string SmtpServer
        {
            get { return Config._smtpServer; }
        }

        public static string ConnectionString
        {
            get { return Config._connectionString; }
        }

     

        public static void SetConfigurations( string alertEmail, string connectionString, string smtpServer, string username, string password, int port)
        {
            AlertEmail = alertEmail;
            _connectionString = connectionString;
            _smtpServer = smtpServer;
            _smtpUsername = username;
            _smtpPassword = password;
            _smtpPort = port;
        }

    }
}

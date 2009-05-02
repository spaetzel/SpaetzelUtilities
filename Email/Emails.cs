using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spaetzel.UtilityLibrary.Email
{
    public static class Emails
    {

        private static bool LogEmail( BaseEmail email, bool success, Exception ex )
        {
            try
            {
                using (EmailDBWriteInterface db = new EmailDBWriteInterface())
                {
                    db.LogEmail(email, success, ex);
                }

                return true;
            }
            catch
            {
                return false;
            }

        }

        internal static void LogSentEmail(BaseEmail email)
        {
            LogEmail(email, true, null);
        }

        internal static void LogFailedEmail(BaseEmail email, Exception ex)
        {
            LogEmail(email, false, ex);
        }
    }
}

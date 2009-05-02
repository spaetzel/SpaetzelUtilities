using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spaetzel.UtilityLibrary;
using Spaetzel.UtilityLibrary.Database;
using MySql.Data.MySqlClient;
using System.Collections;

namespace Spaetzel.UtilityLibrary.Email
{
    class EmailDBWriteInterface : EmailDBInterface
    {
       
        internal void LogEmail(BaseEmail email, bool success, Exception ex)
        {
            //string query = "INSERT INTO emailLog (sentFrom, subject,  from, body, success, exception ) VALUES ( ?sentFrom, ?subject, ?from, ?body, ?success, ?exception )";
            string query = "INSERT INTO emaillog( sentFrom, subject, body, exception, sender, receiver, success ) VALUES( ?sentFrom, ?subject, ?body, ?exception, ?sender, ?receiver, ?success )";

            MySqlCommand command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue( "?sentFrom", email.SentFrom);
          command.Parameters.AddWithValue( "?subject", email.Subject);
          command.Parameters.AddWithValue("?receiver", email.To[0].Address);
           command.Parameters.AddWithValue("?sender", email.From.Address);
            command.Parameters.AddWithValue("?body", email.GetBody());
            command.Parameters.AddWithValue("?success", success);
            if (ex != null)
            {
                command.Parameters.AddWithValue("?exception", ex.Message);
            }
            else
            {
                command.Parameters.AddWithValue("?exception", "");
            }
          
          
            command.ExecuteNonQuery();

        }
    }
}

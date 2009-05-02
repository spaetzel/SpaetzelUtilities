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
    internal class EmailDBInterface : DBInterface
    {

        protected override string connectionString
        {
            get { return Config.ConnectionString; }
        }



       

        
    }
}

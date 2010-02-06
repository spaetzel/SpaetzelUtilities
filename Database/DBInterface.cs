using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace Spaetzel.UtilityLibrary
{
    public abstract class DBInterface : IDisposable
    {
        MySqlConnection _connection = null;

        public DBInterface()
        {
        }

        #region Accessor Methods
        protected abstract string connectionString
        {
            get;
        }

        protected MySqlConnection connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new MySqlConnection(connectionString);
                }

                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }

                return _connection;
            }
        }
        #endregion

        #region Reader column accessors
        public static int GetReaderInt(MySqlDataReader reader, string name)
        {
            int? result = GetReaderIntNull(reader, name);

            if (result != null)
            {
                return (int)result;
            }
            else
            {
                return 0;
            }
        }

        public static int? GetReaderIntNull(MySqlDataReader reader, string name)
        {
            int? result;

            try
            {
                result = Convert.ToInt32(reader[name].ToString());
            }
            catch
            {
                return null;
            }

            return result;
        }

        public static string GetReaderString(MySqlDataReader reader, string name)
        {
            string result;

            try
            {
                result = (string)reader[name];
            }
            catch
            {
                result = "";
            }

            return result;
        }

  

        public static bool GetReaderBool(MySqlDataReader reader, string name)
        {
            bool result;

            try
            {
                result = (bool)reader[name];
            }
            catch
            {
                try
                {
                    int intValue = GetReaderInt(reader, name);

                    result = intValue == 1;
                }
                catch
                {
                    result = false;
                }
            }

            return result;

        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_connection != null)
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();

               _connection.Dispose();
            }
        }

        #endregion



        public static double GetReaderDouble(MySqlDataReader reader, string name)
        {

            double? result = GetReaderDoubleNull(reader, null);



            if (result != null)
            {
                return (double)result;
            }
            else
            {
                return 0;
            }
        
        }


        public static double? GetReaderDoubleNull(MySqlDataReader reader, string name)
        {

            double? result;

            try
            {
                result = Convert.ToDouble(reader[name].ToString());
            }
            catch
            {
                return null;
            }

            return result;

        }


        public static Decimal GetReaderDecimal(MySqlDataReader reader, string name)
        {

            Decimal? result = GetReaderDecimalNull(reader, null);



            if (result != null)
            {
                return (Decimal)result;
            }
            else
            {
                return 0;
            }

        }


        public static Decimal? GetReaderDecimalNull(MySqlDataReader reader, string name)
        {

            Decimal? result;

            try
            {
                result = Convert.ToDecimal(reader[name].ToString());
            }
            catch
            {
                return null;
            }

            return result;

        }

        public static DateTime? GetReaderDateTimeNull(MySqlDataReader reader, string name)
        {
            DateTime? result;

            try
            {
                result = (DateTime)reader[name];
            }
            catch
            {
                result = null;
            }

            return result;
        }

        public static DateTime GetReaderDateTime(MySqlDataReader reader, string name)
        {
            DateTime? result = GetReaderDateTimeNull(reader, name);

            if (result != null)
            {
                return (DateTime)result;
            }
            else
            {
                return DateTime.MinValue;
            }
        }

       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace Spaetzel.UtilityLibrary.Database
{
    public enum Order { Descending, Ascending };
    public abstract class DBQuery
    {
        private enum CommandType { Select, Count, Delete };

        private uint? _count = null;
        private uint _pageNum = 0;
        private Order _order = Order.Descending;
        private uint _startIndex = 0;

        public uint? Count { get { return _count; } set { _count = value; } }

        public uint PageNum { get { return _pageNum; } set {
            if (Count != null && Count > 0)
            {
                _pageNum = value;
                StartIndex = (uint)Count * (value - 1);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Count must have a postive value if setting pageNum");
            }
        } }


        public virtual string ExtraSelect
        {
            get
            {
                return null;
            }
        }

     /*   public virtual string Groups
        {
            get
            {
                return null;
            }
        }
        */

        public Order Order
        {
            get { return _order; }
            set { _order = value; }
        }

        private List<string> _arguments;
        private List<string> _joins;
        private List<string> _groups;

        protected List<string> Arguments
        {
            get { return _arguments; }
            set { _arguments = value; }
        }



        protected List<string> Joins
        {
            get { return _joins; }
            set { _joins = value; }
        }


        protected List<string> Groups
        {
            get
            {
                return _groups;
            }
            set
            {
                _groups = value;
            }
        }

        private string ArgumentString
        {
            get
            {
                string output = "";
                for (int i = 0; i < Arguments.Count; i++)
                {
                    output += Arguments[i];
                    if (i < Arguments.Count - 1)
                    {
                        output += " AND ";
                    }
                }

                return output;
            }
        }

        private string HavingsString
        {
            get
            {
                string output = "";
                for (int i = 0; i < Havings.Count; i++)
                {
                    output += Havings[i];
                    if (i < Havings.Count - 1)
                    {
                        // output += ", ";
                        output += ", ";
                    }
                }

                return output;
            }
        }

        private string GroupsString
        {
            get
            {
                string output = "";
                for (int i = 0; i < Groups.Count; i++)
                {
                    output += Groups[i];
                    if (i < Groups.Count - 1)
                    {
                        // output += ", ";
                        output += ", ";
                    }
                }

                return output;
            }
        }

        private string JoinString
        {
            get
            {
                string output = "";
                for (int i = 0; i < Joins.Count; i++)
                {
                    output += Joins[i];
                    if (i < Joins.Count - 1)
                    {
                       // output += ", ";
                        output += " ";
                    }
                }

                return output;
            }
        }

        protected abstract string MainTable
        {
            get;
        }

        protected abstract string OrderByString
        {
            get;
        }

        public uint StartIndex
        {
            get { return _startIndex; }
            set { _startIndex = value; }
        }

        public List<string> Havings { get; set; }
        protected virtual void FillArgumentsJoins(MySqlCommand command)
        {
            Joins = new List<string>();
            Groups = new List<string>();
            Arguments = new List<string>();
            Havings = new List<string>();

        }

        private MySqlCommand GetCommand(CommandType type)
        {
            string select;

            string begin = "";

            switch (type)
            {
                case CommandType.Count:
                    begin = "SELECT";
                  

                    if (ExtraSelect == null)
                    {
                        select = "COUNT(*)";
                    }
                    else
                    {
                        select = String.Format("COUNT(*), {1}", MainTable, ExtraSelect);
                    }

                    break;
                case CommandType.Select:
                    begin = "SELECT DISTINCT";
                    if (ExtraSelect == null)
                    {
                        select = String.Format("{0}.*", MainTable);
                    }
                    else
                    {
                        select = String.Format("{0}.*, {1}", MainTable, ExtraSelect );
                    }
                    break;
                case CommandType.Delete:
                    begin = "DELETE";
                    select = "";
                    break;
                default:
                    begin = "";
                    select = "";
                    break;
            }

            MySqlCommand command = new MySqlCommand();

            //  string query = "SELECT " + select + " FROM episode WHERE 1=1";


            FillArgumentsJoins(command);


            string query = begin + " " + select + " FROM " + MainTable;

            string joins = JoinString;
            if ( joins != "")
            {
                query += " " + joins;
            }

            string args = ArgumentString;

            if ( args != "")
            {
                query += " WHERE " + args;
            }

            string groups = GroupsString;
            if (groups != "")
            {
                query += " GROUP BY " + groups;
            }


            string havings = HavingsString;
            if (!havings.IsNullOrEmpty())
            {
                query += String.Format(" HAVING {0}", havings);
            }

                if (type == CommandType.Select)
                {
                    string orderString;
                    if (Order == Order.Ascending)
                    {
                        orderString = "ASC";
                    }
                    else
                    {
                        orderString = "DESC";
                    }

                    query += " ORDER BY " + OrderByString + " " + orderString;
                }

                if (type != CommandType.Count && Count != null )
                {
                    query += " LIMIT ?startIndex, ?count";
                    command.Parameters.AddWithValue("?startIndex", StartIndex);
                    command.Parameters.AddWithValue("?count", Count);
                }
          


            command.CommandText = query;




            return command;
        }



        public MySql.Data.MySqlClient.MySqlCommand GetSelectCommand()
        {
            return GetCommand(CommandType.Select);
        }

        public MySql.Data.MySqlClient.MySqlCommand GetCountCommand()
        {
            return GetCommand(CommandType.Count);
        }

        public MySqlCommand GetDeleteCommand()
        {
            return GetCommand(CommandType.Delete);
        }
    }
}

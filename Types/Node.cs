using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MySql.Data.MySqlClient;
using Spaetzel.UtilityLibrary;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Spaetzel.UtilityLibrary.Types
{

    public abstract class Node : IComparable
    {

        protected bool _updateMade = false;
        private string _description = "";
        private int _id = 0;
        private string _name = "";
        private int _addedBy = 0;
        private DateTime _dateAdded;
        private bool _doneLoading = false;

        private string _shortName = "";



        public virtual string ShortName
        {
            get {
                if ((_shortName == "" && Name != "" ) || ( _shortName.Contains("Untitled") && !Name.Contains("Untitles") ) )
                {
                    string[] words = Name.StripTags().StripNonAlphabet().ToLower().Split(' ');

                    string shortName = "";
                    int wordCount = 0;
                    foreach (var curWord in words)
                    {
                        if (shortName.Length < 20 && wordCount <3)
                        {
                            shortName += curWord.FirstToUpper();
                            wordCount++;
                        }
                    }

                    _shortName = GetUniqueShortName(shortName.Substring(0, Math.Min(40, shortName.Length)));

                }

                return _shortName;
            
            }
            set { 
                _shortName = GetUniqueShortName( value.Substring(0, Math.Min(40, value.Length)) );
            }
        }

        protected virtual string GetUniqueShortName(string shortName)
        {
            return GetUniqueShortName(shortName, 1);
        }

        private string GetUniqueShortName(string shortName, int increment)
        {
            string output;

            if (increment > 1)
            {
                output = shortName + increment.ToString();
            }
            else
            {
                output = shortName;
            }

            Node exists = GetObjectByShortName(output);

            if (exists == null || exists.Id == this.Id )
            {
                // There isn't already an object with this shortname
                return output;
            }
            else
            {
                return GetUniqueShortName(shortName, increment + 1);
            }

        }

        protected abstract Node GetObjectByShortName(string shortName);


        public Node()
        {
            DateAdded = DateTime.Now;
        }


        /// <summary>
        /// If this object has been finished loading
        /// </summary>
        protected bool DoneLoading
        {
            get
            {
                return _doneLoading;
            }
            set
            {
                _doneLoading = value;
                if (value == true)
                {
                    _updateMade = false;
                }
            }
        }


        /// <summary>
        /// If an update has been made to this object since it was first loaded
        /// </summary>
        public bool UpdateMade
        {
            get { return _updateMade; }
            protected set { _updateMade = value; }

        }

 


        /// <summary>
        /// The local path that can be followed to see more about this object
        /// </summary>
        public virtual string ViewPath
        {
            get
            {
                return "";
            }
            set
            {
            }
        }


        /// <summary>
        /// The description of this node with all html tags stripped
        /// </summary>
        public string StrippedDescription
        {
            get
            {
                return Utilities.StripTags(this.Description);
            }
        }

        /// <summary>
        /// A long description of this object
        /// </summary>
        public virtual string Description
        {
            get { return _description; }
            set
            {
                if (value != "")
                {
                    _description = value;
                    _updateMade = true;
                }
            }
        }

        /// <summary>
        /// A unique ID number associated with this object
        /// </summary>
        public int Id
        {
            get { return _id; }
            set
            {

                _id = value;


            }
        }


        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (!value.IsNullOrEmpty() )
                {
                    _name = Utilities.StripTags(value.Substring(0, Math.Min(value.Length, 255)));
                    _updateMade = true;

                   
                }
            }
        }

        /// <summary>
        /// The ID number of the user that added this object to the database
        /// </summary>
        public int AddedBy
        {
            get { return _addedBy; }
            set
            {
                _addedBy = value;
                _updateMade = true;
            }
        }

        /// <summary>
        /// The date this object was added to the database
        /// </summary>
        public virtual DateTime DateAdded
        {
            get { return _dateAdded; }
            set
            {
                _dateAdded = value;
                _updateMade = true;
            }
        }

        public virtual void FillFromReader(MySqlDataReader reader)
        {
            this.Id = DBInterface.GetReaderInt(reader, "id");
            this.Name = DBInterface.GetReaderString(reader, "name");

            this.Description = DBInterface.GetReaderString(reader, "description");
            this.AddedBy = DBInterface.GetReaderInt(reader, "addedBy");

            _shortName = DBInterface.GetReaderString(reader, "shortName");
            if (Name.IsNullOrEmpty())
            {
                Name = _shortName;
            }
            this.DateAdded = DBInterface.GetReaderDateTime(reader, "dateAdded");
        }

        public virtual void FillCommandParameters(MySqlCommand command)
        {
            command.Parameters.AddWithValue("?name", this.Name.Trim().StripTags());
            command.Parameters.AddWithValue("?description", this.Description.Trim().FilterTags());
            command.Parameters.AddWithValue("?shortName", this.ShortName.StripTags());
            command.Parameters.AddWithValue("?addedBy", this.AddedBy);
            command.Parameters.AddWithValue("?dateAdded", this.DateAdded);
        }

        public virtual XElement XElement
        {
            get
            {
                XElement element = new XElement("item",
                    new XElement("link", this.ViewPath),
                     new XElement("title", this.Name),
                     new XElement("description", this.Description)
                     );

                return element;
            }
        }

        public override string ToString()
        {
            return Name;
        }

 

        #region ISerializable Members

        protected Node(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetInt32("id");
            Name = info.GetString("name");
            Description = info.GetString("description");
            DateAdded = info.GetDateTime("dateAdded");
            AddedBy = info.GetInt32("addedBy");

        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("id", this.Id);
            info.AddValue("name", this.Name);
            info.AddValue("description", this.Description);
            info.AddValue("dateAdded", this.DateAdded);
            info.AddValue("addedBy", this.AddedBy);

        }

        #endregion


        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            if (obj.GetType() == this.GetType())
            {
                Node compareTo = (Node)obj;

                return this.Id.CompareTo(compareTo.Id);
            }
            else
            {
                throw new Exception(String.Format("Cannot compare {0} and {1}", this.GetType().ToString(), obj.GetType().ToString()));
            }
        }

        #endregion
    }
}

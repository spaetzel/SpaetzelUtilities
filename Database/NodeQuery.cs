using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Spaetzel.UtilityLibrary.Database
{
    public enum SortBy { DateAdded = 0, FeaturedLevel = 1, Random = 2, Name = 3, Relevance = 4};
    

    public abstract class NodeQuery : DBQuery
    {
        private SortBy _sortBy = SortBy.DateAdded;

   

        public string SearchTerm { get; set; }
        public int? AddedBy { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public DateTime? MinDateAdded { get; set; }
        public DateTime? MaxDateAdded { get; set; }
  

    

        public SortBy SortBy
        {
            get { return _sortBy; }
            set { 
                _sortBy = value;

                switch (value)
                {
                    
                    case SortBy.Name:
                        Order = Order.Ascending;
                        break;
                    case SortBy.DateAdded:
                    case SortBy.FeaturedLevel:
                        Order = Order.Descending;
                        break;
                }
            }
        }

        public virtual string SearchMatch
        {
            get
            {
                return "name, shortName";
            }
        }


        public override string ExtraSelect
        {
            get
            {
                switch (SortBy)
                {
                    case SortBy.Relevance:
                        return String.Format("MATCH ({0}) AGAINST(?searchTerm) AS relevance", SearchMatch);
                    default:
                        return base.ExtraSelect;
                }
            }
        }
        protected override string OrderByString
        {
            get {
                switch (SortBy)
                {
                    case SortBy.DateAdded:
                        return MainTable + ".dateAdded";
                    case SortBy.FeaturedLevel:
                        return MainTable + ".featuredLevel";
                    case SortBy.Name:
                        return MainTable + ".name";
                            
                    case SortBy.Random:
                        return "RAND()";
                    case SortBy.Relevance:
                        return "Relevance";
                    default:
                        throw new NotImplementedException("Sort type not implmented");
                }
            }
        }

        protected override void FillArgumentsJoins(MySqlCommand command)
        {
            base.FillArgumentsJoins(command);

            if (AddedBy != null)
            {
                Arguments.Add("addedBy = ?addedBy");
                command.Parameters.AddWithValue("?addedBy", AddedBy);
            }


            if (ShortName != null && ShortName != String.Empty )
            {
                Arguments.Add("shortName = ?shortName");
                command.Parameters.AddWithValue("?shortName", ShortName);
            }
            if (SearchTerm != null && SearchTerm != "")
            {
                Arguments.Add(String.Format("MATCH ({0}) AGAINST(?searchTerm IN BOOLEAN MODE)", SearchMatch));
                command.Parameters.AddWithValue("?searchTerm", SearchTerm);
            }

            if (Description != null && Description != String.Empty)
            {
                Arguments.Add("description = ?description");
                command.Parameters.AddWithValue("?description", Description);
            }

            if (MinDateAdded != null)
            {
                Arguments.Add( MainTable + ".dateAdded >= ?minDateAdded");
                command.Parameters.AddWithValue("?minDateAdded", MinDateAdded);
            }

            if (MaxDateAdded != null)
            {
                Arguments.Add(MainTable + ".dateAdded <= ?maxDateAdded");
                command.Parameters.AddWithValue("?maxDateAdded", MaxDateAdded);
            }

         

        }
    

       

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spaetzel.UtilityLibrary
{
    public enum SaveFailureReason { AlreadyExists, Exception };

    public class DBSaveException : Exception
    {
        private SaveFailureReason _reason;
        private int _existsId;

        public int ExistsId
        {
            get { return _existsId; }
            set { _existsId = value; }
        }

        public SaveFailureReason Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }

        public DBSaveException(string message, int existsId)
            : this( message, SaveFailureReason.AlreadyExists )
        {
            this.ExistsId = existsId;
        }

        public DBSaveException(string message, SaveFailureReason reason)
            : base(message)
        {
            this.Reason = reason;
        }

        public DBSaveException(Exception ex)
            : base(ex.Message, ex)
        {
            this.Reason = SaveFailureReason.Exception;
        }
    }
}

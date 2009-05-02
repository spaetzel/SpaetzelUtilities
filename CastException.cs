using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spaetzel.UtilityLibrary
{
    public class CastException : Exception
    {
        public CastException(string message)
        {
            bool showExceptions = true;

            if (showExceptions)
            {
                throw new Exception(message);
            }
        }

    }
}

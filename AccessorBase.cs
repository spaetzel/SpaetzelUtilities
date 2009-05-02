using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web;

namespace Spaetzel.UtilityLibrary
{
    public class AccessorBase
    {
        private static bool? _useCache;

        public static Cache Cache
        {
            get
            {
                try
                {
                    return HttpContext.Current.Cache;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static bool UseCache
        {
            get
            {
                if (_useCache == null)
                {
                    _useCache = Utilities.GetAppConfig("useCache") == "true";
                }

                return ( (bool)_useCache && Cache != null );
                
            }
            set
            {
                _useCache = value;
            }
        }

    }
}

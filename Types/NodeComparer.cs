using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Spaetzel.UtilityLibrary.Types
{
    public class NodeComparer<T> : IEqualityComparer
    {
        #region IEqualityComparer Members

        bool IEqualityComparer.Equals(object x, object y)
        {
            try
            {
                Node nodeX = (Node)x;
                Node nodeY = (Node)y;

                return nodeX.Id == nodeY.Id;
            }
            catch
            {
                return false;
            }
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spaetzel.UtilityLibrary
{
    /// <summary>
    /// Declares that a <see cref="Page"/> supports 
    /// RSS feed discovery.
    /// </summary>
    public interface ISupportRssFeedDiscovery
    {
        /// <summary>
        /// Gets a <see cref="String"/> which contains the 
        /// URL of the RSS feed.
        /// </summary>
        String RssFeedUrl
        { get; }

        /// <summary>
        /// Gets a <see cref="String"/> which contains the 
        /// title of the RSS feed.
        /// </summary>
        String RssFeedTitle
        { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

/*
namespace Spaetzel.UtilityLibrary.HtmlCleaner
{
    /// <summary>
    /// This comes from here http://netcode.ru/dotnet/?artID=7730
    /// </summary>
    /// <summary>
    /// This class skips all nodes which has some
    /// kind of prefix. This trick does the job 
    /// to clean up MS Word/Outlook HTML markups.
    /// </summary>
    public class HtmlReader : Sgml.SgmlReader
    {
        public HtmlReader(TextReader reader)
            : base()
        {
            base.InputStream = reader;
            base.DocType = "HTML";
        }
        public HtmlReader(string content)
            : base()
        {
            base.InputStream = new StringReader(content);
            base.DocType = "HTML";
        }
        public override bool Read()
        {
            bool status = base.Read();
            if (status)
            {
                if (base.NodeType == XmlNodeType.Element)
                {
                    // Got a node with prefix. This must be one
                    // of those "<o:p>" or something else.
                    // Skip this node entirely. We want prefix
                    // less nodes so that the resultant XML 
                    // requires not namespace.
                    if (base.Name.IndexOf(':') > 0)
                        base.Skip();
                }
            }
            return status;
        }
    }
}
 * */

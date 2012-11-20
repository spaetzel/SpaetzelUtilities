using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
/*
namespace CastRollerDA.HtmlCleaner
{
	/// <summary>
	/// This comes from here http://netcode.ru/dotnet/?artID=7730
	/// </summary>
public class HtmlWriter : XmlTextWriter
{
 /// <summary>
 /// If set to true, it will filter the output
 /// by using tag and attribute filtering,
 /// space reduce etc
 /// </summary>
 public bool FilterOutput = false;

 /// <summary>
 /// If true, it will reduce consecutive &nbsp; with one instance
 /// </summary>
 public bool ReduceConsecutiveSpace = true;

 /// <summary>
 /// Set the tag names in lower case which are allowed to go to output
 /// </summary>
 public string [] AllowedTags = 
  new string[] { "p", "b", "i", "u", "em", "big", "small", 
  "div", "img", "span", "blockquote", "code", "pre", "br", "hr", 
  "ul", "ol", "li", "del", "ins", "strong", "a", "font", "dd", "dt"};

 /// <summary>
 /// If any tag found which is not allowed, it is replaced by this tag.
 /// Specify a tag which has least impact on output
 /// </summary>
 public string ReplacementTag = "dd";

 /// <summary>
 /// New lines \r\n are replaced with space which saves space and makes the
 /// output compact
 /// </summary>
 public bool RemoveNewlines = true;
 /// <summary>
 /// Specify which attributes are allowed. Any other attribute will be discarded
 /// </summary>
 public string [] AllowedAttributes = new string[] { "class", "href", "target", 
  "border", "src", "align", "width", "height", "color", "size" };

	/// <summary>
/// The reason why we are overriding
/// this method is, we do not want the output to be
/// encoded for texts inside attribute
/// and inside node elements. For example, all the &nbsp;
/// gets converted to &amp;nbsp in output. But this does not 
/// apply to HTML. In HTML, we need to have &nbsp; as it is.
/// </summary>
/// <param name="text"></param>
public override void WriteString(string text)
{
 // Change all non-breaking space to normal space
 text = text.Replace( " ", "&nbsp;" );
 /// When you are reading RSS feed and writing Html, this line helps remove
 /// those CDATA tags
 text = text.Replace("<![CDATA[","");
 text = text.Replace("]]>", "");

 // Do some encoding of our own because
 // we are going to use WriteRaw which won't
 // do any of the necessary encoding
 text = text.Replace( "<", "<" );
 text = text.Replace( ">", ">" );
 text = text.Replace( "'", "&apos;" );
 text = text.Replace( "\"", "e;" );

  if( this.FilterOutput )
  {
	text = text.Trim();

	// We want to replace consecutive spaces
	// to one space in order to save horizontal width
	if( this.ReduceConsecutiveSpace ) 
	  text = text.Replace("&nbsp;&nbsp;&nbsp;", "&nbsp;");
	if( this.RemoveNewlines ) 
	  text = text.Replace(Environment.NewLine, " ");

	base.WriteRaw( text );
  }
  else
  {
	base.WriteRaw( text );
  }
}

	public override void WriteStartElement(string prefix, 
							 string localName, string ns)
{
  if( this.FilterOutput ) 
  {
	bool canWrite = false;
	string tagLocalName = localName.ToLower();
	foreach( string name in this.AllowedTags )
	{
	  if( name == tagLocalName )
	  {
		canWrite = true;
		break;
	  }
	}
	if( !canWrite ) 
	  localName = "dd";
  }
  base.WriteStartElement(prefix, localName, ns);
}


	public void WriteAttributes()
	{
		bool canWrite = false;
string attributeLocalName = reader.LocalName.ToLower();
foreach( string name in this.AllowedAttributes )
{
  if( name == attributeLocalName )
  {
	canWrite = true;
	break;
  }
}

 // If allowed, write the attribute
 if( canWrite ) 
   this.WriteStartAttribute(reader.Prefix, 
		attributeLocalName, reader.NamespaceURI);

 while (reader.ReadAttributeValue())
 {
   if (reader.NodeType == XmlNodeType.EntityReference)
   {
	 if( canWrite ) this.WriteEntityRef(reader.Name);
	   continue;
   }
   if( canWrite )this.WriteString(reader.Value);
 }
 if( canWrite ) this.WriteEndAttribute();
	}
}
}

*/
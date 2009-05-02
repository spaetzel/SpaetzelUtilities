using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.UI;
using System.Runtime.Remoting.Contexts;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Spaetzel.UtilityLibrary
{
    public static class Utilities
    {
        public static bool IsNullOrEmpty( this string test )
        {
            if (test == null)
            {
                return true;
            }
            else
            {
                return String.IsNullOrEmpty(test.Trim() );
            }
        }

        public static string EmailRegex
        {
            get
            {
                return @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            }
        }

        public static Control FindControl(System.Web.UI.Page page, string controlId)
        {
           
            Control result = null;

            result = page.FindControl(controlId);

            if (result == null)
            {
               

                MasterPage curMaster = page.Master;
                int counter = 0;
                do
                {
                    
                        result = curMaster.FindControl(controlId);
                    if (curMaster != null)
                    {
                        curMaster = curMaster.Master;
                        counter++;
                    }
                } while (result == null && counter < 10 && curMaster != null);
            }

            /* 
            if (result == null)
            {
                foreach (Control curControl in page.Controls)
                {
                    if (curControl != null)
                    {
                        try
                        {

                            if (((Control)curControl).ID.Contains(controlId))
                            {
                                return curControl;
                            }
                            else
                            {
                                try
                                {
                                    var curContent = (Content)curControl;

                                    foreach (Control curContentControl in curContent.Controls)
                                    {
                                        if (((Control)curControl).ID.Contains(controlId))
                                        {
                                            return (Control)curControl;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            */

            return result;
        }

        
        public static string GetAppConfig(string key)
        {
            string result = ConfigurationSettings.AppSettings[key];

            if (String.IsNullOrEmpty(result))
            {
                result = GetAppConfig(key, false);
            }

            return result;
        }

        public static string GetAppConfig(string key, bool includeEnv)
        {
            string result = ConfigurationSettings.AppSettings[key];

            if (result != null)
            {
                return result;
            }
            else if (result == null && includeEnv == false)
            {
                string environment = ConfigurationSettings.AppSettings["environment"];

                if (environment == null)
                    throw new Exception("Environment config is null");

                return GetAppConfig(key + "_" + environment, true);
            }
            else
            {
                throw new Exception("Key " + key + " not found in configuration settings");
            }
        }

       

        public static string UploadFile(FileUpload control, string destinationPath)
        {
            string fn = System.IO.Path.GetFileName(control.PostedFile.FileName);
            string saveLocation = destinationPath + "\\" + fn;

            try
            {
                control.PostedFile.SaveAs(saveLocation);
                return saveLocation;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static bool CheckValidEmail(string email)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                     @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                     @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(email))
                return (true);
            else
                return (false);
                
        }
            

        public static string FormatDate(DateTime date)
        {
            if( date == DateTime.Today )
            {
                return "Today";
            }
            else if (DateTime.Today - date == new TimeSpan(1, 0, 0, 0 ))
            {
                return "Yesterday";
            }
            else if (DateTime.Today - date < new TimeSpan(4, 0, 0, 0))
            {
                return date.ToString("dddd");
            }
            else
            {
                return date.ToString("ddd MMMM d, yyyy");
            }
        }

        public static string FormatDateTimeRFC1132(DateTime date)
        {
            System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.DateTimeFormatInfo();

            string output = DateTime.Now.ToUniversalTime().ToString(dtfi.RFC1123Pattern);

            return output;
        }
        public static string FormatDateTime(DateTime date)
        {
            return FormatDate(date) + " " + date.ToString("h:mm tt");
        }

        public static string FormatDateTime(DateTime? date)
        {
            if (date == null)
            {
                return "";
            }
            else
            {
                return FormatDateTime((DateTime)date);
            }
        }

       

        /// <summary>
        /// Adds the Base url for the site to the front of the given path, turning it into an absolute URL
        /// </summary>
        /// <param name="path"></param>
        public static string BaseifyUrl(string url)
        {
            string baseUrl = Utilities.GetAppConfig("BaseUrl");


            if (url[0] != '/')
            {
               url = string.Format("/{0}", url);
            }

            if (baseUrl != "/")
            {
                return String.Format("{0}{1}", baseUrl, url);
            }
            else
            {
                return url;
            }
        }

        public static string GetFilenameExtension(string fileName)
        {
            string extension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
            int questionIndex = extension.IndexOf('?');
            if (questionIndex > 0)
            {
                extension = extension.Substring(0, questionIndex);
            }

            return extension;
        }

        public static string DownloadFile(string directory, string url)
        {
            if (url.Length > 0)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }


           //     string fileName = url.Substring(url.LastIndexOf("/") + 1);

                string extension = Utilities.GetFilenameExtension(url);
                    

                string fileName = CalculateMd5Hash(url) + "." + extension;

                System.Net.WebClient client = new System.Net.WebClient();

                string fullPath = directory + "/" + fileName;

                if (!File.Exists(fullPath))
                {
                    // Don't download the file twice

                    try
                    {

                        client.DownloadFile(url.Trim(), fullPath.Trim());
                    }
                    catch (WebException ex)
                    {
                        // Got a 404 or 500 from the site
                        return "";
                        //return fullPath;
                    }

                   
                }

                return fullPath;

            }
            else
            {
                return "";
            }
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }


        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        // Hash an input string and return the hash as
        // a 32 character hexadecimal string.
        public static string CalculateMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static bool SelectListItem(ListControl list, string value, bool clearSelection)
        {
            if (clearSelection)
                list.ClearSelection();

            return SelectListItem(list, value);
        }

        public static bool SelectListItem(ListControl list, string value)
        {
            ListItem curItem = list.Items.FindByValue(value);
            if (curItem != null)
            {

                curItem.Selected = true;


                return true;
            }
            else
            {

                return false;
            }
        }

        public static List<string> AutodetectFeeds(string url)
        {
            List<string> output = new List<string>();
            try
            {
                WebClient client = new WebClient();

                string html = client.DownloadString(url);

                Regex linkPattern = new Regex("<link[^>]+", RegexOptions.IgnoreCase);

                MatchCollection links = linkPattern.Matches(html);



                Regex urlPattern = new Regex("href=\"([^\"]+)\"", RegexOptions.IgnoreCase);

                foreach (var curMatch in links)
                {
                    string curLink = curMatch.ToString().ToLower();

                    if (curLink.Contains("rel=\"alternate\""))
                    {
                        Match htmls = urlPattern.Match(curLink);


                        output.Add(htmls.Groups[1].Value);
                    }
                }

            }
            catch
            {
            }
            return output;


        }

        public static string StripTags(this string input)
        {
            if (input == null || input == String.Empty)
            {
                return input;
            }
            else
            {
                string pattern = @"<(.|\n)*?>";
                return Regex.Replace(input, pattern, string.Empty);
            }
        }

        public static string FilterTags(this string input)
        {
           // return Regex.Replace(input, @"<(.|\n)*?>", string.Empty);
            string output = input;
            string validTags = "((a)|(p)|(img)|(i)|(b)|(strong))";

            string pattern = String.Format("<[/]?{0}?(([^>]*)|())?>", validTags);

            string singlePattern = String.Format("<[/]?{0}(( )|(>))", validTags);
            var matches = Regex.Matches(input, pattern);

            foreach (Match curMatch in matches)
            {
                var matchStrings = from Group m in curMatch.Groups
                                   where Regex.IsMatch(m.Value, singlePattern)
                                   select m.Value;

                if (matchStrings.Count() == 0)
                {
                    output = output.Replace(curMatch.Value, "");
                }
            }


            return output;

        }

        /// <summary>
        /// Strips all characters that are not regular alphabet characters
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripAllNonAlphabet(this string input)
        {
            string pattern = "[^A-Za-z]";

            return Regex.Replace(input, pattern, string.Empty);


        }

        /// <summary>
        /// Strips non alphabet characters
        /// Also allows newlines, spaces and tabs
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripNonAlphabet(this string input)
        {
            string pattern = "[^A-Za-z\n\r ]";

            return Regex.Replace(input, pattern, string.Empty);


        }

        public static string FirstToUpper(this string name)
        {
            if (name.Length <= 1) return name.ToUpper();
                return Char.ToUpper(name[0]).ToString() + name.Substring(1);

            Char[] letters = name.ToCharArray();
            letters[0] = Char.ToUpper(letters[0]);
            return new string(letters);
        }

        public static string LimitLength(this string input, int length)
        {
            return (input.Substring(0, Math.Min(length, input.Length)));
        }

        public static string HttpPost(string uri, string parameters)
        {

            return HttpPost(uri, "", "", parameters);
          
        } // end HttpPost

        public static string CleanUri(string inputUrl)
        {
            string output = inputUrl;

            if (!output.Contains("http://"))
            {
                output = "http://" + output;
            }

            output = output.Replace("\r", "");

            return output;
        }


        public static string Shorten(this string input, int length)
        {
            return input.Shorten(length, 10);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <param name="pasdding">How much less than or greater than the length the output can be</param>
        /// <returns></returns>
        public static string Shorten( this string input, int length, int padding )
        {
            if (length < input.Length)
            { 

                int? firstPeriod = null;
                int? firstSpace = null;

                for (int i = length - padding; i < length + padding && i < input.Length; i++)
                {
                    char curChar = input[i];

                    if (curChar == '.' && firstPeriod == null)
                    {
                        firstPeriod = i;
                    }
                    else if (curChar == ' ' && firstSpace == null)
                    {
                        firstSpace = i;
                    }
                }

                if (firstPeriod != null && (firstSpace == null || firstPeriod <= firstSpace))
                {
                    return input.Substring(0, (int)firstPeriod) + "...";
                }
                else if (firstSpace != null)
                {
                    return input.Substring(0, (int)firstSpace) + "...";
                }
                else
                {
                    return input.Substring(0, length) + "...";
                }
            }
            else
            {
                return input;
            }



        }

        public static IEnumerable<T> RandomSample<T>(
           this IEnumerable<T> source, int count, bool allowDuplicates)
        {
            if (source == null) throw new ArgumentNullException("source");
            return RandomSampleIterator<T>(source, count, -1, allowDuplicates);
        }



        public static IEnumerable<T> RandomSample<T>(
        this IEnumerable<T> source, int count, int seed,
           bool allowDuplicates)
        {
            if (source == null) throw new ArgumentNullException("source");
            return RandomSampleIterator<T>(source, count, seed,
                allowDuplicates);
        }

        public static IEnumerable<Country> GetCountryList()
        {
            #region Country Names
            string[] countryNames = {	"US USA",
	"CA Canada",
	"AF Afghanistan",

	"AL Albania",
	"DZ Algeria",
	"AS American Samoa",
	"AD Andorra",
	"AO Angola",
	"AI Anguilla",

	"AQ Antartica",
	"AG Antigua and Barbuda",
	"AR Argentina",
	"AM Armenia",
	"AW Aruba",
	"AU Australia",

	"AT Austria",
	"AZ Azerbaijan",
	"BS Bahamas",
	"BH Bahrain",
	"BD Bangladesh",
	"BB Barbados",

	"BE Belgium",
	"BZ Belize",
	"BJ Benin",
	"BM Bermuda",
	"BT Bhutan",
	"BO Bolivia",

	"BA Bosnia-Herzegovina",
	"BW Botswana",
	"BV Bouvet Island",
	"BR Brazil",
	"IO British Indian Ocean Territory",
	"BN Brunei Darussalam",

	"BG Bulgaria",
	"BF Burkina Faso",
	"BI Burundi",
	"KH Cambodia",
	"CM Cameroon",
	"CV Cape Verde",

	"KY Cayman Islands",
	"CF Central African Republic",
	"TD Chad",
	"CL Chile",
	"CN China",
	"CX Christmas Island",

	"CC Cocos (Keeling) Islands",
	"CO Colombia",
	"KM Comoros",
	"CG Congo",
	"CK Cook Islands",
	"CR Costa Rica",

	"CI Cote D'Ivoire",
	"HR Croatia",
	"ZC Curacao",
	"CY Cyprus",
	"CZ Czech Republic",
	"DK Denmark",

	"DJ Djibouti",
	"DM Dominica",
	"DO Dominican Republic",
	"TP East Timor",
	"EC Ecuador",
	"EG Egypt",

	"SV El Salvador",
	"GQ Equatorial Guinea",
	"EE Estonia",
	"ET Ethiopia",
	"FO Faeroe Islands",
	"FK Falkland Islands (Malvinas)",

	"FJ Fiji",
	"FI Finland",
	"FR France",
	"GF French Guiana",
	"PF French Polynesia",
	"TF French Southern Territories",

	"GA Gabon",
	"GM Gambia",
	"GE Georgia",
	"DE Germany",
	"GH Ghana",
	"GI Gibraltar",

	"GR Greece",
	"GL Greenland",
	"GD Grenada",
	"GP Guadeloupe",
	"GU Guam",
	"GT Guatemala",

	"GG Guernsey, C.I.",
	"GN Guinea",
	"GW Guinea-Bissau",
	"GY Guyana",
	"HT Haiti",
	"HM Heard and McDonald Islands",

	"HN Honduras",
	"HK Hong Kong",
	"HU Hungary",
	"IS Iceland",
	"IN India",
	"ID Indonesia",

	"IQ Iraq",
	"IE Ireland",
	"IM Isle of Man",
	"IL Israel",
	"IT Italy",
	"JM Jamaica",

	"JP Japan",
	"JE Jersey, C.I.",
	"JO Jordan",
	"KZ Kazakhstan",
	"KE Kenya",
	"KI Kiribati",

	"KP Korea, Dem. People's Rep of",
	"KR Korea, Republic of",
	"KW Kuwait",
	"KG Kyrgyzstan",
	"LA Lao Peoples Democratic Republi",
	"LV Latvia",

	"LB Lebanon",
	"LS Lesotho",
	"LR Liberia",
	"LY Libyan Arab Jamahiriya",
	"LI Liechtenstein",
	"LT Lithuania",

	"LU Luxembourg",
	"MO Macau",
	"MG Madagascar",
	"MW Malawi",
	"MY Malaysia",
	"MV Maldives",

	"ML Mali",
	"MT Malta",
	"MH Marshall Islands",
	"MQ Martinique",
	"MR Mauritania",
	"MU Mauritius",

	"MX Mexico",
	"FM Micronesia, Fed. States of",
	"MD Moldova, Republic of",
	"MC Monaco",
	"MN Mongolia",
	"ME Montenegro",

	"MS Montserrat",
	"MA Morocco",
	"MZ Mozambique",
	"NA Namibia",
	"NR Nauru",
	"NP Nepal",

	"AN Netherland Antilles",
	"NT Neutral Zone (Saudi/Iraq)",
	"NC New Caledonia",
	"NZ New Zealand",
	"NI Nicaragua",
	"NE Niger",

	"NG Nigeria",
	"NU Niue",
	"NF Norfolk Island",
	"MP Northern Mariana Islands",
	"NO Norway",
	"OM Oman",

	"PK Pakistan",
	"PW Palau",
	"PA Panama",
	"PZ Panama Canal Zone",
	"PG Papua New Guinea",
	"PY Paraguay",

	"PE Peru",
	"PH Philippines",
	"PN Pitcairn",
	"PL Poland",
	"PT Portugal",
	"PR Puerto Rico",

	"QA Qatar",
	"RE Reunion",
	"RO Romania",
	"RU Russian Federation",
	"RW Rwanda",
	"KO S. Korea",

	"KN Saint Kitts and Nevis",
	"LC Saint Lucia",
	"WS Samoa",
	"SM San Marino",
	"ST Sao Tome and Principe",
	"SA Saudi Arabia",

	"SN Senegal",
	"RS Serbia",
	"SC Seychelles",
	"SL Sierra Leone",
	"SG Singapore",
	"SK Slovakia",

	"SI Slovenia",
	"SB Solomon Islands",
	"SO Somalia",
	"ZA South Africa",
	"ES Spain",
	"LK Sri Lanka",

	"SH St. Helena",
	"PM St. Pierre and Miquelon",
	"VC St. Vincent and the Grenadines",
	"SR Suriname",
	"SJ Svalbard and Jan Mayen Islands",
	"SZ Swaziland",

	"SE Sweden",
	"CH Switzerland",
	"TW Taiwan",
	"TJ Tajikistan",
	"TZ Tanzania, United Republic of",
	"TH Thailand",

	"NL the Netherlands",
	"TG Togo",
	"TK Tokelau",
	"TO Tonga",
	"TT Trinidad and Tobago",
	"TN Tunisia",

	"TR Turkey",
	"TM Turkmenistan",
	"TC Turks and Caicos Islands",
	"TV Tuvalu",
	"AE U.A.E.",
	"UM U.S.Minor Outlying Islands",

	"UG Uganda",
	"UA Ukraine",
	"GB United Kingdom",
	"UY Uruguay",
	"UZ Uzbekistan",
	"VU Vanuatu",

	"VA Vatican City State",
	"VE Venezuela",
	"VN Viet Nam",
	"VG Virgin Islands (British)",
	"VI Virgin Islands, U.S.",
	"WF Wallis and Futuna Islands",

	"EH Western Sahara",
	"YE Yemen, Republic of",
	"ZR Zaire",
	"ZM Zambia",
	"ZW Zimbabwe"};
            #endregion

            var countries = from c in countryNames
                        select new Country
                        {
                            Code = c.Substring(0, 2),
                            Name = c.Substring(3)
                        };

            return countries;
        }

        static IEnumerable<T> RandomSampleIterator<T>(IEnumerable<T> source,
            int count, int seed, bool allowDuplicates)
        {

            // take a copy of the current list

            List<T> buffer = new List<T>(source);

            // create the "random" generator, time dependent or with 

            // the specified seed

            Random random;
            if (seed < 0)
                random = new Random();
            else
                random = new Random(seed);

            count = Math.Min(count, buffer.Count);

            if (count > 0)
            {
                // iterate count times and "randomly" return one of the 

                // elements

                for (int i = 1; i <= count; i++)
                {
                    // maximum index actually buffer.Count -1 because 

                    // Random.Next will only return values LESS than 

                    // specified.

                    int randomIndex = random.Next(buffer.Count);
                    yield return buffer[randomIndex];
                    if (!allowDuplicates)
                        // remove the element so it can't be selected a 

                        // second time

                        buffer.RemoveAt(randomIndex);

                }
            }
        }

        public static void SaveCookie(string cookieName, string value, TimeSpan expires, string domain)
        {
            //Create a new cookie, passing the name into the constructor
            HttpCookie cookie = new HttpCookie(cookieName);

            if (!domain.IsNullOrEmpty())
            {
                cookie.Domain = domain;
            }

            //Set the cookies value
            cookie.Value = value;

            //Set the cookie to expire in 1 minute
            DateTime dtNow = DateTime.Now;
          
            cookie.Expires = dtNow + expires;

            //Add the cookie
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void DeleteCookie(string cookieName, string domain)
        {
            //Create a new cookie, passing the name into the constructor
            HttpCookie cookie = new HttpCookie(cookieName);

            if (!domain.IsNullOrEmpty())
            {
                cookie.Domain = domain;
            }



            //Set the cookies value
            cookie.Value = "";

            //Set the cookie to expire in 1 minute
            DateTime dtNow = DateTime.Now;

            cookie.Expires = dtNow + new TimeSpan(-1,0,0);

            //Add the cookie
            HttpContext.Current.Response.Cookies.Add(cookie);
        }


        public static string GetCookieValue(string cookieName)
        {
            return HttpContext.Current.Request.Cookies[cookieName].Value;
        }

        public static string HttpPost(string uri, string username, string password, string parameters)
        {

            string uriString;
          
            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();

            if (!username.IsNullOrEmpty())
            {
                myWebClient.Credentials = new NetworkCredential(username, password);
            }

            myWebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            
            // Apply ASCII Encoding to obtain the string as a byte array.
            byte[] byteArray = Encoding.ASCII.GetBytes(parameters);

        
            // Upload the input string using the HTTP 1.0 POST method.
            byte[] responseArray;

            ServicePointManager.Expect100Continue = false;

            try
            {
                responseArray = myWebClient.UploadData(uri, "POST", byteArray);

                // Decode and display the response.
                return Encoding.ASCII.GetString(responseArray);
            }
            catch (WebException ex)
            {
                throw ex;
            }
            catch (Exception ex2)
            {
                throw ex2;
            }


            //System.Net.WebClient client = new WebClient();

            //client.
            //WebRequest webRequest = WebRequest.Create(uri);

            //webRequest.ContentType = "application/x-www-form-urlencoded";
            //webRequest.Method = "POST";

            //if (!username.IsNullOrEmpty())
            //{
            //    webRequest.Credentials = new NetworkCredential(username, password);
            //}

            //byte[] bytes = Encoding.ASCII.GetBytes(parameters);
            //Stream os = null;
            //try
            //{ // send the Post
            //    webRequest.ContentLength = bytes.Length;   //Count bytes to send
            //    os = webRequest.GetRequestStream();
            //    os.Write(bytes, 0, bytes.Length);         //Send it
            //}
            //catch (WebException ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    if (os != null)
            //    {
            //        os.Close();
            //    }
            //}

            //try
            //{ // get the response
            //    WebResponse webResponse = webRequest.GetResponse();
            //    if (webResponse == null)
            //    { return null; }
            //    StreamReader sr = new StreamReader(webResponse.GetResponseStream());
            //    return sr;
            //}
            //catch (WebException ex)
            //{
            //    throw ex;
            //}
       
        }

        public static string UrlEncode(this string input)
        {
            return HttpUtility.UrlEncode(input);
        }

        public static string UrlDecode(this string input)
        {
            return HttpUtility.UrlDecode(input);
        }

        public static string ShortenUrl(string url )
        {
            try
            {
                string shortenerUrl = String.Format("http://is.gd/api.php?longurl={0}", url );

                System.Net.WebClient client = new System.Net.WebClient();
                Stream stream = client.OpenRead(shortenerUrl);

                StreamReader reader = new StreamReader(stream);

                return reader.ReadToEnd(); 
            }
            catch
            {
                return url;
            }
        }
    }
}

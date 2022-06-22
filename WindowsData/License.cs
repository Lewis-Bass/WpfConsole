using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WindowsData
{
    public class License
    {
        /// <summary>
        /// What is the name of the product licensed?
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// type of license ("Demo", "Full")
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Version of the license
        /// </summary>
        public string Version { get; set; }
        //public string DeviceName { get; set; }

        /// <summary>
        /// Status of the license (not used currently)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Unique ID of the device?????
        /// </summary>
        public string Device_Id { get; set; } 

        /// <summary>
        /// Email of the license owner
        /// </summary>
        public string Emaail { get; set; }

        /// <summary>
        /// When does the license expire?
        /// This is a string value from json!
        /// To get the value as a date use Expiration Date
        /// </summary>
        public string Expiration { get; set; }


        /// <summary>
        /// When does the license expire?
        /// This is a date value from json!
        /// </summary>
        [XmlIgnore]
        public DateTime ExpirationDate
        {
            get
            {
                if (DateTime.TryParse(Expiration, out var dt))
                {
                    return dt;
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }
    }
}

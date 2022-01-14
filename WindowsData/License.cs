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
        public string Type { get; set; }


        public string Expiration { get; set; }
        public string DeviceName { get; set; }

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

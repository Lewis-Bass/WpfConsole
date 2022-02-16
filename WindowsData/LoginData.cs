using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WindowsData
{
    public class LoginData
    {
        /// <summary>
        /// IP address to be used in the login
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Library card name that is used
        /// </summary>
        public string AccessKeyName { get; set; }

        /// <summary>
        /// Port number 
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// PIN number
        /// </summary>
        public string Pin { get; set; }

    }
}

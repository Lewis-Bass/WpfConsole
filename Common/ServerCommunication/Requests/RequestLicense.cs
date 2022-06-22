using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsData;

namespace Common.ServerCommunication.Requests
{
    public class RequestLicense //: BaseRequest
    {

        /// <summary>
        /// Name of the Product ("Vault"). For future Reference 
        /// </summary>
        public string Product { get; set; } = string.Empty;

        /// <summary>
        /// Type of the license or "Demo"
        /// </summary>
        public string LicenseType { get; set; } = string.Empty;

        /// <summary>
        /// License owners email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Device Name (Iphone, Windows etc..)
        /// </summary>
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>
        /// User Name of who owns the license. Leave blank for Demo
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Phone Number of who owns the license. Leave blank for Demo
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Not Used currently. For future use 
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Not Used currently. For future use 
        /// </summary>
        public string CreditName { get; set; } = string.Empty;


        /// <summary>
        /// Not Used currently. For future use 
        /// </summary>
        public string CreditNumber { get; set; } = string.Empty;


        /// <summary>
        /// Not Used currently. For future use 
        /// </summary>
        public string SecurityCode { get; set; } = string.Empty;

        /// <summary>
        /// Device specific information
        /// </summary>
        public List<LicenseDeviceInfo> DeviceInfo { get; set; } = new List<LicenseDeviceInfo>();

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsData
{
    public class DeviceInfo
    {
        /// <summary>
        /// Type of the stat - example Disk
        /// </summary>
        public string StatisticType { get; set; }
        /// <summary>
        /// Category of the stat - C: or the name of the disk
        /// </summary>
        public string StatisticCategory { get; set; }
        /// <summary>
        /// Name of the stat - Available Disk Space
        /// </summary>
        public string StatisticName { get; set; }
        /// <summary>
        /// The statistic value
        /// </summary>
        public string StatisticValue { get; set; }

        public DeviceInfo(string type, string category, string name, string value)
        {
            StatisticType = type;
            StatisticCategory = category;
            StatisticName = name;
            StatisticValue = value;
        }
    }

    /// <summary>
    /// This class is necessary so that we can talk to the license server 
    /// Hopefully someday this will not be necessary
    /// </summary>
    public class LicenseDeviceInfo
    {
        /// <summary>
        /// Type of the stat - example Disk
        /// </summary>
        public string statistic_type { get; set; }
        /// <summary>
        /// Category of the stat - C: or the name of the disk
        /// </summary>
        public string statistic_category { get; set; }
        /// <summary>
        /// Name of the stat - Available Disk Space
        /// </summary>
        public string statistic_name { get; set; }
        /// <summary>
        /// The statistic value
        /// </summary>
        public string statistic_value { get; set; }

        public LicenseDeviceInfo(string type, string category, string name, string value)
        {
            statistic_type = type;
            statistic_category = category;
            statistic_name = name;
            statistic_value = value;
        }
    }
}

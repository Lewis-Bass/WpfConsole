using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
using System.Management;
using System.IO;
using System.Net;
using System.Reflection;
using WindowsData;
//using System.Reflection;
//using Microsoft.Win32;
//using System.Globalization;


namespace Common.Licenses
{
	public class WindowsInformation
	{

		#region create list of stats

		private List<DeviceInfo> stats = new List<DeviceInfo>();

		protected List<DeviceInfo> PullAllStats()
		{
			GetDeviceStatistics();
			//GetStorageStatistics();
			//GetProcessorStatistics(); // cant use serial number
			//GetBIOSStatistics(); // cant use serial number
			//GetMotherBoardStatistics();
			//GetNetworkAdapter();
			return stats;
		}

		private void GetDeviceStatistics()
		{

			ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
			foreach (ManagementObject mo in mos.Get())
			{
				AddProperties(mo, "OS");
			}

			mos = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
			foreach (ManagementObject mo in mos.Get())
			{
				AddProperties(mo, "OS");
			}


			//AddWmiProperties("Win32_DisplayConfiguration", new List<string>() { "Caption", "DriverVersion" },   // MSDN says we should query Win32_VideoController instead for Windows Vista and later, but this seems to be working fine for all versions.
			//    (name, value) => stats.Add(new DeviceInfo(string.Format("Display {0}", name), value)));

			stats.Add(new DeviceInfo("OS", "Processor", "Count", Environment.ProcessorCount.ToString()));
			stats.Add(new DeviceInfo("OS", "Version", string.Empty, Environment.OSVersion.VersionString));
			stats.Add(new DeviceInfo("OS", "CLR", "Version", Environment.Version.ToString()));

			try
			{
				IPAddress[] localAddresses = Dns.GetHostAddresses(string.Empty); // Per MSDN documentation this returns the IPv4 addresses of the local host for all operating systems except Windows Server 2003; for Windows Server 2003, both IPv4 and IPv6 addresses for the local host are returned.
				for (int i = 0; i < localAddresses.Length; i++)
				{
					stats.Add(new DeviceInfo("NET", "IPAddress", (i + 1).ToString(), localAddresses[i].ToString()));
				}
			}
			catch (Exception) { } // swallow the error

		}

		private void GetStorageStatistics()
		{
			foreach (string drive in Environment.GetLogicalDrives())
			{
				try
				{
					DriveInfo driveInfo = new DriveInfo(drive);
					if (driveInfo.IsReady)
					{
						//stats.Add(new DeviceInfo("Drive Name", driveInfo.Name));
						stats.Add(new DeviceInfo("Drive", driveInfo.Name, "Format", driveInfo.DriveFormat));
						stats.Add(new DeviceInfo("Drive", driveInfo.Name, "Type", driveInfo.DriveType.ToString()));
						stats.Add(new DeviceInfo("Drive", driveInfo.Name, "FullName", driveInfo.RootDirectory.FullName));
						stats.Add(new DeviceInfo("Drive", driveInfo.Name, "Volume", driveInfo.VolumeLabel));
						stats.Add(new DeviceInfo("Drive", driveInfo.Name, "Available", driveInfo.AvailableFreeSpace.ToString()));
						stats.Add(new DeviceInfo("Drive", driveInfo.Name, "Total Size", driveInfo.TotalSize.ToString()));
					}
				}
				catch (Exception) { } // swallow the error
			}

			try
			{
				// look for mounted drives, skip drives with just a drive letter
				ManagementObjectSearcher ms = new ManagementObjectSearcher("Select * from Win32_Volume");
				foreach (ManagementObject mo in ms.Get())
				{
					string name = mo["Name"].ToString();
					string root = name.IndexOf("\\") < name.Length ? name.Substring(0, name.IndexOf("\\") + 1) : name;
					DriveType driveType = (DriveType)Enum.Parse(typeof(DriveType), mo["DriveType"].ToString());
					if (name.Equals(mo["DeviceID"]) || driveType == DriveType.CDRom || name.Equals(root))
					{
						continue;
					}
					stats.Add(new DeviceInfo("Network Drive", name, "FileSystem", mo["FileSystem"].ToString()));
					stats.Add(new DeviceInfo("Network Drive", name, "Type", driveType.ToString()));
					stats.Add(new DeviceInfo("Network Drive", name, "Root", root));
					stats.Add(new DeviceInfo("Network Drive", name, "Label", mo["Label"].ToString()));
					stats.Add(new DeviceInfo("Network Drive", name, "Available", mo["FreeSpace"].ToString()));
					stats.Add(new DeviceInfo("Network Drive", name, "Total Size", mo["Capacity"].ToString()));
				}
			}
			catch (Exception) { } // swallow the error
		}

		private void GetProcessorStatistics()
		{
			ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
			foreach (ManagementObject mo in mos.Get())
			{
				AddProperties(mo, "CPU");
				break; // only get the properties of the first cpu
			}
		}

		private void GetBIOSStatistics()
		{
			ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
			foreach (ManagementObject mo in mos.Get())
			{
				AddProperties(mo, "BIOS");
			}
		}

		private void GetMotherBoardStatistics()
		{
			ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_MotherboardDevice");
			foreach (ManagementObject mo in mos.Get())
			{
				AddProperties(mo, "MB");
			}
		}

		private void GetNetworkAdapter()
		{
			var mos = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
			foreach (ManagementObject mo in mos.Get())
			{
				if ((bool)(mo.Properties["IPEnabled"].Value))
				{
					AddProperties(mo, "NET");
				}
			}
		}

		private void AddProperties(ManagementObject mo, string propertyType)
		{
			foreach (var property in mo.Properties)
			{
				if (property.Value != null)
				{
					string value = property.Value.ToString();
					if (!string.IsNullOrWhiteSpace(value) && value.ToLowerInvariant() != "system.string[]" && value.ToLowerInvariant() != "system.uint16[]")
					{
						stats.Add(new DeviceInfo(propertyType, property.Type.ToString(),  property.Name, value));
					}
				}
			}
		}

		/// <summary>
		/// Formats a number for user-friendly display.
		/// </summary> 
		private string FormatNumberTo4DigitsMaxWithUnitChar(long number)
		{
			if (number < 10000)
			{
				return string.Format("{0}", number);
			}
			else if (number < 10000000)
			{
				return string.Format("{0:#.#}K", (double)number / (double)1024); // Divide by 2^10 and add K
			}
			else if (number < 10000000000)
			{
				return string.Format("{0:#.#}M", (double)number / (double)1048576); // Divide by 2^20 and add M
			}
			else if (number < 100000000000000)
			{
				return string.Format("{0:#.#}G", (double)number / (double)1073741824); // Divide by 2^30 and add G
			}
			else
			{
				return string.Format("{0:#.#}T", (double)number / (double)1099511627776); // Divide by 2^40 and add T
			}
		}


		#endregion

		#region Processing

		public DeviceInfo GetValue(string value)
		{
			var stats = PullAllStats();
			return stats.FirstOrDefault(r=>r.StatisticName == value);
		}

		#endregion
	}


}




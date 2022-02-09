
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WindowsData;

namespace Common
{
	/// <summary>
	/// This class contains global values that can be used by all wpf screens
	/// </summary>
	public static class GlobalValues
	{
		#region globals

		/// <summary>
		/// name of the product - used in the path
		/// </summary>
		private const string PRODUCTPATH = "Vault\\";

		/// <summary>
		/// temp directory for documents that are being viewed
		/// </summary>
		private const string VIEWPATH = "View\\";

		/// <summary>
		/// directory for storing files that are checked out
		/// </summary>
		private const string CHECKPATH = "Check\\";

		/// <summary>
		/// name of the xml file that contains files that are checkedout or viewed
		/// </summary>
		private const string FILELIST = "localfiles";


		/// <summary>
		/// Name of the user settings file
		/// </summary>
		private const string USERSETTINGSFILE = "usersettings";

		/// <summary>
		/// Name of the user settings file
		/// </summary>
		private const string FILESANNERSETTINGSFILE = "filesettings";

		/// <summary>
		/// Name of the license file
		/// </summary>
		private const string LICENSEFILENAME = "license";


		/// <summary>
		/// Name of the directory that holds the log files
		/// </summary>
		private const string SERILOGDIRECTORY = "LogFiles\\";

		/// <summary>
		/// Port used when communicating with the server
		/// </summary>
		public const int Port = 27015; //44350;

		/// <summary>
		/// gets the base directory for storing local application data
		/// </summary>
		/// <returns></returns>
		public static string GetBaseDirectory()
		{
			var tempPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			var basePath = Path.Combine(tempPath, PRODUCTPATH);
			if (!Directory.Exists(basePath))
			{
				Directory.CreateDirectory(basePath);
			}

			return basePath;
		}

		/// <summary>
		/// Physical Location of the output file on the disk
		/// </summary>
		public static string LocalFilesStorageFileName
		{
			get
			{

				return Path.Combine(GetBaseDirectory(), FILELIST);
			}
		}

		/// <summary>
		/// Physical Location of the output file on the disk
		/// </summary>
		public static string UserSettingsStorageLocation
		{
			get
			{
				return Path.Combine(GetBaseDirectory(), USERSETTINGSFILE);
			}
		}

		/// <summary>
		/// Physical Location of the output file on the disk
		/// </summary>
		public static string FileScannerSettingsStorageLocation
		{
			get
			{
				return FILESANNERSETTINGSFILE;
			}
		}

		/// <summary>
		/// Physical Location of the output file on the disk
		/// </summary>
		public static string LoggingDirectory
		{
			get
			{
				return Path.Combine(GetBaseDirectory(), SERILOGDIRECTORY);
			}
		}

		/// <summary>
		/// Physical full path to the license file
		/// </summary>
		public static string LicenseFileName
		{
			get
			{
				return Path.Combine(GetBaseDirectory(), LICENSEFILENAME);
			}
		}
		#endregion

		#region connections

		/// <summary>
		/// Contains the last login connection
		/// </summary>
		public static ConnectionInformation LastConnection { get; set; }

		/// <summary>
		/// Is this connection valid
		/// </summary>
		public static bool IsConnectionValid
		{
			get
			{
				return LastConnection != null && LastConnection.IsCurrentConnection;
			}
		}


		/// <summary>
		/// Connection Token returned from the Ark
		/// This field should be null until the GUI logs into the ark
		/// </summary>
		public static long? ConnectionToken { get; set; } = null;

		#endregion
	}
}

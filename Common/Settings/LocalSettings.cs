using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WindowsData;

namespace Common.Settings
{
    public class LocalSettings
    {
        #region Property Wrappers

        int _ActiveTheme = 0;
        /// <summary>
        /// Integer representing the theme that has been chosen by the user
        /// </summary>
        public int ActiveTheme
        {
            get { return _ActiveTheme; }
            set
            {
                if (_ActiveTheme != value)
                {
                    _ActiveTheme = value;
                    WriteFile();
                }
            }
        }

        int _MaxConnections = 5;
        /// <summary>
        /// maximum number of connections to show on the left navigator
        /// </summary>
        public int MaxConnections
        {
            get { return _MaxConnections; }
            set
            {
                if (_MaxConnections != value)
                {
                    _MaxConnections = value;
                    WriteFile();
                }
            }
        }

        int _MaxFilesViewed = 5;
        /// <summary>
        /// maximum number of files viewed to show on the left navigator
        /// </summary>
        public int MaxFilesViewed
        {
            get { return _MaxFilesViewed; }
            set
            {
                if (_MaxFilesViewed != value)
                {
                    _MaxFilesViewed = value;
                    WriteFile();
                }
            }
        }

        List<ConnectionInformation> _ConnectionsData = new List<ConnectionInformation>();
        /// <summary>
        /// connections information for library cards that have been added
        /// </summary>
        public List<ConnectionInformation> ConnectionsData
        {
            get
            {
                return _ConnectionsData;
            }
            set
            {
                if (_ConnectionsData == null || _ConnectionsData != value)
                {
                    ConnectionsData = value;
                    WriteFile();
                }
            }
        }

        int _MaxFilesCheckedOutViewed = 5;
        /// <summary>
        /// Maximum number of checked out files to view
        /// </summary>
        public int MaxFilesCheckedOutViewed
        {
            get { return _MaxFilesCheckedOutViewed; }
            set
            {
                if (_MaxFilesCheckedOutViewed != value)
                {
                    _MaxFilesCheckedOutViewed = value;
                    WriteFile();
                }
            }
        }

        bool _CheckOutShowOnlyMyFiles = false;
        /// <summary>
        /// stores the option selected by the user on check in screen
        /// </summary>
        public bool CheckOutShowOnlyMyFiles
        {
            get { return _CheckOutShowOnlyMyFiles; }
            set
            {
                if (_CheckOutShowOnlyMyFiles != value)
                {
                    _CheckOutShowOnlyMyFiles = value;
                    WriteFile();
                }
            }
        }

        bool _CheckOutRemoveFileAtCheckin = false;
        /// <summary>
        /// shows the option selected by the user on the check in screen
        /// </summary>
        public bool CheckOutRemoveFileAtCheckin
        {
            get { return _CheckOutRemoveFileAtCheckin; }
            set
            {
                if (_CheckOutRemoveFileAtCheckin != value)
                {
                    _CheckOutRemoveFileAtCheckin = value;
                    WriteFile();
                }
            }
        }

        string _Language = "English";
        /// <summary>
        /// Which language was selected by the user
        /// </summary>
        public string Language
        {
            get { return _Language; }
            set
            {
                if (_Language != value)
                {
                    _Language = value;
                    WriteFile();
                }
            }
        }

        ConnectionInformation _LastConnection = null;
        /// <summary>
        /// Contains the last login connection
        /// </summary>
        public ConnectionInformation LastConnection
        {
            get { return _LastConnection; }
            set
            {
                if (_LastConnection != value)
                {
                    _LastConnection = value;
                    WriteFile();
                }
            }
        }

        ConnectionInformation _AutoLoadConnection = null;
        /// <summary>
        /// Autoload connection
        /// </summary>
        public ConnectionInformation AutoLoadConnection
        {
            get { return _AutoLoadConnection; }
            set
            {
                if (_AutoLoadConnection != value)
                {
                    _AutoLoadConnection = value;
                    WriteFile();
                }
            }
        }

        int _AutoLoadStartTime = 0;
        /// <summary>
        /// AutoLoadService starts after this hour
        /// </summary>
        public int AutoLoadStartTime
        {
            get { return _AutoLoadStartTime; }
            set
            {
                if (_AutoLoadStartTime != value)
                {
                    _AutoLoadStartTime = value;
                    WriteFile();
                }
            }
        }

        int _AutoLoadEndTime = 5;
        /// <summary>
        /// AutoLoad service finishes after this hour
        /// </summary>
        public int AutoLoadEndTIme
        {
            get { return _AutoLoadEndTime; }
            set
            {
                if (_AutoLoadEndTime != value)
                {
                    _AutoLoadEndTime = value;
                    WriteFile();
                }
            }
        }

        string _AutoLoadPIN = String.Empty;
        /// <summary>
        /// AutoLoad uses this pin to connect
        /// </summary>
        public string AutoLoadPin
        {
            get { return _AutoLoadPIN; }
            set
            {
                if (_AutoLoadPIN != value)
                {
                    _AutoLoadPIN = value;
                    WriteFile();
                }
            }
        }

        DateTime _AutoLoadLastProcessed = DateTime.MinValue;
        /// <summary>
        /// AutoLoad Service was last run on this day
        /// </summary>
        public DateTime AutoLoadLastProcessed
        {
            get { return _AutoLoadLastProcessed; }
            set
            {
                if (_AutoLoadLastProcessed != value)
                {
                    _AutoLoadLastProcessed = value;
                    WriteFile();
                }
            }
        }
        int _AutoLoadLastTotalUpload = 0;
        /// <summary>
        /// AutoLoad Service Total Number of files loaded
        /// </summary>
        public int AutoLoadLastTotalUpload
        {
            get { return _AutoLoadLastTotalUpload; }
            set
            {
                if (_AutoLoadLastTotalUpload != value)
                {
                    _AutoLoadLastTotalUpload = value;
                    WriteFile();
                }
            }
        }

        List<String> _AutoLoadDirectories = new List<String>();
        /// <summary>
        /// AutoLoad service will scan these directories
        /// </summary>
        public List<string> AutoLoadDirectories
        {
            get { return _AutoLoadDirectories; }
            set
            {
                if (_AutoLoadDirectories != value)
                {
                    _AutoLoadDirectories = value;
                    WriteFile();
                }
            }
        }

        #endregion

        #region Constructor

        ///// <summary>
        ///// stop the default constructor from being executed - use the load function instead!
        ///// </summary>
        //public LocalSettings()
        //{          
        //}

        #endregion

        #region File Handlers

        /// <summary>
        /// used to determine if we are reading the file and setting up properties
        /// </summary>
        //private static readonly object readingFile = new object();
        private static bool readingFile = false;

        /// <summary>
        /// read the configuration from the local storage
        /// </summary>
        public static LocalSettings Load()
        {
            LocalSettings settings = null;
            ////lock (readingFile)
            ////{
            //Monitor.Enter(readingFile);
            readingFile = true;
            if (!File.Exists(GlobalValues.UserSettingsStorageLocation))
            {
                settings = new LocalSettings();
            }
            else
            {
                TextReader textReader = null;
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(LocalSettings));
                    textReader = new StreamReader(GlobalValues.UserSettingsStorageLocation);
                    settings = (LocalSettings)serializer.Deserialize(textReader);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "Can not load user settings");
                    settings = new LocalSettings();
                }
                finally
                {
                    if (textReader != null)
                    {
                        textReader.Close();
                    }
                }
            }
            //if (!settings.ConnectionsData.Any(r => r.AccessKeyName == ConnectionInformation.LocalAdminName))
            //{
            //    // this connection should always exist
            //    settings.AddConnection(new ConnectionInformation { AccessKeyName = ConnectionInformation.LocalAdminName, IPAddress = "localhost", IsCurrentConnection = false });
            //}
            //}
            //Monitor.Exit(readingFile);

            readingFile = false;
            return settings;
        }

        /// <summary>
        /// write the configuration to the local storage
        /// </summary>
        private void WriteFile()
        {
            if (readingFile)
            {
                return;
            }

            //Monitor.TryEnter(readingFile);
            if (File.Exists(GlobalValues.UserSettingsStorageLocation))
            {
                File.Delete(GlobalValues.UserSettingsStorageLocation);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(LocalSettings));
            TextWriter textWriter = new StreamWriter(GlobalValues.UserSettingsStorageLocation);
            serializer.Serialize(textWriter, this);
            textWriter.Close();
            //Monitor.Exit(readingFile);
        }

        #endregion

        #region Connection handling

        /// <summary>
        /// Save the object to the system settings as an encoded string
        /// </summary>
        /// <param name="info"></param>
        /// <param name="propertyName"></param>
        public void AddConnection(ConnectionInformation info)
        {
            // does the connection exist?
            ConnectionInformation existing = ConnectionsData.FirstOrDefault(r => r.AccessKeyName == info.AccessKeyName);
            if (existing != null)
            {
                existing.AccessKeyName = info.AccessKeyName;
                existing.IPAddress = info.IPAddress;
                existing.IsCurrentConnection = info.IsCurrentConnection;
            }
            else
            {
                ConnectionsData.Add(info);
            }
            WriteFile();
        }

        /// <summary>
        /// Save the object to the system settings as an encoded string
        /// </summary>
        /// <param name="info"></param>
        /// <param name="propertyName"></param>
        public void RemoveConnection(ConnectionInformation info)
        {
            // does the connection exist?
            ConnectionInformation existing = ConnectionsData.FirstOrDefault(r => r.AccessKeyName == info.AccessKeyName);
            if (existing != null)
            {
                ConnectionsData.Remove(existing);
            }
            WriteFile();
        }

        #endregion
    }
}

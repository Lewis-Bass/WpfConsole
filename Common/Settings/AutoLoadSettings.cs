using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WindowsData;

namespace Common.Settings
{
   public class AutoLoadSettings 
    {

        #region Properties

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
        public int AutoLoadEndTime
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

        #region File Handlers

        /// <summary>
        /// used to determine if we are reading the file and setting up properties
        /// </summary>
        private static bool readingFile = false;

        /// <summary>
        /// read the configuration from the local storage
        /// </summary>
        public static AutoLoadSettings Load()
        {
            AutoLoadSettings settings = null;
            readingFile = true;
            if (!File.Exists(GlobalValues.FileScannerSettingsStorageLocation))
            {
                settings = new AutoLoadSettings();
            }
            else
            {
                TextReader textReader = null;
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AutoLoadSettings));
                    textReader = new StreamReader(GlobalValues.UserSettingsStorageLocation);
                    settings = (AutoLoadSettings)serializer.Deserialize(textReader);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "Can not load user settings");
                    settings = new AutoLoadSettings();
                }
                finally
                {
                    if (textReader != null)
                    {
                        textReader.Close();
                    }
                }
            }
            
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
            if (File.Exists(GlobalValues.FileScannerSettingsStorageLocation))
            {
                File.Delete(GlobalValues.FileScannerSettingsStorageLocation);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(AutoLoadSettings));
            TextWriter textWriter = new StreamWriter(GlobalValues.FileScannerSettingsStorageLocation);
            serializer.Serialize(textWriter, this);
            textWriter.Close();
        }

        #endregion
    }
}

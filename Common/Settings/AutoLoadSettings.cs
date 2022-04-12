using Common.ServerCommunication;
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
        /// AutoLoad connection
        /// </summary>
        public ConnectionInformation AutoLoadConnection
        {
            get { return _AutoLoadConnection; }
            set
            {
                if (_AutoLoadConnection != value)
                {
                    _AutoLoadConnection = value;
                    //WriteFile();
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
                    //WriteFile();
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
                    //WriteFile();
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
                    //WriteFile();
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
                    //WriteFile();
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
                    //WriteFile();
                }
            }
        }

        List<DirectoriesToScan> _AutoLoadDirectories = new List<DirectoriesToScan>();
        /// <summary>
        /// AutoLoad service will scan these directories
        /// </summary>
        public List<DirectoriesToScan> AutoLoadDirectories
        {
            get { return _AutoLoadDirectories; }
            set
            {
                if (_AutoLoadDirectories != value)
                {
                    _AutoLoadDirectories = value;
                    //WriteFile();
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
        public static AutoLoadSettings Load(bool loadFromFile)
        {
            if (loadFromFile)
            {
                return ReadFromFile(); // used by the file scan service
            }
            else
            {
                return ReadFromWebSite(); // used by the console 
            }
        }

        // TODO: Async method
        private static AutoLoadSettings ReadFromWebSite()
        {
            LoginData loginData = SetupLogin();
            //https://localhost:5001/settings/readsettings
            //var uri = new Uri($"https://{loginData.IPAddress}:{GlobalValues.FileScanPort}/settings/readsettings");
            var uri = new Uri($"http://{loginData.IPAddress}:{GlobalValues.FileScanPort}/settings/readsettings");
            var returnStr = SendToServer.SendRest(loginData, uri).Result;
            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<AutoLoadSettings>(returnStr.ToString());
            return response;
        }

        private static LoginData SetupLogin()
        {
            // TODO: Use Global Connection?????
            return new LoginData
            {
                AccessKeyName = "Local Admin",
                Port = 5001,
                IPAddress = "localhost",
                Pin = string.Empty
            };
        }

        public bool WriteToWebSite()
        {
            LoginData loginData = SetupLogin();
            //https://localhost:5001/settings/readsettings
            //var uri = new Uri($"https://{loginData.IPAddress}:{loginData.Port}/settings/writesettings");
            //var uri = new Uri($"https://{loginData.IPAddress}:{GlobalValues.FileScanPort}/settings/writesettings");
            var uri = new Uri($"http://{loginData.IPAddress}:{GlobalValues.FileScanPort}/settings/writesettings");
            var returnStr = SendToServer.SendRest(this, uri).Result;
            //var response = Newtonsoft.Json.JsonConvert.DeserializeObject<AutoLoadSettings>(returnStr.ToString());
            //return response;
            return true;
        }

        /// <summary>
        /// Read from the local file - should only be used by a service!
        /// </summary>
        /// <returns></returns>
        private static AutoLoadSettings ReadFromFile()
        {
            AutoLoadSettings settings = null;
            string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GlobalValues.FileScannerSettingsStorageLocation);
            readingFile = true;
            if (!File.Exists(filename))
            {
                settings = new AutoLoadSettings();
            }
            else
            {
                TextReader textReader = null;
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AutoLoadSettings));
                    textReader = new StreamReader(filename);
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
        public void WriteFile()
        {
            if (readingFile)
            {
                return;
            }

            string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GlobalValues.FileScannerSettingsStorageLocation);

            //Monitor.TryEnter(readingFile);
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(AutoLoadSettings));
            TextWriter textWriter = new StreamWriter(filename);
            serializer.Serialize(textWriter, this);
            textWriter.Close();
        }

        #endregion
    }
}

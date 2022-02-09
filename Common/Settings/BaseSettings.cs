//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Serialization;

//namespace Common.Settings
//{
//    public class BaseSettings
//    {

//        #region File Handlers

//        /// <summary>
//        /// used to determine if we are reading the file and setting up properties
//        /// </summary>
//        //private static readonly object readingFile = new object();
//        private static bool readingFile = false;

//        /// <summary>
//        /// read the configuration from the local storage
//        /// </summary>
//        public static LocalSettings Load()
        
//        {
//            LocalSettings settings = null;
//            ////lock (readingFile)
//            ////{
//            //Monitor.Enter(readingFile);
//            readingFile = true;
//            if (!File.Exists(GlobalValues.UserSettingsStorageLocation))
//            {
//                settings = new LocalSettings();
//            }
//            else
//            {
//                TextReader textReader = null;
//                try
//                {
//                    XmlSerializer serializer = new XmlSerializer(typeof(LocalSettings));
//                    textReader = new StreamReader(GlobalValues.UserSettingsStorageLocation);
//                    settings = (LocalSettings)serializer.Deserialize(textReader);
//                }
//                catch (Exception ex)
//                {
//                    Serilog.Log.Error(ex, "Can not load user settings");
//                    settings = new LocalSettings();
//                }
//                finally
//                {
//                    if (textReader != null)
//                    {
//                        textReader.Close();
//                    }
//                }
//            }
 
//            readingFile = false;
//            return settings;
//        }

//        /// <summary>
//        /// write the configuration to the local storage
//        /// </summary>
//        public void WriteFile()
//        {
//            if (readingFile)
//            {
//                return;
//            }

//            //Monitor.TryEnter(readingFile);
//            if (File.Exists(GlobalValues.UserSettingsStorageLocation))
//            {
//                File.Delete(GlobalValues.UserSettingsStorageLocation);
//            }

//            XmlSerializer serializer = new XmlSerializer(typeof(LocalSettings));
//            TextWriter textWriter = new StreamWriter(GlobalValues.UserSettingsStorageLocation);
//            serializer.Serialize(textWriter, this);
//            textWriter.Close();
//            //Monitor.Exit(readingFile);
//        }

//        #endregion
//    }
//}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Common;

namespace WindowsData
{
    public class UserSettings
    {
		#region constructor

        public UserSettings()
		{


		}
		#endregion

		#region properties

		/// <summary>
		/// Integer representing the theme that has been chosen by the user
		/// </summary>
		public int ActiveTheme { get; set; } = 0;

        /// <summary>
        /// maximum number of connections to show on the left navigator
        /// </summary>
        public int MaxConnections { get; set; } = 5;

        /// <summary>
        /// maximum number of files viewed to show on the left navigator
        /// </summary>
        public int MaxFilesViewed { get; set; } = 5;

        /// <summary>
        /// Connection Information.
        /// </summary>
        public ConnectionInformation[] ConnectionsData { get; set; }

        /// <summary>
        /// Maximum number of checked out files to view
        /// </summary>
        public int MaxFilesCheckedOutViewed { get; set; } = 5;

        /// <summary>
        /// stores the option selected by the user on check in screen
        /// </summary>
        public bool CheckOutShowOnlyMyFiles { get; set; } = true;

        /// <summary>
        /// shows the option selected by the user on the check in screen
        /// </summary>
        public bool CheckOutRemoveFileAtCheckin { get; set; } = false;

        /// <summary>
        /// Which language was selected by the user
        /// </summary>
        public string Language { get; set; } = "English";

		/////////// <summary>
		/////////// list of available connections info
		/////////// </summary>
		////////public string LastConnectionsData { get; set; }
		///

#endregion

		////#region File Handlers


		/////// <summary>
		/////// read the configuration from the local storage
		/////// </summary>
		////private void ReadFile()
  ////      {
  ////          if (File.Exists(GlobalValues.UserSettingsStorageLocation))
  ////          {
  ////              XmlSerializer serializer = new XmlSerializer(AllSettings.GetType());
  ////              TextReader textReader = new StreamReader(GlobalValues.UserSettingsStorageLocation);
  ////              AllSettings = (UserSettings)serializer.Deserialize(textReader);
  ////              textReader.Close();
  ////          }
  ////      }

  ////      /// <summary>
  ////      /// write the configuration to the local storage
  ////      /// </summary>
  ////      private void WriteFile()
  ////      {

  ////          if (File.Exists(GlobalValues.UserSettingsStorageLocation))
  ////          {
  ////              File.Delete(GlobalValues.UserSettingsStorageLocation);
  ////          }

  ////          XmlSerializer serializer = new XmlSerializer(AllSettings.GetType());
  ////          TextWriter textWriter = new StreamWriter(GlobalValues.UserSettingsStorageLocation);
  ////          serializer.Serialize(textWriter, AllSettings);
  ////          textWriter.Close();

  ////      }

  ////      #endregion

    }
}

using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WindowsData;

namespace WpfConsole.Helpers

{
    public class PropertyHelpers
    {

        #region ConnectionInfo

        ///////// <summary>
        ///////// Load the comma delimited string into the properties class
        ///////// </summary>
        ///////// <param name="collectionString"></param>
        ///////// <returns></returns>
        //////public ObservableCollection<ConnectionInformation> LoadConnectionInfoFromProperty(string collectionString)
        //////{

        //////    var retval = new ObservableCollection<ConnectionInformation>();
            

        //////    //////var retval = new ObservableCollection<ConnectionInformation>();
        //////    //////if (!String.IsNullOrWhiteSpace(collectionString))
        //////    //////{
        //////    //////    foreach (var info in collectionString.Split('|'))
        //////    //////    {
        //////    //////        if (!string.IsNullOrWhiteSpace(info))
        //////    //////        {
        //////    //////            var parts = info.Split(',');
        //////    //////            if (parts.Length >= 5)
        //////    //////            {
        //////    //////                //int p = 0;
        //////    //////                //int.TryParse(parts[1], out p);
        //////    //////                bool inuse = false;
        //////    //////                bool.TryParse(parts[4], out inuse);
        //////    //////                retval.Add(new ConnectionInformation
        //////    //////                {
        //////    //////                    IPAddress = parts[0],
        //////    //////                    //ConnectionName = parts[1],
        //////    //////                    AccessKeyName = parts[2],
        //////    //////                    //Password = parts[3],
        //////    //////                    IsCurrentConnection = inuse
        //////    //////                });
        //////    //////            }
        //////    //////        }
        //////    //////    }
        //////    //////}
        //////    return retval;
        //////}

        ///////////// <summary>
        ///////////// Save the object to the system settings as an encoded string
        ///////////// </summary>
        ///////////// <param name="info"></param>
        ///////////// <param name="propertyName"></param>
        //////////public void SaveConnectionInfoToProperty(ConnectionInformation info)
        //////////{
        //////////    var sb = new StringBuilder();
        //////////    var settings = LocalSettings.Load();

        //////////    settings.ConnectionsData.Add(info);
            
        //////////}

        //////public enum ConnectionPropertyNames
        //////{
        //////    MaxConnectionsData,
        //////    LastConnectionsData
        //////}

        #endregion
    }
}

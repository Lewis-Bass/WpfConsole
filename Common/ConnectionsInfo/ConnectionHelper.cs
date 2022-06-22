using Common.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsData;

namespace Common.ConnectionInfo
{
    public class ConnectionHelper
    {

        public ConnectionInformation[] GetAllConnections()
        {
            LocalSettings settings = LocalSettings.Load();

            // add the local host setting if it is not there
            if (settings.ConnectionsData.Any(r => r.AccessKeyName == ConnectionInformation.LocalAdminName) == false)
            {
                // this connection should always exist
                var conn = new ConnectionInformation { AccessKeyName = ConnectionInformation.LocalAdminName, IPAddress = "localhost", IsCurrentConnection = false };
                settings.AddConnection(conn);
                settings.LastConnection = conn;
            }

            return settings.ConnectionsData.ToArray();
        }

    }
}

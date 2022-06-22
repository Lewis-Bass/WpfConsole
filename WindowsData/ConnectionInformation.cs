
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WindowsData
{
    public class ConnectionInformation : INotifyPropertyChanged
    {

        public static string LocalAdminName { get { return Resources.Resource.LocalAdminName; } }

        string _IPAddress;
        /// <summary>
        /// IP address of the connection
        /// </summary>
        public string IPAddress
        {
            get
            {
                return _IPAddress;
            }
            set
            {
                if (_IPAddress != value)
                {
                    _IPAddress = value;
                    OnPropertyChanged("IPAddress");
                }
            }
        }

        string _AccessKeyName;
        /// <summary>
        /// Key Name - name of the key associated with the ark
        /// </summary>
        public string AccessKeyName
        {
            get
            {
                return _AccessKeyName;
            }
            set
            {
                if (_AccessKeyName != value)
                {
                    _AccessKeyName = value;
                    OnPropertyChanged("LoginName");
                }
            }
        }

        bool _IsCurrentConnection = false;
        /// <summary>
        /// GUI ONLY PROPERTY - IS THIS CONNECTION THE CURRENT ONE?
        /// </summary>
        public bool IsCurrentConnection
        {
            get
            {
                return _IsCurrentConnection;
            }
            set
            {
                if (_IsCurrentConnection != value)
                {
                    _IsCurrentConnection = value;
                    OnPropertyChanged("IsCurrentConnection");
                }
            }
        }

        /// <summary>
        /// GUI ONLY PROPERTY - IS THIS CONNECTION THE CURRENT ONE?
        /// </summary>
        public bool IsLocalAdmin
        {
            get
            {
                //return ((IPAddress != "localhost" || IPAddress != "127.0.0.1") && AccessKeyName != ConnectionInformation.LocalAdminName);
                return ((IPAddress != "localhost" || IPAddress != "127.0.0.1") && string.Compare(AccessKeyName, ConnectionInformation.LocalAdminName, true) == 0);
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}

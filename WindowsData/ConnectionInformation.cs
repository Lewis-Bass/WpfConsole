﻿
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WindowsData
{
    public class ConnectionInformation : INotifyPropertyChanged
    {

        public const string LOCALADMIN = "Local Admin";


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

        bool _IsLocalAdmin = false;
        /// <summary>
        /// GUI ONLY PROPERTY - IS THIS CONNECTION THE CURRENT ONE?
        /// </summary>
        public bool IsNotLocalAdmin
        {
            get
            {
                return _AccessKeyName != LOCALADMIN;
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WindowsData
{
    public class LocalFileStatus : INotifyPropertyChanged
    {
        /// <summary>
        /// Vault unique identifier
        /// </summary>
        string _VaultId;
        public string VaultID
        {
            get
            {
                return _VaultId;
            }
            set
            {
                if (_VaultId != value)
                {
                    _VaultId = value;
                    OnPropertyChanged("VaultID");
                }

            }
        }

        string _DocumentName;
        public string DocumentName
        {
            get
            {
                return _DocumentName;

            }
            set
            {
                if (_DocumentName != value)
                {
                    _DocumentName = value;
                    OnPropertyChanged("DocumentName");
                }
            }
        }

        string _LocalFileLocation;
        public string LocalFileLocation
        {
            get
            {
                return _LocalFileLocation;
            }
            set
            {
                if (_LocalFileLocation != value)
                {
                    _LocalFileLocation = value;
                    OnPropertyChanged("LocalFileLocation");
                }
            }
        }

        DateTime _DateRecieved;
        public DateTime DateRecieved
        {
            get
            {
                return _DateRecieved;
            }
            set
            {
                if (_DateRecieved != value)
                {
                    _DateRecieved = value;
                    OnPropertyChanged("DateRecieved");
                }
            }
        }

        bool _IsCheckedOut = false;
        public bool IsCheckedOut
        {
            get
            {
                return _IsCheckedOut;
            }
            set
            {
                if (_IsCheckedOut != value)
                {
                    _IsCheckedOut = value;
                    OnPropertyChanged("");
                }
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

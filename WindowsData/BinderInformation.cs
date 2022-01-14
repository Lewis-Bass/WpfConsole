using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WindowsData
{
    public class BinderInformation : INotifyPropertyChanged
    {

        string _BinderName = string.Empty;
        public string BinderName
        {
            get
            {
                return _BinderName;
            }
            set
            {
                if (_BinderName != value)
                {
                    _BinderName = value;
                    OnPropertyChanged("BinderName");
                }
            }
        }


        string _BinderID = string.Empty;
        public string BinderID
        {
            get
            {
                return _BinderID;
            }
            set
            {
                if (_BinderID != value)
                {
                    _BinderID = value;
                    OnPropertyChanged("BinderID");
                }
            }
        }

        DateTime _CreatedOn;
        public DateTime CreatedOn
        {
            get { return _CreatedOn; }
            set
            {
                if (_CreatedOn != value)
                {
                    _CreatedOn = value;
                    OnPropertyChanged("CreatedOn");
                }
            }
        }

        DateTime _LastUpdate
        {
            get { return _LastUpdate; }
            set
            {
                if (_LastUpdate != value)
                {
                    _LastUpdate = value;
                    OnPropertyChanged("LastUpdate");
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace WindowsData
{
    public class ConnectionModel : INotifyPropertyChanged
    {

        ObservableCollection<ConnectionInformation> _LanAddress;
        public ObservableCollection<ConnectionInformation> LanAddressInfo
        {
            get { return _LanAddress; }
            set
            {
                _LanAddress = value;
                OnPropertyChanged("LanAddressInfo");
            }
        }

        //ObservableCollection<ConnectionInformation> _PriorAddress;
        //public ObservableCollection<ConnectionInformation> PriorAddressInfo
        //{
        //    get { return _PriorAddress; }
        //    set
        //    {
        //        _PriorAddress = value;
        //        OnPropertyChanged("PriorAddressInfo");
        //    }
        //}

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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace WindowsData
{
    public class LocalFileList : INotifyPropertyChanged
    {
        private ObservableCollection<LocalFileStatus> _priorFiles = new ObservableCollection<LocalFileStatus>();
        public ObservableCollection<LocalFileStatus> Files
        {
            get
            {
                return _priorFiles;
            }
            set
            {
                _priorFiles = value;
                OnPropertyChanged("Files");
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

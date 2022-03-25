using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace WindowsData
{
    public class FileTrack : INotifyPropertyChanged
    {
        string _FileName;
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                if (_FileName != value)
                {
                    _FileName = value;
                    OnPropertyChanged("FileName");
                }
            }
        }

        ObservableCollection<MetaTags> _Tags = new ObservableCollection<MetaTags>();
        public ObservableCollection<MetaTags> Tags
        {
            get { return _Tags; }
            set
            {
                if (Tags != value)
                {
                    _Tags = value;
                    OnPropertyChanged("Tags");
                    OnPropertyChanged("TagDisplay");
                }
            }
        }

        public string TagDisplay
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                bool firstTime = true;
                foreach (var tag in Tags)
                {
                    if (firstTime)
                    {
                        sb.Append(tag.TagName);
                    }
                    else
                    {
                        sb.Append($", {tag.TagName}");
                    }
                    firstTime = false;
                }
                return sb.ToString();

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

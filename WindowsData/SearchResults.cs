

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace WindowsData
{
    public class SearchResults : INotifyPropertyChanged
    {
        #region Current Document

        bool _isDetailsShown = false;
        public bool IsDetailsShown
        {
            get { return _isDetailsShown; }
            set
            {
                if (value != _isDetailsShown)
                {
                    _isDetailsShown = value;
                    OnPropertyChanged("ShowDetails");
                    OnPropertyChanged("HideDetails");
                }
            }
        }

        string _DocumentName = string.Empty;
        public string DocumentName
        {
            get { return _DocumentName; }
            set
            {
                if (_DocumentName != value)
                {
                    _DocumentName = value;
                    OnPropertyChanged("DocumentName");
                }
            }
        }

        string _PathName = string.Empty;
        public string PathName
        {
            get { return _PathName; }
            set
            {
                if (_PathName != value)
                {
                    _PathName = value;
                    OnPropertyChanged("PathName");
                }
            }
        }

        string _DocumentDate = string.Empty;
        public string DocumentDate
        {
            get { return _DocumentDate; }
            set
            {
                if (_DocumentDate != value)
                {
                    _DocumentDate = value;
                    OnPropertyChanged("DocumentDate");
                }
            }
        }

        bool _DocumentLocked = false;
        public bool DocumentLocked
        {
            get { return _DocumentLocked; }
            set
            {
                if (_DocumentLocked != value)
                {
                    _DocumentLocked = value;
                    OnPropertyChanged("DocumentLocked");
                }
            }
        }

        //List<MetaTags> _Tags = new List<MetaTags>();
        //public List<MetaTags> Tags
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

        #endregion

        #region GUI Only

        public bool ShowDetails
        {
            get
            {
                return (IsDetailsShown);
            }
        }
        public bool HideDetails
        {
            get
            {
                return (!IsDetailsShown);
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

        #endregion

        #region Prior Version Info

        public SearchResults[] PriorVersions { get; set; }

        public int PriorVersionCount
        {
            get
            {
                return (PriorVersions != null) ? PriorVersions.Length : 0;
            }
        }

        public bool HasPriorVersions
        {
            get
            {
                return PriorVersionCount > 0;
            }
        }

        #endregion

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

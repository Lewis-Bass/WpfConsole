

using System;
using System.Collections.Generic;
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

        public string DocumentName { get; set; }
        public string PathName { get; set; }
        public string DocumentDate { get; set; }

        public string[] Tags { get; set; }

        public bool DocumentLocked { get; set; }
        
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsData
{
    public class MetaTags : INotifyPropertyChanged
    {

        #region Properties

        /// <summary>
        /// Back end identifier for the tag
        /// </summary>
        string _TagId = string.Empty;
        public string TagId
        {
            get { return _TagId; }
            set
            {
                if (string.Compare(value, _TagId) != 0)
                {
                    _TagId = value;
                    OnPropertyChanged("TagId");
                }
            }
        }

        /// <summary>
        /// name of the tag
        /// </summary>
        string _TagName = string.Empty;
        public string TagName
        {
            get { return _TagName; }
            set
            {
                if (string.Compare(value, _TagName) != 0)
                {
                    _TagName = value;
                    OnPropertyChanged("TagName");
                }
            }
        }

        int _TotalTagUsage = 0;
        /// <summary>
        /// How Many documents are tagged with this value
        /// </summary>
        public int TotalTagUsage
        {
            get { return _TotalTagUsage; }
            set
            {
                if (_TotalTagUsage != value)
                {
                    _TotalTagUsage = value;
                    OnPropertyChanged("TotalTagUsage");
                }
            }
        }

        #endregion

        #region GUI Only Properties

        public bool Selected { get; set; } = false;

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

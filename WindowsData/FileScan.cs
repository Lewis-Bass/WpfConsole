using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsData
{
    public class FileScan : INotifyPropertyChanged
    {

        /// <summary>
        /// Contains the user name who added the directory
        /// used with special operating system files
        /// </summary>
        string _UserName = string.Empty;
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                if (_UserName != value)
                {
                    _UserName = value;
                    OnPropertyChanged("UserName");
                }
            }
        }

        /// <summary>
        /// Name of the directory being scanned. this is the last part of the directory and not the full path
        /// </summary>
        string _DirectoryName;
        public string DirectoryName
        {
            get
            {
                return _DirectoryName;
            }
            set
            {
                if (_DirectoryName != value)
                {
                    _DirectoryName = value;
                    OnPropertyChanged("DirectoryName");
                }
            }
        }

        /// <summary>
        /// Full Path on the machine where it is being scanned
        /// </summary>
        string _PathName;
        public string PathName
        {
            get
            {
                return _PathName;
            }
            set
            {
                if (_PathName != value)
                {
                    _PathName = value;
                    OnPropertyChanged("PathName");
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


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace WindowsData
{
    public class SearchGroup : INotifyPropertyChanged
    {
        #region Constructor and Globals
        

        public SearchGroup(string baseGroupName) {
            BaseGroupName = baseGroupName;
        }

        #endregion

        #region Properties

        ObservableCollection<SearchCriteria> _userCriteria;
        public ObservableCollection<SearchCriteria> UserCriteria
        {
            get { return _userCriteria; }
            set
            {
                if (_userCriteria != value)
                {
                    _userCriteria = value;
                    OnPropertyChanged("UserCriteria");
                }
            }
        }

        public string GroupName
        {
            get
            {
                return $"{BaseGroupName}{GroupNumber}";
            }
        }

        /// <summary>
        /// used to establish the beginning of the group name - required for multi language support
        /// </summary>
        public string BaseGroupName { get; set; } = "Group";


        int _groupNumber;

        public int GroupNumber
        {
            get
            {
                return _groupNumber;
            }
            set
            {
                if (_groupNumber != value)
                {
                    _groupNumber = value;
                    OnPropertyChanged("GroupNumber");

                }
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

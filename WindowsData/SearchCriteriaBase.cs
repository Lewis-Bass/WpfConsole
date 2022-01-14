using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WindowsData
{
    /// <summary>
    /// Properties that are common between the GUI and the Web Site
    /// </summary>
    public class SearchCriteriaBase : INotifyPropertyChanged
    {

        #region Input Fields

        private string _field;
        /// <summary>
        /// Field to search
        /// </summary>
        public string Field
        {
            get { return _field; }
            set
            {
                if (value != _field)
                {
                    _field = value;
                    OnPropertyChanged("Field");
                    NotifyShows();
                }
            }
        }

        private string _criteria;
        /// <summary>
        /// How to search
        /// </summary>
        public string Criteria
        {
            get { return _criteria; }
            set
            {
                if (value != _criteria)
                {
                    _criteria = value;
                    OnPropertyChanged("Criteria");
                    NotifyShows();
                }
            }
        }

        private string _valueMin;
        /// <summary>
        /// Minimum Search value
        /// </summary>
        public string ValueMin
        {
            get { return _valueMin; }
            set
            {
                if (value != _valueMin)
                {
                    _valueMin = value;
                    OnPropertyChanged("ValueMin");
                    NotifyShows();
                }
            }
        }

        private string _valueMax;
        /// <summary>
        /// Maximum value
        /// </summary>
        public string ValueMax
        {
            get { return _valueMax; }
            set
            {
                if (value != _valueMax)
                {
                    _valueMax = value;
                    OnPropertyChanged("ValueMax");
                    NotifyShows();
                }
            }
        }
        private bool _valueBool;

        /// <summary>
        /// Boolean value to search
        /// </summary>
        public bool ValueBool
        {
            get { return _valueBool; }
            set
            {
                if (value != _valueBool)
                {
                    _valueBool = value;
                    OnPropertyChanged("ValueBool");
                    NotifyShows();
                }
            }
        }

        private DateTime _valueMinDate = DateTime.Now;
        /// <summary>
        /// Minimum date
        /// </summary>
        public DateTime ValueMinDate
        {
            get { return _valueMinDate; }
            set
            {
                if (value != _valueMinDate)
                {
                    _valueMinDate = value;
                    OnPropertyChanged("ValueMinDate");
                    NotifyShows();
                }
            }
        }

        private DateTime _valueMaxDate = DateTime.Now;
        /// <summary>
        /// maximum date
        /// </summary>
        public DateTime ValueMaxDate
        {
            get { return _valueMaxDate; }
            set
            {
                if (value != _valueMaxDate)
                {
                    _valueMaxDate = value;
                    OnPropertyChanged("ValueMaxDate");
                    NotifyShows();
                }
            }
        }

        private string _relationship = "&";
        /// <summary>
        /// How is the relationship to be established 
        /// & = and
        /// ^ = or
        /// ! = not
        /// </summary>
        public string Relationship
        {
            get { return _relationship; }
            set
            {
                if (value != _relationship)
                {
                    _relationship = value;
                    OnPropertyChanged("Relationship");                    
                }
            }
        }

        int _groupId;
        /// <summary>
        /// Group id
        /// This is used to add subgroups to the query
        /// </summary>
        public int GroupID
        {
            get { return _groupId; }
            set
            {
                if (value != _groupId)
                {
                    _groupId = value;
                    OnPropertyChanged("GroupID");
                }
            }
        }

        private string _groupRelationship = "^";
        /// <summary>
        /// How is the group relationship to be established 
        /// & = and
        /// ^ = or
        /// ! = not
        /// </summary>
        public string GroupRelationship
        {
            get { return _groupRelationship; }
            set
            {
                if (value != _groupRelationship)
                {
                    _groupRelationship = value;
                    OnPropertyChanged("GroupRelationship");
                }
            }
        }
        #endregion

        #region Virtual helpers

        public virtual void NotifyShows() { }

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

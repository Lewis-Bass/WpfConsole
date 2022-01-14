
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace WindowsData
{
    public class SearchInfo : INotifyPropertyChanged
    {
        public ObservableCollection<SearchGroup> UserGroups { get; set; } = new ObservableCollection<SearchGroup>();

        public ObservableCollection<SearchResults> UserResults { get; set; } = new ObservableCollection<SearchResults>();

        public SearchInfo(string groupName, Dictionary<string, string> searchFields, Dictionary<string, string> searchHow)
        {
            for (int i = 1; i <= 2; i++)
            {
                var group = new SearchGroup(groupName)
                {
                    GroupNumber = i,  
                    UserCriteria = new ObservableCollection<SearchCriteria>()
                };
                group.UserCriteria.Add(new SearchCriteria() { GroupNumber = i });

                UserGroups.Add(group);
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

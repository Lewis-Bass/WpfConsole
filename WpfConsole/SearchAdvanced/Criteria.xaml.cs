using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsData;

namespace WpfConsole.SearchAdvanced
{
    /// <summary>
    /// Interaction logic for Criteria.xaml
    /// </summary>
    public partial class Criteria : UserControl
    {

        #region Properties

        /// <summary>
        /// Search critera 
        /// </summary>
        public ObservableCollection<SearchCriteriaGUI> SearchCriteriaInfo
        {
            get
            {
                return _SearchCriteriaInfo;

            }
            set
            {
                _SearchCriteriaInfo = value;
                OnPropertyChanged("SearchCriteriaInfo");
            }
        }

        ObservableCollection<SearchCriteriaGUI> _SearchCriteriaInfo = new ObservableCollection<SearchCriteriaGUI>();

        /// <summary>
        /// Nested Group By Information
        /// </summary>
        public ObservableCollection<Criteria> GroupInfo
        {
            get
            {
                return _GroupInfo;

            }
            set
            {
                _GroupInfo = value;
                OnPropertyChanged("GroupInfo");
            }
        }
        ObservableCollection<Criteria> _GroupInfo = new ObservableCollection<Criteria>();

        /// <summary>
        /// background colors for groups
        /// </summary>
        private Dictionary<int, string> GroupColors = new Dictionary<int, string>
        {
            {1,"#641e16"},
            {2,"#512e5f"},
            {3,"#154360"},
            {4,"#0e6251"},
            {5,"#145a32"},
            {6,"#7e5109"},
            {7,"#6e2c00"},
                    
            // second set
            {8,"#c0392b"},
            {9,"#884ea0"},
            {10,"#2471a3"},
            {11,"#17a589"},
            {12,"#229954"},
            {13,"#d68910"},
            {14,"#ba4a00"},

            // third set
            {15,"#d98880"},
            {16,"#c39bd3"},
            {17,"#7fb3d5"},
            {18,"#76d7c4"},
            {19,"#7dcea0"},
            {20,"#f8c471"},
            {21,"#e59866"}
        };
        
        #endregion

        #region Constructor

        public Criteria()
        {            
            InitializeComponent();
            DataContext = this;

            LoadCriteria();
            cbSelect_Click(null, null);
        }

        #endregion

        #region initial load

        private void LoadCriteria()
        {
            SearchCriteriaInfo = new ObservableCollection<SearchCriteriaGUI> {
                new SearchCriteriaGUI()
            };
            SetFirstRecordRelationships();
        }


        #endregion

        #region Button Clicks

        /// <summary>
        ///  remove the search criteria from the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveCriteria_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.DataContext is WindowsData.SearchCriteriaGUI)
            {
                var data = (WindowsData.SearchCriteriaGUI)button.DataContext;
                SearchCriteriaInfo.Remove(data);
                // add a blank back if needed
                if (SearchCriteriaInfo.Count <= 0)
                {
                    var criteria = new SearchCriteriaGUI();
                    SearchCriteriaInfo.Add(criteria);
                }
            }
            SetFirstRecordRelationships();
        }

        /// <summary>
        /// add a new search criteria to the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCriteria_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.DataContext is WindowsData.SearchCriteriaGUI)
            {
                var data = (WindowsData.SearchCriteriaGUI)button.DataContext;

                for (int i = 0; i < SearchCriteriaInfo.Count(); i++)
                {
                    if (SearchCriteriaInfo[i] == data)
                    {
                        var rec = new SearchCriteriaGUI();
                        rec.GroupID = data.GroupID;
                        rec.GroupColor = data.GroupColor;
                        rec.ShowGroupRelationship = false;

                        SearchCriteriaInfo.Insert(i + 1, rec);
                        break;
                    }
                }
            }
            SetFirstRecordRelationships();
        }

        /// <summary>
        /// add to an existing sub group or create a new one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            // are we creating a new group or adding to an existing one?
            int groupID = 0;


            // calculate the group id - first group id selected wins!
            List<SearchCriteriaGUI> groupRecords = new List<SearchCriteriaGUI>();
            foreach (var rec in SearchCriteriaInfo)
            {
                if (rec.Select)
                {
                    if (rec.GroupID > 0)
                    {
                        if (groupID <= 0)
                        {
                            groupID = rec.GroupID;
                        }
                    }
                }
            }

            // did we find one to add to?
            if (groupID == 0)
            {
                groupID = SearchCriteriaInfo.Max(r => r.GroupID) + 1;
            }

            // remove all records from the group and set the insertion point
            int firstRecordIndex = 0;
            int currentIndex = 0;
            bool firstRecordProcessed = true;
            foreach (var rec in SearchCriteriaInfo)
            {
                if (rec.Select || rec.GroupID == groupID)
                {
                    if (firstRecordProcessed)
                    {
                        firstRecordIndex = currentIndex;
                        firstRecordProcessed = false;
                    }
                    groupRecords.Add(rec);
                }
                currentIndex++;
            }

            // setup the records...
            // 1) remove from list
            foreach (var rec in groupRecords)
            {
                SearchCriteriaInfo.Remove(rec);
            }

            // 2) add the selected records after the first selected one
            firstRecordProcessed = true;
            foreach (var rec in groupRecords)
            {
                rec.ShowGroupRelationship = firstRecordProcessed;
                rec.GroupID = groupID;
                rec.Select = false;
                rec.GroupColor = GroupColors[groupID];
                rec.ShowRelationship = !firstRecordProcessed;
                SearchCriteriaInfo.Insert(firstRecordIndex, rec);
                firstRecordIndex++;
                firstRecordProcessed = false;
            }

            SetFirstRecordRelationships();
        }

        private void SetFirstRecordRelationships()
        {
            SearchCriteriaInfo.First().ShowGroupRelationship = false;
            SearchCriteriaInfo.First().ShowRelationship = false;
        }

        /// <summary>
        /// a line was selected or unselected to become part of a group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSelect_Click(object sender, RoutedEventArgs e)
        {
            AddGroup.IsEnabled = SearchCriteriaInfo.Any(r => r.Select);
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

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

namespace WpfConsole.SearchMaster
{
    /// <summary>
    /// Interaction logic for AdvancedTree.xaml
    /// </summary>
    public partial class AdvancedTree : UserControl
    {

        #region Properties

        /// <summary>
        /// Search criteria 
        /// </summary>
        public ObservableCollection<SearchTreeGUI> SearchCriteriaInfo
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

        ObservableCollection<SearchTreeGUI> _SearchCriteriaInfo = new ObservableCollection<SearchTreeGUI>();
        public UserControl DisplayResult { get; set; }

        #endregion

        #region Constructor

        public AdvancedTree()
        {
            InitializeComponent();
            DataContext = this;

            LoadCriteria();
            ///cbSelect_Click(null, null);
        }

        #endregion

        #region initial load

        private void LoadCriteria()
        {
            SearchCriteriaInfo = new ObservableCollection<SearchTreeGUI> {
                new SearchTreeGUI()
            };
            SetFirstRecordRelationships();
            BindCriteraToControl();
        }

        private void BindCriteraToControl()
        {
            CriteriaTree.Items.Clear();
            SetFirstRecordRelationships();
            foreach (var rec in SearchCriteriaInfo)
            {
                CriteriaTree.Items.Add(rec);
            }
        }

        #endregion

        #region Helpers

        private void SetFirstRecordRelationships()
        {
            SearchCriteriaInfo.First().ShowGroupRelationship = false;
            SearchCriteriaInfo.First().ShowRelationship = false;
        }

        #endregion

        #region Buttons

        /// <summary>
        /// Execute the search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            // convert the base to the gui model
            var newGui = SearchCriteriaInfo.Select(r => new SearchCriteriaGUI
            {
                Field = r.Field,
                Relationship = r.Relationship,
                Criteria = r.Criteria,
                ValueMin = r.ValueMin,
                ValueMax = r.ValueMax,
                ValueBool = r.ValueBool,
                ValueMinDate = r.ValueMinDate,
                ValueMaxDate = r.ValueMaxDate
            }).ToArray();
            newGui[0].ShowRelationship = false;
            newGui[0].ShowGroupRelationship = false;

            // pass the query to the results screen
            RaiseEvent(new RoutedEventArgs(MainSearch.PerformSearchEvent, newGui));
        }


        /// <summary>
        ///  remove the search criteria from the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveCriteria_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.DataContext is WindowsData.SearchTreeGUI)
            {
                var data = (WindowsData.SearchTreeGUI)button.DataContext;
                bool isEntryRemoved = false;
                var criteria = RecursiveRemoveCriteria(SearchCriteriaInfo.ToList(), data, out isEntryRemoved);

                // add a blank back if needed
                if (criteria.Count <= 0)
                {
                    var rec = new SearchTreeGUI();
                    criteria.Add(rec);
                }
                SearchCriteriaInfo = new ObservableCollection<SearchTreeGUI>(criteria);
                BindCriteraToControl();
            }
        }

        private List<SearchTreeGUI> RecursiveRemoveCriteria(List<SearchTreeGUI> searchMe, SearchTreeGUI removeMe, out bool isEntryRemoved)
        {
            isEntryRemoved = false;
            for (int i = 0; i < searchMe.Count(); i++)
            {
                if (searchMe[i] == removeMe)
                {
                    searchMe.Remove(removeMe);
                    isEntryRemoved = true;
                    break;
                }
                else
                {
                    // recursive call to search sub queryies..
                    if (searchMe[i].SubQuery.Count() > 0)
                    {
                        var recs = RecursiveRemoveCriteria(searchMe[i].SubQuery.ToList(), removeMe, out isEntryRemoved);
                        if (isEntryRemoved)
                        {
                            searchMe[i].SubQuery = new ObservableCollection<SearchTreeGUI>(recs);
                            break;
                        }
                    }
                }
            }

            return searchMe;
        }

        /// <summary>
        /// add a new search criteria to the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCriteria_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.DataContext is WindowsData.SearchTreeGUI)
            {
                var data = (WindowsData.SearchTreeGUI)button.DataContext;
                bool isEntryAdded;
                var criteria = RecursiveAddCriteria(SearchCriteriaInfo.ToList(), data, out isEntryAdded);

                SearchCriteriaInfo = new ObservableCollection<SearchTreeGUI>(criteria);
                BindCriteraToControl();
            }
        }

        private List<SearchTreeGUI> RecursiveAddCriteria(List<SearchTreeGUI> searchMe, SearchTreeGUI addAfterMe, out bool isEntryAdded)
        {
            isEntryAdded = false;
            for (int i = 0; i < searchMe.Count(); i++)
            {
                if (searchMe[i] == addAfterMe)
                {
                    searchMe.Insert(i + 1, new SearchTreeGUI());
                    isEntryAdded = true;
                    break;
                }
                else
                {
                    // recursive call to search sub queryies..
                    if (searchMe[i].SubQuery.Count() > 0)
                    {
                        var recs = RecursiveAddCriteria(searchMe[i].SubQuery.ToList(), addAfterMe, out isEntryAdded);
                        if (isEntryAdded)
                        {
                            searchMe[i].SubQuery = new ObservableCollection<SearchTreeGUI>(recs);
                            break;
                        }
                    }
                }
            }
            return searchMe;
        }

        /// <summary>
        /// add to an existing sub group or create a new one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.DataContext is WindowsData.SearchTreeGUI)
            {
                var data = (WindowsData.SearchTreeGUI)button.DataContext;
                if (data.SubQuery == null)
                {
                    data.SubQuery = new ObservableCollection<SearchTreeGUI>();
                }
                var rec = new SearchTreeGUI();
                rec.ShowGroupRelationship = true;
                rec.ShowRelationship = false;
                data.SubQuery.Add(rec);
                BindCriteraToControl();
            }

            ////// are we creating a new group or adding to an existing one?
            ////int groupID = 0;


            ////// calculate the group id - first group id selected wins!
            ////List<SearchTreeGUI> groupRecords = new List<SearchTreeGUI>();
            ////foreach (var rec in SearchCriteriaInfo)
            ////{
            ////    if (rec.Select)
            ////    {
            ////        if (rec.GroupID > 0)
            ////        {
            ////            if (groupID <= 0)
            ////            {
            ////                groupID = rec.GroupID;
            ////            }
            ////        }
            ////    }
            ////}

            ////// did we find one to add to?
            ////if (groupID == 0)
            ////{
            ////    groupID = SearchCriteriaInfo.Max(r => r.GroupID) + 1;
            ////}

            ////// remove all records from the group and set the insertion point
            ////int firstRecordIndex = 0;
            ////int currentIndex = 0;
            ////bool firstRecordProcessed = true;
            ////foreach (var rec in SearchCriteriaInfo)
            ////{
            ////    if (rec.Select || rec.GroupID == groupID)
            ////    {
            ////        if (firstRecordProcessed)
            ////        {
            ////            firstRecordIndex = currentIndex;
            ////            firstRecordProcessed = false;
            ////        }
            ////        groupRecords.Add(rec);
            ////    }
            ////    currentIndex++;
            ////}

            ////// setup the records...
            ////// 1) remove from list
            ////foreach (var rec in groupRecords)
            ////{
            ////    SearchCriteriaInfo.Remove(rec);
            ////}

            ////// 2) add the selected records after the first selected one
            ////firstRecordProcessed = true;
            ////foreach (var rec in groupRecords)
            ////{
            ////    rec.ShowGroupRelationship = firstRecordProcessed;
            ////    rec.GroupID = groupID;
            ////    rec.Select = false;
            ////    rec.GroupColor = GroupColors[groupID];
            ////    rec.ShowRelationship = !firstRecordProcessed;
            ////    SearchCriteriaInfo.Insert(firstRecordIndex, rec);
            ////    firstRecordIndex++;
            ////    firstRecordProcessed = false;
            ////}

            ////SetFirstRecordRelationships();
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

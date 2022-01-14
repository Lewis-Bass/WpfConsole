using Common;
using Common.ServerCommunication.Helpers;
using Common.ServerCommunication.Requests;
using Microsoft.Win32;
using WpfConsole.Resources;
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
using WpfConsole.Search;

namespace WpfConsole.SearchAdvanced
{
    /// <summary>
    /// Interaction logic for AdvancedMain.xaml
    /// </summary>
    public partial class AdvancedMain : UserControl
    {
               
        #region Constructor

        public AdvancedMain()
        {
            InitializeComponent();

            DataContext = this;

            LoadBinderList();
            LoadCriteria();
            cbSelect_Click(null, null);
        }

        #endregion

        #region Binders - previous searches

        #region Properties

        ObservableCollection<BinderInformation> _BinderList = new ObservableCollection<BinderInformation>();
        /// <summary>
        /// Collection of binders to show on the screen
        /// </summary>
        public ObservableCollection<BinderInformation> BinderList
        {
            get { return _BinderList; }
            set
            {
                if (BinderList != value)
                {
                    _BinderList = value;
                    OnPropertyChanged("BinderList");
                }
            }
        }

        #endregion

        #region Load Properties

        private void LoadBinderList()
        {
            var request = new RequestBinderList
            {
                Connection = Common.GlobalValues.LastConnection,
                ConnectionToken = GlobalValues.ConnectionToken,
                MaxEntries = 1000
            };
            var values = BinderHelpers.GetBinderList(request);
            if (values.Any())
            {
                BinderList = new ObservableCollection<BinderInformation>(values.OrderBy(r => r.BinderName));
            }
        }

        #endregion

        #region Button Clicks

        /// <summary>
        /// delete a specific binder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteBinder_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.DataContext is BinderInformation)
            {
                var data = (BinderInformation)button.DataContext;

            }
        }

        /// <summary>
        /// the selected search binder changed - load the query
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DocumentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSelectedBinder((BinderInformation)DocumentList.SelectedItem);
        }

        /// <summary>
        /// The user wants to modify a binders - same as selection changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyBinder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var button = (Button)sender;
                if (button.DataContext is BinderInformation)
                {                    
                    LoadSelectedBinder((BinderInformation)button.DataContext);
                }
            }
        }

        /// <summary>
        /// Process the selection changed
        /// </summary>
        /// <param name="binder"></param>
        private void LoadSelectedBinder(BinderInformation binder) {
           
            var newCriteria = BinderHelpers.GetBinderSearchCriteria(GlobalValues.LastConnection, binder.BinderID);

            // convert the base to the gui model
            var newGui = newCriteria.Select(r => new SearchCriteriaGUI
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

            // update the display...
            SearchCriteriaInfo = new ObservableCollection<SearchCriteriaGUI>(newGui);
            UserSearchInput.ItemsSource = SearchCriteriaInfo;

            // switch the tab
            MainTabs.SelectedIndex = 1;
        }

        #endregion

        #endregion

        #region Search Criteria

        #region Properties

        /// <summary>
        /// Search criteria 
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

        /// <summary>
        /// Execute the search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteSearch_Click(object sender, RoutedEventArgs e)
        {
            BinderInformation binder = null;
            if (DocumentList.SelectedIndex >= 0)
            {
                binder = (BinderInformation)DocumentList.SelectedItem;
            }
            GoToWebSiteSearch(binder, false);
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

        #endregion

        #region Results

        #region Properties
        
        /// <summary>
        /// Results of the last executed search
        /// </summary>
        public ObservableCollection<SearchResults> SearchResultsInfo { get; set; } = new ObservableCollection<SearchResults>();

        #endregion
             
        #region Routed Event Handlers

        ////////////////// Create RoutedEvent
        ////////////////// This creates a static property on the UserControl, ViewFileEvent, which 
        ////////////////// will be used by the Window, or any control up the Visual Tree, that wants to 
        ////////////////// handle the event. 
        ////////////////public static readonly RoutedEvent ViewFileEvent =
        ////////////////    EventManager.RegisterRoutedEvent("ViewFileEvent", RoutingStrategy.Bubble,
        ////////////////    typeof(RoutedEventHandler), typeof(MainSearch));

        ////////////////// Create RoutedEventHandler
        ////////////////// This adds the Custom Routed Event to the WPF Event System and allows it to be 
        ////////////////// accessed as a property from within xaml if you so desire
        ////////////////public event RoutedEventHandler ViewFile
        ////////////////{
        ////////////////    add { AddHandler(ViewFileEvent, value); }
        ////////////////    remove { RemoveHandler(ViewFileEvent, value); }
        ////////////////}


        ////////////////// Create RoutedEvent
        ////////////////// This creates a static property on the UserControl, CheckOutFileEvent, which 
        ////////////////// will be used by the Window, or any control up the Visual Tree, that wants to 
        ////////////////// handle the event. 
        ////////////////public static readonly RoutedEvent CheckOutFileEvent =
        ////////////////    EventManager.RegisterRoutedEvent("CheckOutFileEvent", RoutingStrategy.Bubble,
        ////////////////    typeof(RoutedEventHandler), typeof(MainSearch));

        ////////////////// Create RoutedEventHandler
        ////////////////// This adds the Custom Routed Event to the WPF Event System and allows it to be 
        ////////////////// accessed as a property from within xaml if you so desire
        ////////////////public event RoutedEventHandler CheckOutFile
        ////////////////{
        ////////////////    add { AddHandler(CheckOutFileEvent, value); }
        ////////////////    remove { RemoveHandler(CheckOutFileEvent, value); }
        ////////////////}
        ///
        #endregion

        #region Search

        /// <summary>
        /// Execute the search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            BinderInformation binder = null;
            if (DocumentList.SelectedIndex >= 0)
            {
                binder = (BinderInformation)DocumentList.SelectedItem;
            }
            GoToWebSiteSearch(binder, false);
        }

        private void ExecuteSave_Click(object sender, RoutedEventArgs e)
        {
            // get a name for the search
            Dialogs.UserInput userInput = new Dialogs.UserInput(Resource.SearchSaveName, "");
            if (userInput.ShowDialog() == true)
            {
                BinderInformation binder = new BinderInformation { BinderID = string.Empty, BinderName = userInput.Answer };
                GoToWebSiteSearch(binder, true);
            }
        }

        private void GoToWebSiteSearch(BinderInformation binder, bool keepResults)
        {
            var request = new RequestSearch
            {
                Connection = GlobalValues.LastConnection,
                ConnectionToken = GlobalValues.ConnectionToken,
                BinderName = (binder != null) ? binder.BinderName : string.Empty,
                BinderID = (binder != null) ? binder.BinderID : string.Empty,
                StartingEntry = 0,
                UpdateBinder = keepResults,
                Search = SearchCriteriaInfo.Select(r => new SearchCriteriaBase
                {
                    Criteria = r.Criteria,
                    Field = r.Field,
                    Relationship = r.Relationship,
                    ValueBool = r.ValueBool,
                    ValueMax = r.ValueMax,
                    ValueMaxDate = r.ValueMaxDate,
                    ValueMin = r.ValueMin,
                    ValueMinDate = r.ValueMinDate
                }).ToList()
            };

            var criteria = SearchHelpers.ProcessSearch(request);
            SearchResultsInfo = new ObservableCollection<SearchResults>(criteria);
            DisplayResults.ItemsSource = SearchResultsInfo;
            MainTabs.SelectedIndex = 2;
        }
                       
        #endregion

        //#region Results Show
        //private void HideDetails_Click(object sender, RoutedEventArgs e)
        //{
        //    var button = (Button)sender;
        //    if (button.DataContext is SearchResults)
        //    {
        //        var data = (SearchResults)button.DataContext;
        //        data.IsDetailsShown = true;
        //    }
        //}

        //private void ShowDetails_Click(object sender, RoutedEventArgs e)
        //{
        //    var button = (Button)sender;
        //    if (button.DataContext is SearchResults)
        //    {
        //        var data = (SearchResults)button.DataContext;
        //        data.IsDetailsShown = false;
        //    }
        //}
        //#endregion

        ////////#region Get Binder details

        /////////// <summary>
        /////////// event for when loading a default binder
        /////////// </summary>
        /////////// <param name="sender"></param>
        /////////// <param name="e"></param>
        ////////private void cbBinderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        ////////{
        ////////    if (sender is ComboBox)
        ////////    {
        ////////        var cb = (ComboBox)sender;
        ////////        if (cb.SelectedItem != null && cb.SelectedItem is BinderInformation)
        ////////        {
        ////////            var data = (BinderInformation)cb.SelectedItem;

        ////////            var newCriteria = BinderHelpers.GetBinderSearchCriteria(GlobalValues.LastConnection, data.BinderID);

        ////////            // convert the base to the gui model
        ////////            var newGui = newCriteria.Select(r => new SearchCriteriaGUI
        ////////            {
        ////////                Field = r.Field,
        ////////                Criteria = r.Criteria,
        ////////                ValueMin = r.ValueMin,
        ////////                ValueMax = r.ValueMax,
        ////////                ValueBool = r.ValueBool,
        ////////                ValueMinDate = r.ValueMinDate,
        ////////                ValueMaxDate = r.ValueMaxDate
        ////////            });

        ////////            // update the display...
        ////////            SearchCriteriaInfo = new ObservableCollection<SearchCriteriaGUI>(newGui);
        ////////            UserSearchInput.ItemsSource = SearchCriteriaInfo;
        ////////            //if (newGui.Any())
        ////////            //{
        ////////            //    Relationship = newGui.First().Relationship;
        ////////            //}
        ////////            //else
        ////////            //{
        ////////            //    Relationship = "&";
        ////////            //}
        ////////        }
        ////////    }
        ////////}

        ////////#endregion

        #region View File

        private void View_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var button = (Button)sender;
                if (button.DataContext != null && button.DataContext is SearchResults)
                {
                    var searchResult = (SearchResults)button.DataContext;

                    var file = new LocalFileStatus
                    {
                        //VaultID = searchResult.
                        DocumentName = searchResult.DocumentName,
                        IsCheckedOut = false,
                        DateRecieved = DateTime.Now
                    };

                    // ask the user where the file is to be placed
                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.OverwritePrompt = true;
                    saveFileDialog.CheckPathExists = true;
                    saveFileDialog.FileName = searchResult.DocumentName;
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        file.LocalFileLocation = saveFileDialog.FileName;
                        var localFiles = new Common.LocalFiles();
                        localFiles.ViewFile(file);
                        //RaiseEvent(new RoutedEventArgs(ViewFileEvent));
                    }

                }
            }

        }

        #endregion

        #region Checkout file
        private void CheckOut_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var button = (Button)sender;
                if (button.DataContext != null && button.DataContext is SearchResults)
                {
                    var searchResult = (SearchResults)button.DataContext;

                    var file = new LocalFileStatus
                    {
                        //VaultID = searchResult.
                        DocumentName = searchResult.DocumentName,
                        IsCheckedOut = true,
                        DateRecieved = DateTime.Now
                    };

                    // ask the user where the file is to be placed
                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.OverwritePrompt = true;
                    saveFileDialog.CheckPathExists = true;
                    saveFileDialog.FileName = searchResult.DocumentName;
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        file.LocalFileLocation = saveFileDialog.FileName;
                        var localFiles = new Common.LocalFiles();
                        localFiles.CheckOutFile(file);
                        //RaiseEvent(new RoutedEventArgs(CheckOutFileEvent));
                    }
                }
            }

        }

        #endregion
        
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

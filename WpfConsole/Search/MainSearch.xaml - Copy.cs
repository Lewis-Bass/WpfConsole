using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using WindowsData;
using Common;
using Microsoft.Win32;
//using System.Runtime.InteropServices.WindowsRuntime;
using Common.ServerCommunication.Helpers;
using Common.ServerCommunication.Requests;
using System.ComponentModel;
using WpfConsole.Resources;

namespace WpfConsole.Search
{
    /// <summary>
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class MainSearch : UserControl
    {
        #region Globals and constructor

        /// <summary>
        /// List of binders that are available from the ark
        /// </summary>
        public ObservableCollection<BinderInformation> BinderList { get; set; }

        /// <summary>
        /// Search critera 
        /// </summary>
        public ObservableCollection<SearchCriteriaGUI> SearchCriteriaInfo
        {
            get
            {
                return _SearchCriteriaInfo;

            }
            set {
                _SearchCriteriaInfo = value;
                OnPropertyChanged("SearchCriteriaInfo");
            }
        }

        ObservableCollection<SearchCriteriaGUI> _SearchCriteriaInfo = new ObservableCollection<SearchCriteriaGUI>();

        /// <summary>
        /// Results of the last executed search
        /// </summary>
        public ObservableCollection<SearchResults> SearchResultsInfo { get; set; } = new ObservableCollection<SearchResults>();

        /// <summary>
        /// Relationship Fields to display
        /// </summary>
        public List<RelationshipCriteria> RelationshipFields
        {
            get
            {
                return new List<RelationshipCriteria>
                {
                    new RelationshipCriteria
                    {
                        Key = "&",
                        Value ="/Resources/VenAnd100-71.png",
                        ToolTip = Resource.RelationshipAndHelp
                    },
                    new RelationshipCriteria
                    {
                        Key = "^",
                        Value ="/Resources/VenOr100-71.png",
                        ToolTip = Resource.RelationshipOrHelp
                    }
                };
            }
        }

        private string _relationship = "&";
        /// <summary>
        /// How is the relationship to be established 
        /// & = and
        /// ^ = or
        /// ! = not
        /// </summary>
        /// TODO - Switch to an enum?
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

        public MainSearch()
        {
            InitializeComponent();

            DataContext = this;

            LoadCriteria();
            LoadBinderList();
            ////Relationship = "&";
        }
        #endregion

        #region initial load

        private void LoadCriteria()
        {
            SearchCriteriaInfo = new ObservableCollection<SearchCriteriaGUI> {
                new SearchCriteriaGUI()
            };

        }

        private void LoadBinderList()
        {
            var request = new RequestBinderList
            {
                Connection = GlobalValues.LastConnection,
                ConnectionToken = GlobalValues.ConnectionToken,
                MaxEntries = 1000
            };
            var values = BinderHelpers.GetBinderList(request);
            if (values.Any())
            {
                values.Add(new BinderInformation());
                BinderList = new ObservableCollection<BinderInformation>(values.OrderBy(r => r.BinderName));
            }
        }
        #endregion

        #region Routed Event Handlers

        // Create RoutedEvent
        // This creates a static property on the UserControl, ViewFileEvent, which 
        // will be used by the Window, or any control up the Visual Tree, that wants to 
        // handle the event. 
        public static readonly RoutedEvent ViewFileEvent =
            EventManager.RegisterRoutedEvent("ViewFileEvent", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(MainSearch));

        // Create RoutedEventHandler
        // This adds the Custom Routed Event to the WPF Event System and allows it to be 
        // accessed as a property from within xaml if you so desire
        public event RoutedEventHandler ViewFile
        {
            add { AddHandler(ViewFileEvent, value); }
            remove { RemoveHandler(ViewFileEvent, value); }
        }


        // Create RoutedEvent
        // This creates a static property on the UserControl, CheckOutFileEvent, which 
        // will be used by the Window, or any control up the Visual Tree, that wants to 
        // handle the event. 
        public static readonly RoutedEvent CheckOutFileEvent =
            EventManager.RegisterRoutedEvent("CheckOutFileEvent", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(MainSearch));

        // Create RoutedEventHandler
        // This adds the Custom Routed Event to the WPF Event System and allows it to be 
        // accessed as a property from within xaml if you so desire
        public event RoutedEventHandler CheckOutFile
        {
            add { AddHandler(CheckOutFileEvent, value); }
            remove { RemoveHandler(CheckOutFileEvent, value); }
        }


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
            if (cbBinderList.SelectedItem != null && cbBinderList.SelectedItem is BinderInformation)
            {
                binder = (BinderInformation)cbBinderList.SelectedItem;
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
                    //Relationship = r.Relationship,
                    Relationship = Relationship,
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
        }

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
        }

        private void AddCriteria_Click(object sender, RoutedEventArgs e)
        {
            var criteria = new SearchCriteriaGUI();
            SearchCriteriaInfo.Add(criteria);
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

        #region Get Binder details

        /// <summary>
        /// event for when loading a default binder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbBinderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                var cb = (ComboBox)sender;
                if (cb.SelectedItem != null && cb.SelectedItem is BinderInformation)
                {
                    var data = (BinderInformation)cb.SelectedItem;

                    var newCriteria = BinderHelpers.GetBinderSearchCriteria(GlobalValues.LastConnection, data.BinderID);

                    // convert the base to the gui model
                    var newGui = newCriteria.Select(r => new SearchCriteriaGUI
                    {
                        Field = r.Field,
                        Criteria = r.Criteria,
                        ValueMin = r.ValueMin,
                        ValueMax = r.ValueMax,
                        ValueBool = r.ValueBool,
                        ValueMinDate = r.ValueMinDate,
                        ValueMaxDate = r.ValueMaxDate
                    });

                    // update the display...
                    SearchCriteriaInfo = new ObservableCollection<SearchCriteriaGUI>(newGui);
                    UserSearchInput.ItemsSource = SearchCriteriaInfo;
                    if (newGui.Any())
                    {
                        Relationship = newGui.First().Relationship;
                    }
                    else
                    {
                        Relationship = "&";
                    }
                }
            }
        }

        #endregion

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
                        RaiseEvent(new RoutedEventArgs(ViewFileEvent));
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
                        RaiseEvent(new RoutedEventArgs(CheckOutFileEvent));
                    }
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

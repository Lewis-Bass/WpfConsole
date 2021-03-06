using Common.ServerCommunication.Requests;
using DialogLibrary.SystemDialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WindowsData;

namespace WpfConsole.TagManagement
{
    /// <summary>
    /// Interaction logic for TagMain.xaml
    /// </summary>
    public partial class TagMain : UserControl, INotifyPropertyChanged
    {
        #region Properties

        ObservableCollection<MetaTags> _TagList = new ObservableCollection<MetaTags>();
        /// <summary>
        /// Collection of binders to show on the screen
        /// </summary>
        public ObservableCollection<MetaTags> TagList
        {
            get { return _TagList; }
            set
            {
                if (_TagList != value)
                {
                    _TagList = value;
                    OnPropertyChanged("FileList");
                }
            }
        }

        public UserControl DisplayResult { get; set; }

        #endregion

        #region Constructor
        public TagMain()
        {
            InitializeComponent();
            DataContext = this;

            LoadTagList();
        }

        #endregion

        #region helpers

        /// <summary>
        /// Load up the tags
        /// </summary>
        private void LoadTagList()
        {
            var request = new RequestFileTag();
            // wire up the communication

            var dummy = new List<MetaTags>
            {
                new MetaTags{ TagId = "1", TagName = "one", TotalTagUsage = 1 },
                new MetaTags{ TagId = "2", TagName = "Two", TotalTagUsage = 2 },
                new MetaTags{ TagId = "3", TagName = "Three", TotalTagUsage = 3 },
                new MetaTags{ TagId = "4", TagName = "Four", TotalTagUsage = 4 },
                new MetaTags{  TagId = "5", TagName = "Five", TotalTagUsage = 5 },
                new MetaTags{  TagId = "6", TagName = "Six", TotalTagUsage = 6 },
                new MetaTags{  TagId = "7", TagName = "Seven", TotalTagUsage = 7 },
                new MetaTags{  TagId = "8", TagName = "Eight", TotalTagUsage = 8 },
                new MetaTags{  TagId = "9", TagName = "Nine", TotalTagUsage = 9 },
                new MetaTags{  TagId = "10", TagName = "Ten", TotalTagUsage = 10 },
            };
            TagList = new ObservableCollection<MetaTags>(dummy.OrderBy(r => r.TagName));
        }


        #endregion

        #region Button Click

        /// <summary>
        /// Add a new tag record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string newTagName = string.Empty;
            var dlg = new UserInput(WpfConsole.Resources.Resource.TagAddNew);
            dlg.Owner = Application.Current.MainWindow;
            if (dlg.ShowDialog() == true)
            {
                newTagName = dlg.Answer;
            }
            if (!string.IsNullOrWhiteSpace(newTagName))
            {
                // check for duplicate
                if (_TagList.Any(r => string.Compare(r.TagName, newTagName, StringComparison.InvariantCultureIgnoreCase) == 0))
                {
                    Serilog.Log.Logger.Warning($"Attempted to add duplicate tag {newTagName}");
                    MessageBox.Show(Application.Current.MainWindow, string.Format(WpfConsole.Resources.Resource.TagAttemptDuplicateAdd, newTagName), WpfConsole.Resources.Resource.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // add a new record
                // TODO: Send it to the back end and get the tagid                 
                _TagList.Add(new MetaTags { TagName = newTagName, TagId = newTagName, TotalTagUsage = 0 });
            }
        }

        /// <summary>
        /// Rename the existing connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            string newTagName = string.Empty;
            var dlg = new UserInput(WpfConsole.Resources.Resource.TagRenameString);
            dlg.Owner = Application.Current.MainWindow;
            if (dlg.ShowDialog() == true)
            {
                newTagName = dlg.Answer;
            }
            if (!string.IsNullOrWhiteSpace(newTagName))
            {
                // check for duplicate
                if (_TagList.Any(r => string.Compare(r.TagName, newTagName, StringComparison.InvariantCultureIgnoreCase) == 0))
                {
                    Serilog.Log.Logger.Warning($"Attempted to add duplicate tag {newTagName}");
                    MessageBox.Show(Application.Current.MainWindow, string.Format(WpfConsole.Resources.Resource.TagAttemptDuplicateAdd, newTagName), WpfConsole.Resources.Resource.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var btn = (Button)sender;
                var tag = (MetaTags)btn.DataContext;
                // TODO: Send it to the back end to change the name
                tag.TagName = newTagName;
                OnPropertyChanged("FileList");
            }
        }

        /// <summary>
        /// Delete the tag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var tag = (MetaTags)btn.DataContext;
            var result = MessageBox.Show(Application.Current.MainWindow, string.Format(WpfConsole.Resources.Resource.TagDeleteRecord, tag.TagName),
                WpfConsole.Resources.Resource.Delete, MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                //TODO: Send delete to the back end
                TagList.Remove(tag);
            }
        }

        /// <summary>
        /// Combine the tag with another
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCombine_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var tag = (MetaTags)btn.DataContext;
            if (tag == null || string.IsNullOrWhiteSpace(tag.TagName))
            {
                Serilog.Log.Logger.Error("Can not combine an empty tag name");
                MessageBox.Show(Application.Current.MainWindow, WpfConsole.Resources.Resource.ErrorEmptyTag, WpfConsole.Resources.Resource.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }
            var sel = _TagList.Where(r => r.TagName != tag.TagName).OrderBy(r => r.TagName).Select(r => new DropDownRecords { Key = r.TagId, Value = r.TagName }).ToList();
            var dlg = new UserDropdownSelect(sel, string.Format(WpfConsole.Resources.Resource.TagCombineString, tag.TagName));
            dlg.Owner = Application.Current.MainWindow;
            if (dlg.ShowDialog() == true)
            {
                if (dlg.Answer != null)
                {
                    // TODO: Send information to the back end for processing
                    _TagList.Remove(tag);
                }
            }
        }

        /// <summary>
        /// Search Button - display the search screen and its results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var tag = (MetaTags)btn.DataContext;
            if (tag == null || string.IsNullOrWhiteSpace(tag.TagName))
            {
                Serilog.Log.Logger.Error("Can not combine an empty tag name");
                MessageBox.Show(Application.Current.MainWindow, WpfConsole.Resources.Resource.ErrorEmptyTag, WpfConsole.Resources.Resource.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }

            // convert the base to the gui model
            var newGui = new List<SearchCriteriaGUI> {
                new SearchCriteriaGUI {
                    Field = "Tag",
                    Criteria = "Matches",
                    Relationship = "&",
                    ValueMin = tag.TagName
                }
            };
            newGui[0].ShowRelationship = false;
            newGui[0].ShowGroupRelationship = false;

            // pass the query to the main screen - this is needed because the search screen may not be visible
            RaiseEvent(new RoutedEventArgs(MainWindow.NewSearchEvent, newGui));

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

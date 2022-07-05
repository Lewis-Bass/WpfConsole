using Common;
using Common.ServerCommunication.Helpers;
using Common.ServerCommunication.Requests;
using Common.Settings;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Themes.Helpers;
using WindowsData;
//using WpfConsole.AccountChange;
using WpfConsole.CheckedOutFiles;
using WpfConsole.Connection;
using WpfConsole.FileDrop;
////using WpfConsole.KeyManagement;
using WpfCommon.Preference;
//using WpfConsole.Search;
using WpfConsole.SearchMaster;
//using WpfConsole.SearchFilter;
using WpfConsole.Resources;
//using WpfConsole.AutoLoad;
using static Themes.Enumerations.ThemeEnums;
using System.IO;
using Common.Licenses;
using System.Linq;
//using DialogLibrary.SystemDialogs;
using System.Windows.Interop;
using System.Windows.Markup;
using WpfConsole.TagManagement;
using System.Collections.Generic;
using static Common.Licenses.LicenseChecks;
//using WpfConsole.ExportVault;
using WpfConsole.Properties;
using Common.ConnectionInfo;

namespace WpfConsole
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region globals

        UserControl _SearchMaster;
        UserControl _Connections;
        UserControl _Statistics;
        UserControl _FileDisplay;
        //UserControl _AutoLoad;
        UserControl _PreferenceSetup;
        //UserControl _KeyManagement;
        UserControl _TagManagement;
        UserControl _CheckedOut;
        ///UserControl _MyPassword;
        //UserControl _ExportMain;

        /// <summary>
        /// Prior Connections - left nav item
        /// </summary>
        public ObservableCollection<ConnectionInformation> PriorConnections { get; set; } = new ObservableCollection<ConnectionInformation>();

        /// <summary>
        /// Last (x) Prior Viewed Files  - left nav item
        /// </summary>
        public LocalFileList PriorFiles { get; set; } = new LocalFileList();

        /// <summary>
        /// Last (x) Checked out files - left nav
        /// </summary>
        public LocalFileList CheckFiles { get; set; } = new LocalFileList();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SerilogSetup();
            EventSetup();
            ThemeSetup();

            // do we have any problems associated with running the back end to make the user aware of ?
            // TODO: Check disk space

            LoadConnections();
            LoadPriorFiles();
            LoadCheckedFiles();

            SetTopMenuAvailability();
            Statistics_Click(null, null); // set the first displayed screen
        }

        /// <summary>
        /// OnLoad event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLoad(object sender, RoutedEventArgs e)
        {
            // reconnect to last session if possible
            ReconnectIfPossible();
        }

        /// <summary>
        /// License Issues
        /// </summary>
        private string LicenseSetup()
        {
            string message = string.Empty;

            // do we have a license file?
            var chk = new LicenseChecks();
            var licenseStatus = chk.GetLicenseStatus();
            if (licenseStatus == LicenseChecks.LicenseStatus.Missing)
            {
                chk.CreateDemoLicense();
            }
            else if (licenseStatus == LicenseChecks.LicenseStatus.Expired)
            {
                // local admin can always connect
                var settings = LocalSettings.Load();
                if (settings.LastConnection.IsLocalAdmin)
                {
                    message = Resource.AdminExpiredLicense;
                }
                else
                {
                    message = Resource.ExpiredLicense;
                    GlobalValues.LastConnection = null;
                }
            }
            else if (licenseStatus == LicenseChecks.LicenseStatus.Demo)
            {
                var expireDate = chk.GetExpirationDate();
                int daysRemain = (int)expireDate.Subtract(DateTime.Now).TotalDays;
                // Demo License - display the warning
                message = string.Format(Resource.DemoLicense, daysRemain);
            }
            return message;
        }

        /// <summary>
        /// Events supported
        /// </summary>
        private void EventSetup()
        {
            // setup handlers and events
            AddHandler(MainSearch.ViewFileEvent,
                new RoutedEventHandler(MainSearch_ViewFileEventMethod));

            AddHandler(MainSearch.CheckOutFileEvent,
                new RoutedEventHandler(MainSearch_CheckOutFileEventMethod));

            AddHandler(CheckedOutList.CheckInFileEvent,
                new RoutedEventHandler(MainSearch_CheckInFileEventMethod));

            AddHandler(MainConnection.ConnectionChangedEvent,
                new RoutedEventHandler(MainSearch_ConnectionChangedMethod));

            AddHandler(MainWindow.NewSearchEvent,
               new RoutedEventHandler(MainWindow_HandleNewSearchMethod));

        }

        /// <summary>
        /// which theme is being used?
        /// </summary>
        private static void ThemeSetup()
        {
            // setup themes
            LocalSettings settings = LocalSettings.Load();
            ThemeSelector.ApplyTheme(
                new Uri(ThemeSelector.ThemeEnumToURIString((ETheme)settings.ActiveTheme),
                UriKind.Relative));
        }

        /// <summary>
        /// Setup the logging framework
        /// </summary>
        private static void SerilogSetup()
        {
            // setup serilog logging
            if (!Directory.Exists(GlobalValues.LoggingDirectory))
            {
                Directory.CreateDirectory(GlobalValues.LoggingDirectory);
            }
            string logTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u}] [{SourceContext}] {Message}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: logTemplate)
#else
                .MinimumLevel.Information()
#endif
                .WriteTo.File(GlobalValues.LoggingDirectory, rollingInterval: RollingInterval.Hour, outputTemplate: logTemplate)
                .CreateLogger();
            Log.Logger.Error($"Starting Application {DateTime.Now.ToLongTimeString()}");
        }

        /// <summary>
        /// When the screen initially loads, reconnect to the ark that was last used
        /// </summary>
        private void ReconnectIfPossible()
        {
            LocalSettings settings = LocalSettings.Load();
            if (settings.LastConnection != null)
            {
                ProcessLogin(settings.LastConnection);
                Statistics_Click(null, null);
            }
        }

        #endregion

        #region Global Routed events

        // Create RoutedEvent
        // This creates a static property on the UserControl, ViewFileEvent, which 
        // will be used by the Window, or any control up the Visual Tree, that wants to 
        // handle the event. 
        public static readonly RoutedEvent NewSearchEvent =
            EventManager.RegisterRoutedEvent("NewSearch", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(MainSearch));

        // Create RoutedEventHandler
        // This adds the Custom Routed Event to the WPF Event System and allows it to be 
        // accessed as a property from within xaml if you so desire
        public event RoutedEventHandler NewSearch
        {
            add { AddHandler(NewSearchEvent, value); }
            remove { RemoveHandler(NewSearchEvent, value); }
        }

        #endregion

        #region Routed Event Handlers

        /// <summary>
        /// Handle the event from the MainSearch user control - a new file was viewed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainSearch_ViewFileEventMethod(object sender, RoutedEventArgs e)
        {
            // the list of file views has changed - update the display
            LoadPriorFiles();
        }

        /// <summary>
        /// Handle the event from the MainSearch user control - a new file was checkedout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainSearch_CheckOutFileEventMethod(object sender, RoutedEventArgs e)
        {
            // the list of file views has changed - update the display
            LoadCheckedFiles();
        }

        /// <summary>
        /// handle the event form the CheckedOutFiles user control - a file was just checkedin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainSearch_CheckInFileEventMethod(object sender, RoutedEventArgs e)
        {
            // the list of file views has changed - update the display
            LoadCheckedFiles();
        }

        /// <summary>
        /// Handle the event from the MainConnection user control - a new connection was just established
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainSearch_ConnectionChangedMethod(object sender, RoutedEventArgs e)
        {
            var message = ProcessConnectionChanged();
        }

        /// <summary>
        /// The current connection to the vault has changed and needs to be handled
        /// </summary>
        /// <returns></returns>
        private string ProcessConnectionChanged()
        {
            string message = LicenseSetup();
            LoadConnections();
            // make certain that the last connection still has a valid value (it might have been deleted by MainConnection.xaml)
            if (!PriorConnections.Any(r => r.AccessKeyName == GlobalValues.LastConnection?.AccessKeyName))
            {
                GlobalValues.LastConnection = null;
            }
            SetTopMenuAvailability();
            return message;
        }

        /// <summary>
        /// Handle the event from the MainConnection user control - a new search has been initiated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_HandleNewSearchMethod(object sender, RoutedEventArgs e)
        {
            // display the search screen
            SearchMaster_Click(sender, null);
            var criteria = ((List<SearchCriteriaGUI>)e.OriginalSource)
                .Select(r => new SearchCriteriaBase
                {
                    Criteria = r.Criteria,
                    Field = r.Field,
                    GroupID = r.GroupID,
                    GroupRelationship = r.GroupRelationship,
                    Relationship = r.Relationship,
                    ValueBool = r.ValueBool,
                    ValueMax = r.ValueMax,
                    ValueMin = r.ValueMin,
                    ValueMaxDate = r.ValueMaxDate,
                    ValueMinDate = r.ValueMinDate,
                });
            // perform the search - tried to use events but couldn't get them to fire
            ((MainSearch)_SearchMaster).InitialSearch(criteria.ToList());
        }

        #endregion

        #region Menu Click Events

        /// <summary>
        /// Search menu clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SearchMaster_Click(object sender, RoutedEventArgs e)
        {

            if (_SearchMaster == null)
            {
                _SearchMaster = new SearchMaster.MainSearch();
            }
            DisplayControl(_SearchMaster);
        }

        //////private void CreateArk_Click(object sender, RoutedEventArgs e)
        //////{
        //////    if (_Create == null)
        //////    {
        //////        _Create = new Create.MainCreate();
        //////    }
        //////    DisplayControl(_Create);
        //////}

        /// <summary>
        /// Connections menu clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connections_Click(object sender, RoutedEventArgs e)
        {
            if (_Connections == null)
            {
                _Connections = new Connection.MainConnection();
            }
            DisplayControl(_Connections);
        }

        /// <summary>
        /// Stats menu clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            if (!AdminMenuCheck() && !UserMenuCheck())
            {
                return;
            }
            if (_Statistics == null)
            {
                _Statistics = new Statistics.StatisticsMain();
            }
            DisplayControl(_Statistics);
        }

        /// <summary>
        /// File Display menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileDisplay_Click(object sender, RoutedEventArgs e)
        {
            if (_FileDisplay == null)
            {
                _FileDisplay = new FileDrop.FileDisplay();
            }
            DisplayControl(_FileDisplay);
        }

        /// <summary>
        /// Preferences menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Preferences_Click(object sender, RoutedEventArgs e)
        {
            if (_PreferenceSetup == null)
            {
                _PreferenceSetup = new PreferenceSetup();
            }
            DisplayControl(_PreferenceSetup);
        }

        /// <summary>
        /// Key menu clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void KeyManagement_Click(object sender, RoutedEventArgs e)
        //{
        //    if (_KeyManagement == null)
        //    {
        //        _KeyManagement = new KeysMain();
        //    }
        //    DisplayControl(_KeyManagement);
        //}

        /// <summary>
        /// Tag menu clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TagManagement_Click(object sender, RoutedEventArgs e)
        {
            // the search master must be instantiated because TagManagement
            if (_SearchMaster == null)
            {
                _SearchMaster = new SearchMaster.MainSearch();
            }
            if (_TagManagement == null)
            {
                _TagManagement = new TagMain();
            }
            DisplayControl(_TagManagement);
        }


        /// <summary>
        /// Check out menu clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckedOut_Click(object sender, RoutedEventArgs e)
        {
            if (_CheckedOut == null)
            {
                _CheckedOut = new CheckedOutList();
            }
            DisplayControl(_CheckedOut);
        }

        ///// <summary>
        ///// Top Menu Password option
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Password_Click(object sender, RoutedEventArgs e)
        //{
        //    if (_MyPassword == null)
        //    {
        //        _MyPassword = new MyPasswordChange();
        //    }
        //    DisplayControl(_MyPassword);
        //}

        ///// <summary>
        ///// Top Menu Auto Load Preferences
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void AutoLoad_Click(object sender, RoutedEventArgs e)
        //{
        //    if (_AutoLoad == null)
        //    {
        //        _AutoLoad = new AutoLoadMain();
        //    }
        //    DisplayControl(_AutoLoad);
        //}

        ///////// <summary>
        ///////// Top Menu Export option
        ///////// </summary>
        ///////// <param name="sender"></param>
        ///////// <param name="e"></param>
        //////private void ExportVault_Click(object sender, RoutedEventArgs e)
        //////{
        //////    // TODO: Wire up the form
        //////    if (_ExportMain == null)
        //////    {
        //////        _ExportMain = new ExportMain();
        //////    }
        //////    DisplayControl(_ExportMain);
        //////}

        /// <summary>
        /// logout of the current session
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var request = new BaseRequest { };
            var response = AccountHelpers.LogoutDisconnect(request);

            // clear the values
            GlobalValues.ConnectionToken = null;
            GlobalValues.LastConnection = null;

            // clear the stored list
            LoadConnections();
            SetTopMenuAvailability();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// change the user control that is displayed
        /// </summary>
        /// <param name="displayControl"></param>
        private void DisplayControl(UserControl displayControl)
        {
            for (int i = (MenuSelectionDisplay.Children.Count - 1); i >= 0; i--)
            {
                MenuSelectionDisplay.Children.RemoveAt(i);
            }
            MenuSelectionDisplay.Children.Add(displayControl);
        }

        /// <summary>
        /// Load (X) files that have been viewed previous
        /// </summary>
        private void LoadPriorFiles()
        {
            LocalSettings settings = LocalSettings.Load();
            PriorFiles = new LocalFileList();
            var lf = new LocalFiles();
            int count = 0;
            foreach (var data in lf.LocalFileViews)
            {
                count++;
                if (count > settings.MaxFilesViewed)
                {
                    break;
                }
                PriorFiles.Files.Add(data);
            }

            lvPriorFiles.ItemsSource = PriorFiles.Files;
        }

        /// <summary>
        /// Load checked out files
        /// </summary>
        private void LoadCheckedFiles()
        {
            LocalSettings settings = LocalSettings.Load();
            CheckFiles = new LocalFileList();
            var lf = new LocalFiles();
            int count = 0;
            foreach (var data in lf.LocalFileCheckedOut)
            {
                count++;
                if (count > settings.MaxFilesViewed)
                {
                    break;
                }
                CheckFiles.Files.Add(data);
            }

            lvCheckedFiles.ItemsSource = CheckFiles.Files;
        }

        /// <summary>
        /// Load the connections from the settings file. If the local admin account is missing its added
        /// </summary>
        private void LoadConnections()
        {
            var conHelper = new ConnectionHelper();

            PriorConnections = new ObservableCollection<ConnectionInformation>(conHelper.GetAllConnections());
            foreach (var con in PriorConnections)
            {
                con.IsCurrentConnection = (GlobalValues.LastConnection != null) ? con.AccessKeyName == GlobalValues.LastConnection.AccessKeyName : false;
            }
            lvPriorConnections.ItemsSource = PriorConnections;
        }

        /// <summary>
        /// Setup the top nav visibility options
        /// </summary>
        private void SetTopMenuAvailability()
        {
            bool adminMenu = AdminMenuCheck();
            bool userMenu = UserMenuCheck();
            Statistics.IsEnabled = userMenu;
            SearchMaster.IsEnabled = userMenu;
            AddFiles.IsEnabled = userMenu;
            CheckedOutFiles.IsEnabled = userMenu;
            //Keys.IsEnabled = userMenu;
            Tags.IsEnabled = userMenu;
            Logout.IsEnabled = userMenu || adminMenu;
            //AutoLoad.IsEnabled = userMenu;
            //ExportVault.IsEnabled = (userMenu || adminMenu);
        }

        /// <summary>
        /// check to see if the user menu items should be shown
        /// </summary>
        /// <returns></returns>
        private static bool UserMenuCheck()
        {
            var chk = new LicenseChecks();
            var licenseStatus = chk.GetLicenseStatus();
            return GlobalValues.IsConnectionValid &&
                            (licenseStatus != LicenseChecks.LicenseStatus.Expired);
        }

        /// <summary>
        /// Check to see if the admin user items should be shown
        /// </summary>
        /// <returns></returns>
        private static bool AdminMenuCheck()
        {
            var chk = new LicenseChecks();
            var licenseStatus = chk.GetLicenseStatus();
            var settings = LocalSettings.Load();
            var adminMenu = GlobalValues.IsConnectionValid &&
                            (
                                (licenseStatus == LicenseChecks.LicenseStatus.Expired && settings.LastConnection.IsLocalAdmin)
                                || licenseStatus != LicenseChecks.LicenseStatus.Expired)
                            && (GlobalValues.LastConnection.IPAddress.ToLowerInvariant().Contains("localhost")
                                || GlobalValues.LastConnection.IPAddress.ToLowerInvariant().Contains("127.0.0.1")); ;
            return adminMenu;
        }

        /// <summary>
        /// Handle the login event
        /// </summary>
        /// <param name="data"></param>
        private bool ProcessLogin(ConnectionInformation data)
        {
            bool retval = false;
            var helper = new Helpers.LoginHelper();
            var loginStatus = helper.ProcessLogin(data);
            string message = string.Empty;
            if (loginStatus != null && loginStatus.Result == LoginResult.ResultList.Success)
            {
                message = ProcessConnectionChanged();
                retval = true;
            }
            else
            {
                Connections_Click(null, null);
            }

            //TODO: Write my own so that the messagebox will center over the application
            MessageBox.Show(Application.Current.MainWindow,
                $"{loginStatus.Message}. {message}",
                WpfConsole.Resources.Resource.LoginName,
                MessageBoxButton.OK,
                MessageBoxImage.Asterisk);
            return retval;
        }

        #endregion

        #region Drag Drop

        /// <summary>
        /// Entering the drop zone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragDropZone_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Dragging over the drop zone - currently not implemented
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragDropZone_DragOver(object sender, DragEventArgs e)
        {
        }

        /// <summary>
        /// Process the file that is being dragged over the drop zone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragDropZone_Drop(object sender, DragEventArgs e)
        {
            string[] inList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            _FileDisplay = new FileDisplay(inList);
            FileDisplay_Click(sender, e);
        }

        #endregion

        #region Left Menu Events

        /// <summary>
        /// Raised when a prior viewed file is selected - so we show the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton)
            {
                var rb = (RadioButton)sender;
                if (rb.DataContext is LocalFileStatus)
                {
                    var data = (LocalFileStatus)rb.DataContext;
                    var lf = new LocalFiles();
                    lf.ViewLocalFile(data);
                }
                rb.IsChecked = false;
            }
        }

        /// <summary>
        /// attempt to login to the selected connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginConnection_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton)
            {
                var btn = (RadioButton)sender;
                if (btn.DataContext is ConnectionInformation)
                {
                    var data = (ConnectionInformation)btn.DataContext;
                    if (!ProcessLogin(data))
                    {
                        //lvPriorConnections.SelectedItem = null;
                        //LoadConnections();
                        Logout_Click(sender, e);
                    }
                }
            }
        }

        #endregion
    }
}

using Common;
using Common.ServerCommunication.Helpers;
using Common.ServerCommunication.Requests;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Themes.Helpers;
using WindowsData;
using WpfConsole.AccountChange;
using WpfConsole.CheckedOutFiles;
using WpfConsole.Connection;
using WpfConsole.FileDrop;
using WpfConsole.KeyManagement;
using WpfConsole.Preference;
using WpfConsole.Search;
using WpfConsole.SearchFilter;
using WpfConsole.Resources;
using WpfConsole.AutoLoad;
using static Themes.Enumerations.ThemeEnums;
using System.IO;
using Common.Licenses;
using System.Linq;
using WpfConsole.Dialogs;
using System.Windows.Interop;
using System.Windows.Markup;

namespace WpfConsole
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region globals

        UserControl _Search;
        UserControl _SearchAdvanced;
        UserControl _SearchTree;
        UserControl _SearchFilter;
        UserControl _Connections;
        UserControl _FileDisplay;
        UserControl _AutoLoad;
        UserControl _PreferenceSetup;
        UserControl _KeyManagement;
        UserControl _CheckedOut;
        UserControl _MyPassword;

        public ObservableCollection<ConnectionInformation> PriorConnections { get; set; } = new ObservableCollection<ConnectionInformation>();
        public LocalFileList PriorFiles { get; set; } = new LocalFileList();
        public LocalFileList CheckFiles { get; set; } = new LocalFileList();

        #endregion

        #region constructor
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
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            // reconnect to last session if possible
            ReconnectIfPossible();
        }

        /// <summary>
        /// License Issues
        /// </summary>
        private void LicenseSetup()
        {
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
                if (!settings.LastConnection.IsLocalAdmin)
                {
                    MessageBox.Show(Resource.ExpiredLicense, Resource.Error, MessageBoxButton.OK, MessageBoxImage.Stop);
                    GlobalValues.LastConnection = null;
                }
            }
            else if (licenseStatus == LicenseChecks.LicenseStatus.Demo)
            {
                var expireDate = chk.GetExpirationDate();
                int daysRemain = (int)expireDate.Subtract(DateTime.Now).TotalDays;
                // Demo License - display the warning
                MessageBox.Show(string.Format(Resource.DemoLicense, daysRemain), Resource.Notice, MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: logTemplate)
                .WriteTo.File(GlobalValues.LoggingDirectory, rollingInterval: RollingInterval.Hour, outputTemplate: logTemplate)
                .CreateLogger();
            Log.Logger.Error($"Starting Application {DateTime.Now.ToLongTimeString()}");
        }

        private void ReconnectIfPossible()
        {
            LocalSettings settings = LocalSettings.Load();
            if (settings.LastConnection != null)
            {
                ProcessLogin(settings.LastConnection);
            }
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
            ProcessConnectionChanged();
        }

        private void ProcessConnectionChanged()
        {
            LicenseSetup();
            LoadConnections();
            // make certain that the last connection still has a valid value (it might have been deleted by MainConnection.xaml)
            if (!PriorConnections.Any(r => r.AccessKeyName == GlobalValues.LastConnection?.AccessKeyName))
            {
                GlobalValues.LastConnection = null;
            }
            SetTopMenuAvailability();
        }

        #endregion

        #region Menu Click Events

        private void Search_Click(object sender, RoutedEventArgs e)
        {

            if (_Search == null)
            {
                _Search = new Search.MainSearch();
            }
            DisplayControl(_Search);
        }

        private void Search_Filter(object sender, RoutedEventArgs e)
        {

            if (_SearchFilter == null)
            {
                _SearchFilter = new SearchFilter.FilterMain();
            }
            DisplayControl(_SearchFilter);
        }


        private void SearchAdvanced_Click(object sender, RoutedEventArgs e)
        {
            if (_SearchAdvanced == null)
            {
                _SearchAdvanced = new SearchAdvanced.AdvancedMain();
            }
            DisplayControl(_SearchAdvanced);
        }

        private void SearchTreeMain_Click(object sender, RoutedEventArgs e)
        {
            if (_SearchTree == null)
            {
                _SearchTree = new SearchTree.SearchTreeMain();
            }
            DisplayControl(_SearchTree);
        }

        //////private void CreateArk_Click(object sender, RoutedEventArgs e)
        //////{
        //////    if (_Create == null)
        //////    {
        //////        _Create = new Create.MainCreate();
        //////    }
        //////    DisplayControl(_Create);
        //////}

        private void Connections_Click(object sender, RoutedEventArgs e)
        {
            if (_Connections == null)
            {
                _Connections = new Connection.MainConnection();
            }
            DisplayControl(_Connections);
        }

        private void FileDisplay_Click(object sender, RoutedEventArgs e)
        {
            if (_FileDisplay == null)
            {
                _FileDisplay = new FileDrop.FileDisplay();
            }
            DisplayControl(_FileDisplay);
        }

        private void Preferences_Click(object sender, RoutedEventArgs e)
        {
            if (_PreferenceSetup == null)
            {
                _PreferenceSetup = new PreferenceSetup();
            }
            DisplayControl(_PreferenceSetup);
        }

        private void KeyManagement_Click(object sender, RoutedEventArgs e)
        {
            if (_KeyManagement == null)
            {
                _KeyManagement = new KeysMain();
            }
            DisplayControl(_KeyManagement);
        }


        private void CheckedOut_Click(object sender, RoutedEventArgs e)
        {
            if (_CheckedOut == null)
            {
                _CheckedOut = new CheckedOutList();
            }
            DisplayControl(_CheckedOut);
        }

        /// <summary>
        /// Top Menu Password option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Password_Click(object sender, RoutedEventArgs e)
        {
            if (_MyPassword == null)
            {
                _MyPassword = new MyPasswordChange();
            }
            DisplayControl(_MyPassword);
        }

        /// <summary>
        /// Top Menu Auto Load Preferences
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoLoad_Click(object sender, RoutedEventArgs e)
        {
            if (_AutoLoad == null)
            {
                _AutoLoad = new AutoLoadMain();
            }
            DisplayControl(_AutoLoad);

        }

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

        private void LoadConnections()
        {
            LocalSettings settings = LocalSettings.Load();

            // add the local host setting if it is not there
            if (settings.ConnectionsData.Any(r => r.AccessKeyName == ConnectionInformation.LocalAdminName) == false)
            {
                // this connection should always exist
                var conn = new ConnectionInformation { AccessKeyName = ConnectionInformation.LocalAdminName, IPAddress = "localhost", IsCurrentConnection = false };
                settings.AddConnection(conn);
                settings.LastConnection = conn;
            }

            PriorConnections = new ObservableCollection<ConnectionInformation>(settings.ConnectionsData);
            // setup the current connection 
            foreach (var con in settings.ConnectionsData)
            {
                con.IsCurrentConnection = (GlobalValues.LastConnection != null) ? con.AccessKeyName == GlobalValues.LastConnection.AccessKeyName : false;
            }
            lvPriorConnections.ItemsSource = PriorConnections;
        }

        private void SetTopMenuAvailability()
        {
            Search.IsEnabled = GlobalValues.IsConnectionValid;
            SearchTree.IsEnabled = GlobalValues.IsConnectionValid;
            SearchFilter.IsEnabled = GlobalValues.IsConnectionValid;
            AddFiles.IsEnabled = GlobalValues.IsConnectionValid;
            CheckedOutFiles.IsEnabled = GlobalValues.IsConnectionValid;
            Keys.IsEnabled = GlobalValues.IsConnectionValid;
            Logout.IsEnabled = GlobalValues.IsConnectionValid;
            AutoLoad.IsEnabled = GlobalValues.IsConnectionValid;
        }

        #endregion

        #region Drag Drop

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

        private void DragDropZone_DragOver(object sender, DragEventArgs e)
        {
        }

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
                    ProcessLogin(data);
                }
            }
        }

        private void ProcessLogin(ConnectionInformation data)
        {
            var helper = new Helpers.LoginHelper();
            helper.ProcessLogin(data);

            //LicenseSetup();
            //LoadConnections();
            //SetTopMenuAvailability();
            ProcessConnectionChanged();
        }

        #endregion

    }
}

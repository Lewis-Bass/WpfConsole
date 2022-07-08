using Common;
using Common.Licenses;
using Common.Settings;
using DialogLibrary.SystemDialogs;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Themes.Helpers;
using WindowsData;
using WpfAdmin.AutoLoad;
using WpfAdmin.ExportVault;
using WpfAdmin.KeyManagement;
using WpfAdmin.Resources;
using WpfCommon.Preference;
using WpfAdmin.Vault;
using static Themes.Enumerations.ThemeEnums;

namespace WpfAdmin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region globals

        // User screens
        UserControl _VaultSetup;
        UserControl _KeyManagement;
        UserControl _AutoLoad;
        UserControl _ExportMain;
        UserControl _PreferenceSetup;

        //UserControl _TagManagement;
        //UserControl _CheckedOut;
        //UserControl _MyPassword;



        /// <summary>
        /// The admin console only works with a localhost connection. set it up here
        /// </summary>
        public static ConnectionInformation LocalVaultConnection = new ConnectionInformation
        {
            AccessKeyName = "LocalHost",
            IPAddress = "127.0.0.1",
            IsCurrentConnection = true,
        };

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SerilogSetup();
            ThemeSetup();
            EventSetup();

            // do we have any problems associated with running the back end to make the user aware of ?
            // TODO: Check disk space
        }

        #endregion

        #region Events

        /// <summary>
        /// OnLoad event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLoad(object sender, RoutedEventArgs e)
        {

            if (!CheckPurchaseKey())
            {
                Close(); // fatal error - bail out
                return;
            }

            if (!CheckDeviceLicense())
            {
                Close();
                return;
            }

            if (!CheckVaultSetup())
            {
                // the vault location must be specified before anything can be done, turn off all of the options except the setup
                // and put a note on the screen
                SetupNavAvailability(false);
                VaultSetup_Click(null, null);
            }
        }

        /// <summary>
        /// Handle the event from the MainSearch user control - a new file was viewed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VaultSetup_VaultChanges(object sender, RoutedEventArgs e)
        {
            // the vault location must be specified before anything can be done, turn off all of the options except the setup
            // and put a note on the screen
            SetupNavAvailability(CheckVaultSetup());
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Does the installation have a customer purchase key?
        /// </summary>
        /// <returns></returns>
        private bool CheckPurchaseKey()
        {
            // do we have a purchase key?
            var settings = LocalSettings.Load();
            if (settings == null)
            {
                settings = new LocalSettings();
            }
            if (string.IsNullOrWhiteSpace(settings.CustomerKey))
            {
                bool ok = false;
                while (!ok)
                {
                    var dlg = new PurchaseKey(Resource.PurchaseKeyDescription);
                    if (Application.Current.MainWindow.IsLoaded)
                    {
                        dlg.Owner = Application.Current.MainWindow;
                    }
                    if (dlg.ShowDialog() == true)
                    {
                        if (dlg.IsRedirectAnswer)
                        {
                            System.Diagnostics.Process.Start(Resource.PurchaseKeyUrl);
                        }
                        else
                        {
                            settings.CustomerKey = dlg.Answer;
                        }
                    }

                    // TODO: Verify that the customer key is valid!
                    ok = !string.IsNullOrWhiteSpace(settings.CustomerKey);
                }
            }
            // it is possible to cancel the dialog message without the customer key
            return !string.IsNullOrWhiteSpace(settings.CustomerKey);
        }

        /// <summary>
        /// Does the device have a license?
        /// </summary>
        /// <returns></returns>
        private bool CheckDeviceLicense()
        {
            //// Does the local vault have a device license?
            var chk = new LicenseChecks();
            var licenseStatus = chk.GetLicenseStatus();
            string message = string.Empty;
            if (licenseStatus == LicenseChecks.LicenseStatus.Missing)
            {
                chk.CreateDemoLicense();
            }
            else if (licenseStatus == LicenseChecks.LicenseStatus.Expired)
            {
                // local admin can always connect
                message = Resource.AdminExpiredLicense;

            }
            else if (licenseStatus == LicenseChecks.LicenseStatus.Demo)
            {
                var expireDate = chk.GetExpirationDate();
                int daysRemain = (int)expireDate.Subtract(DateTime.Now).TotalDays;
                // Demo License - display the warning
                message = string.Format(Resource.DemoLicense, daysRemain);
            }
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(Application.Current.MainWindow,
                    $"{message}",
                    string.Empty,
                    MessageBoxButton.OK,
                    MessageBoxImage.Asterisk);
                return false;
            }
            return true;
        }

        /// <summary>
        /// is the vault setup valid?
        /// </summary>
        /// <returns></returns>
        private bool CheckVaultSetup()
        {
            var settings = LocalSettings.Load();
            if (settings == null)
            {
                settings = new LocalSettings();
            }
            return (settings.GetVaultInformation != null && !string.IsNullOrWhiteSpace(settings.GetVaultInformation.VaultStorageLocation));
        }

        /// <summary>
        /// Enable or disable the top nav area
        /// </summary>
        /// <param name="allOptionsAvailable"></param>
        public void SetupNavAvailability(bool allOptionsAvailable)
        {
            Vault.IsEnabled = true; // ALWAYS AVAILABLE1!
            Keys.IsEnabled = allOptionsAvailable;
            AutoLoad.IsEnabled = allOptionsAvailable;
            ExportVault.IsEnabled = allOptionsAvailable;
            Preferences.IsEnabled = allOptionsAvailable; // need to think about this one - it might need to be always on
        }

        /// <summary>
        /// Login to the vault
        /// </summary>
        private void ConnectToLocalVault()
        {
            ProcessLogin(LocalVaultConnection);
            VaultSetup_Click(null, null);
        }

        /// <summary>
        /// Process the login attempt
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool ProcessLogin(ConnectionInformation data)
        {
            bool retval = false;
            //var helper = new Helpers.LoginHelper();
            //var loginStatus = helper.ProcessLogin(data);
            //string message = string.Empty;
            //if (loginStatus != null && loginStatus.Result == LoginResult.ResultList.Success)
            //{
            //    message = ProcessConnectionChanged();
            //    retval = true;
            //}
            //else
            //{
            //    Connections_Click(null, null);
            //}

            ////TODO: Write my own so that the messagebox will center over the application
            //MessageBox.Show(Application.Current.MainWindow,
            //    $"{loginStatus.Message}. {message}",
            //    WpfConsole.Resources.Resource.LoginName,
            //    MessageBoxButton.OK,
            //    MessageBoxImage.Asterisk);
            return retval;
        }

        /// <summary>
        /// License Issues
        /// </summary>
        private string LicenseSetup()
        {
            ////////string message = string.Empty;

            ////////// do we have a license file?
            ////////var chk = new LicenseChecks();
            ////////var licenseStatus = chk.GetLicenseStatus();
            ////////if (licenseStatus == LicenseChecks.LicenseStatus.Missing)
            ////////{
            ////////    chk.CreateDemoLicense();
            ////////}
            ////////else if (licenseStatus == LicenseChecks.LicenseStatus.Expired)
            ////////{
            ////////    // local admin can always connect
            ////////    var settings = LocalSettings.Load();
            ////////    if (settings.LastConnection.IsLocalAdmin)
            ////////    {
            ////////        message = Resource.AdminExpiredLicense;
            ////////    }
            ////////    else
            ////////    {
            ////////        message = Resource.ExpiredLicense;
            ////////        GlobalValues.LastConnection = null;
            ////////    }
            ////////}
            ////////else if (licenseStatus == LicenseChecks.LicenseStatus.Demo)
            ////////{
            ////////    var expireDate = chk.GetExpirationDate();
            ////////    int daysRemain = (int)expireDate.Subtract(DateTime.Now).TotalDays;
            ////////    // Demo License - display the warning
            ////////    message = string.Format(Resource.DemoLicense, daysRemain);
            ////////}
            ////////return message;
            return String.Empty;
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
        /// Remove the currently displayed control and switch to the control passed in
        /// </summary>
        private void DisplayControl(UserControl displayControl)
        {
            for (int i = (MenuSelectionDisplay.Children.Count - 1); i >= 0; i--)
            {
                MenuSelectionDisplay.Children.RemoveAt(i);
            }
            MenuSelectionDisplay.Children.Add(displayControl);
        }

        /// <summary>
        /// Events supported
        /// </summary>
        private void EventSetup()
        {
            // setup handlers and events
            AddHandler(VaultSetup.VaultChangesEvent,
                new RoutedEventHandler(VaultSetup_VaultChanges));
        }

        #endregion

        #region Top Nav clicks

        /// <summary>
        /// Vault Setup clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VaultSetup_Click(object sender, RoutedEventArgs e)
        {
            if (_VaultSetup == null)
            {
                _VaultSetup = new VaultSetup();
            }
            DisplayControl(_VaultSetup);
        }

        /// <summary>
        /// Key menu clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyManagement_Click(object sender, RoutedEventArgs e)
        {
            if (_KeyManagement == null)
            {
                _KeyManagement = new KeysMain();
            }
            DisplayControl(_KeyManagement);
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
        /// Top Menu Export option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportVault_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Wire up the form
            if (_ExportMain == null)
            {
                _ExportMain = new ExportMain();
            }
            DisplayControl(_ExportMain);
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

        #endregion

    }
}

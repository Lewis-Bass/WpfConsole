using Common;
using Common.Licenses;
using Common.Settings;
using DialogLibrary.SystemDialogs;
using Serilog;
using System;
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

            // do we have any problems associated with running the back end to make the user aware of ?
            // TODO: Check disk space
        }

        /// <summary>
        /// OnLoad event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLoad(object sender, RoutedEventArgs e)
        {
            // do we have a purchase key?
            var settings = LocalSettings.Load();
            if (settings == null)
            {
                settings = new LocalSettings();
            }
            if (string.IsNullOrWhiteSpace(settings.CustomerKey))
            {
                //var resp = MessageBox.Show(Application.Current.MainWindow, Resource.PurchaseKeyIf, string.Empty, MessageBoxButton.YesNo, MessageBoxImage.Question);
                //if (resp == MessageBoxResult.Yes)
                //{
                //    // TODO: Redirect to the web site
                //}

                var dlg = new UserInput(Resource.PurchaseKeyDescription);
                if (Application.Current.MainWindow.IsLoaded)
                {
                    dlg.Owner = Application.Current.MainWindow;
                }
                bool ok = false;
                while (!ok)
                {

                    if (dlg.ShowDialog() == true)
                    {
                        //pin = dlg.Answer;
                        settings.CustomerKey = dlg.Answer;
                    }

                    // TODO: Verify that the customer key is valid!

                    ok = !string.IsNullOrWhiteSpace(settings.CustomerKey);
                }
            }
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
            }
        }

        #endregion

        #region Helpers

        private void ConnectToLocalVault()
        {
            ProcessLogin(LocalVaultConnection);


        }

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

        #endregion

        #region Top Nav clicks

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

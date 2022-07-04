using Common;
using Common.Licenses;
using Common.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using Themes.Helpers;
using static Themes.Enumerations.ThemeEnums;
using System.Windows.Interop;
using System.Windows.Markup;
using static Common.Licenses.LicenseChecks;
using Common.ConnectionInfo;
using WindowsData;
using WpfAdmin.Resources;
using System.Net.NetworkInformation;
using DialogLibrary.SystemDialogs;

namespace WpfAdmin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region globals

        public static ConnectionInformation LocalVaultConnection = new ConnectionInformation
        {
            AccessKeyName = "LocalHost",
            IPAddress = "127.0.0.1",
            IsCurrentConnection = true,
        };





        //UserControl _SearchMaster;
        //UserControl _Connections;
        //UserControl _Statistics;
        //UserControl _FileDisplay;
        //UserControl _AutoLoad;
        //UserControl _PreferenceSetup;
        //UserControl _KeyManagement;
        //UserControl _TagManagement;
        //UserControl _CheckedOut;
        //UserControl _MyPassword;
        //UserControl _ExportMain;

        ///////////////////// <summary>
        ///////////////////// Prior Connections - left nav item
        ///////////////////// </summary>
        //////////////////public ObservableCollection<ConnectionInformation> PriorConnections { get; set; } = new ObservableCollection<ConnectionInformation>();

        ///////////////////// <summary>
        ///////////////////// Last (x) Prior Viewed Files  - left nav item
        ///////////////////// </summary>
        //////////////////public LocalFileList PriorFiles { get; set; } = new LocalFileList();

        ///////////////////// <summary>
        ///////////////////// Last (x) Checked out files - left nav
        ///////////////////// </summary>
        //////////////////public LocalFileList CheckFiles { get; set; } = new LocalFileList();

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


        #region

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
        /// When the screen initially loads, reconnect to the ark that was last used
        /// </summary>


        #endregion

        #region Helpers



        #endregion

    }
}

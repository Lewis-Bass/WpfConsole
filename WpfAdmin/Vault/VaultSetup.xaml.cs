using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
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
using System.Collections.ObjectModel;
using Common.Settings;
using WpfAdmin.Resources;
using WpfAdmin.Properties;
using Common.Licenses;

namespace WpfAdmin.Vault
{
    /// <summary>
    /// Interaction logic for VaultSetup.xaml
    /// </summary>
    public partial class VaultSetup : UserControl
    {

        #region Properties

        public VaultInformation Ark { get; set; }

        LocalSettings _Settings;

        #endregion

        #region Constructor
        public VaultSetup()
        {

            InitializeComponent();           
        }

        #endregion

        #region Global Events

        // Create RoutedEvent
        // This creates a static property on the UserControl, ViewFileEvent, which 
        // will be used by the Window, or any control up the Visual Tree, that wants to 
        // handle the event. 
        public static readonly RoutedEvent VaultChangesEvent =
            EventManager.RegisterRoutedEvent("VaultChangesEvent", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(VaultSetup));

        // Create RoutedEventHandler
        // This adds the Custom Routed Event to the WPF Event System and allows it to be 
        // accessed as a property from within xaml if you so desire
        public event RoutedEventHandler VaultChanges
        {
            add { AddHandler(VaultChangesEvent, value); }
            remove { RemoveHandler(VaultChangesEvent, value); }
        }

        #endregion

        #region Form Events

        /// <summary>
        /// Fires when the form is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VaultSetup_Loaded(object sender, RoutedEventArgs e)
        {
            _Settings = LocalSettings.Load();
            // get the ark information from the configuration
            Ark = (_Settings != null && _Settings.GetVaultInformation != null) ? _Settings.GetVaultInformation : new VaultInformation();
            this.DataContext = Ark;

            txtAnswer.Text = (_Settings != null) ? _Settings.CustomerKey : String.Empty;

            EnableDisableArkSetup();

            // if this is the first time (vault not setup) display a message          
            if (Ark == null || string.IsNullOrWhiteSpace(Ark.VaultStorageLocation))
            {
                tbInformationMessage.Text = Resource.VaultInitialSetupMessage;
            }
            else
            {
                tbInformationMessage.Text = String.Empty;
            }
        }

        #endregion

        #region

        /// <summary>
        /// process the customer key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtAnswer.Text))
            {
                // TODO: Verify the key with the web site

                _Settings.CustomerKey = txtAnswer.Text;
            }
            // can the vault be setup now?
            EnableDisableArkSetup();
        }

        /// <summary>
        /// Redirect to the web
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPurchaseWeb_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Resource.PurchaseKeyUrl);
        }

        #endregion

        #region Button Events

        /// <summary>
        /// Display the file browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileBrowser_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new SaveFileDialog
            {
                CheckPathExists = false,
                FileName = txtArkName.Text,
                AddExtension = true,
                DefaultExt = "Ark",
                Filter = "Ark documents (.ark)|*.ark"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                txtFileName.Text = openFileDialog.FileName;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            // if the file exists verify that the user wants to overwrite it
            var settings = LocalSettings.Load();
            settings.GetVaultInformation = Ark;

            // clear any message that might be shown
            VaultSetup_Loaded(sender, e);

            // raise the event
            RaiseEvent(new RoutedEventArgs(VaultSetup.VaultChangesEvent, null));

            // TODO: Send information to the vault        }

        }

        #endregion


        /// <summary>
        /// Can the ark information be updated?
        /// </summary>
        private void EnableDisableArkSetup()
        {
            ArkSetup.IsEnabled = _Settings.CustomerKey != null && !string.IsNullOrEmpty(_Settings.CustomerKey);
        }


        ///////// <summary>
        ///////// Does the device have a license?
        ///////// </summary>
        ///////// <returns></returns>
        //////private bool CheckDeviceLicense()
        //////{
        //////    // Does the local vault have a device license?
        //////    var chk = new LicenseChecks();
        //////    var licenseStatus = chk.GetLicenseStatus();
        //////    string message = string.Empty;
        //////    if (licenseStatus == LicenseChecks.LicenseStatus.Missing)
        //////    {
        //////        chk.CreateDemoLicense();
        //////    }
        //////    else if (licenseStatus == LicenseChecks.LicenseStatus.Expired)
        //////    {
        //////        // local admin can always connect
        //////        message = Resource.AdminExpiredLicense;

        //////    }
        //////    else if (licenseStatus == LicenseChecks.LicenseStatus.Demo)
        //////    {
        //////        var expireDate = chk.GetExpirationDate();
        //////        int daysRemain = (int)expireDate.Subtract(DateTime.Now).TotalDays;
        //////        // Demo License - display the warning
        //////        message = string.Format(Resource.DemoLicense, daysRemain);
        //////    }
        //////    if (!string.IsNullOrEmpty(message))
        //////    {
        //////        MessageBox.Show(Application.Current.MainWindow,
        //////            $"{message}",
        //////            string.Empty,
        //////            MessageBoxButton.OK,
        //////            MessageBoxImage.Asterisk);
        //////        return false;
        //////    }
        //////    return true;
        //////}

    }
}

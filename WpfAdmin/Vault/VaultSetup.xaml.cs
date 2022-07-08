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

namespace WpfAdmin.Vault
{
    /// <summary>
    /// Interaction logic for VaultSetup.xaml
    /// </summary>
    public partial class VaultSetup : UserControl
    {

        #region Properties

        public VaultInformation Ark { get; set; }
        
        #endregion

        #region Constructor
        public VaultSetup()
        {
          
            InitializeComponent();
            // get the ark information from the configuration

            var settings = LocalSettings.Load();            
            Ark = (settings != null && settings.GetVaultInformation != null) ? settings.GetVaultInformation: new VaultInformation();
            this.DataContext = Ark;             
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
                txtFileName.Text = openFileDialog.FileName;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            // if the file exists verify that the user wants to overwrite it
            var settings = LocalSettings.Load();
            settings.GetVaultInformation = Ark;

            // raise the event
            RaiseEvent(new RoutedEventArgs(VaultSetup.VaultChangesEvent, null));

            // TODO: Send information to the vault
        }

        #endregion

    }
}

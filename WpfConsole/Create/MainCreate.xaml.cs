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

namespace WpfConsole.Create
{
    /// <summary>
    /// Interaction logic for MainCreate.xaml
    /// </summary>
    public partial class MainCreate : UserControl
    {

        #region properties
        public ArkData Ark { get; set; }

        #endregion

        #region constructor
        public MainCreate()
        {
          
            InitializeComponent();
            // get the ark information from the configuration
            var ark = new ArkData();
            //ark.LoadFromConfig();            
            Ark = ark;
            this.DataContext = Ark;             
        }

        #endregion

        #region button events

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

        }

        #endregion

        #region password events
        private void Password1_LostFocus(object sender, RoutedEventArgs e)
        {
            
        }

        #endregion
    }
}

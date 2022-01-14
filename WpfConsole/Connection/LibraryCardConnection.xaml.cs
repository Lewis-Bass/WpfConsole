using Common;
using Microsoft.Win32;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using WindowsData;

namespace WpfConsole.Connection
{
    /// <summary>
    /// Interaction logic for LibraryCardConnection.xaml
    /// </summary>
    public partial class LibraryCardConnection : Window
    {
        public LibraryCard LibraryCard { get; set; }
        public LibraryCardConnection()
        {
            InitializeComponent();
        }

        private void btnFileBrowser_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                AddExtension = true,
                Multiselect = false,
                Filter = "Library Card|*.Card"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    var fileContent = File.ReadAllText(fileName);

                    ProcessConnection(fileContent);
                }
            }
        }

        private void btnString_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                txtError.Text = WpfConsole.Resources.Resource.ErrorMissingLibraryCard;
            }
            else
            {
                ProcessConnection(txtMessage.Text);
            }

        }

        private void ProcessConnection(string cardString)
        {
            LibraryCard card = null;
            // serialize the object from the file
            try
            {
                card = JsonConvert.DeserializeObject<LibraryCard>(cardString);
            }
            catch (Exception ex)
            {
                Log.Error(ex, cardString);
                txtError.Text = WpfConsole.Resources.Resource.ErrorBadLibraryCard;
                return;
            }
            txtError.Text = string.Empty;

            // setup return value
            LibraryCard = card;
            this.DialogResult = true;
            this.Close();
        }

    }
}

using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WindowsData;
using WpfConsole.Properties;

namespace WpfConsole.AutoLoad
{
    /// <summary>
    /// Interaction logic for AutoLoadMain.xaml
    /// </summary>
    public partial class AutoLoadMain : UserControl
    {
        #region Properties

        LocalSettings settings = LocalSettings.Load();

        public ObservableCollection<string> FileList { get; set; } = new ObservableCollection<string>();

        public Dictionary<int, string> timeValues = new Dictionary<int, string>()
        {
            {0, "Midnight" },
            {1, "1:00 AM" },
            {2, "2:00 AM" },
            {3, "3:00 AM" },
            {4, "4:00 AM" },
            {5, "5:00 AM" },
            {6, "6:00 AM" },
            {7, "7:00 AM" },
            {8, "8:00 AM" },
            {9, "9:00 AM" },
            {10, "10:00 AM" },
            {11, "11:00 AM" },
            {12, "Noon" },
            {13, "1:00 PM" },
            {14, "2:00 PM" },
            {15, "3:00 PM" },
            {16, "4:00 PM" },
            {17, "5:00 PM" },
            {18, "6:00 PM" },
            {19, "7:00 PM" },
            {20, "8:00 PM" },
            {21, "9:00 PM" },
            {22, "10:00 PM" },
            {23, "11:00 PM" },
        };

        #endregion

        #region Constructor and initilization

        public AutoLoadMain()
        {
            InitializeComponent();

            DataContext = this;

            LoadScreenValues();
        }

        private void LoadScreenValues()
        {
            cbConnection.ItemsSource = settings.ConnectionsData;
            for (int i = 0; i < cbConnection.Items.Count; i++)
            {
                var connect = cbConnection.Items[i] as ConnectionInformation;
                if (connect.AccessKeyName == ((settings.AutoLoadConnection == null) ? settings.LastConnection : settings.AutoLoadConnection).AccessKeyName)
                {
                    cbConnection.SelectedIndex = i;
                    break;
                }
            }

            SetTimeControl(cbStartTime, timeValues, settings.AutoLoadStartTime);
            SetTimeControl(cbEndTime, timeValues, settings.AutoLoadEndTIme);

            tbLastProcessed.Text = settings.AutoLoadLastProcessed.ToShortDateString();
            tbTotalUpload.Text = settings.AutoLoadLastTotalUpload.ToString();

            if (settings.AutoLoadDirectories == null || settings.AutoLoadDirectories.Count <= 0)
            {
                settings.AutoLoadDirectories = new List<string>
                {
                    WpfConsole.Resources.Resource.MyDocuments,
                    WpfConsole.Resources.Resource.MyMusic,
                    WpfConsole.Resources.Resource.MyPictures,
                    WpfConsole.Resources.Resource.MyVideo,
                };
            }

            FileList = new ObservableCollection<string>(settings.AutoLoadDirectories);
            lvDirectories.ItemsSource = FileList;
        }

        private void SetTimeControl(ComboBox control, Dictionary<int, string> bindSource, int selection)
        {
            control.ItemsSource = bindSource;
            control.SelectedIndex = selection;
        }

        #endregion

        #region Button Clicks

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var btn = (Button)sender;
                FileList.Remove(btn.DataContext.ToString());
                settings.AutoLoadDirectories = FileList.Select(r => r).ToList();
            }
        }

        private void BtnDirectoryBrowse_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.FolderBrowserDialog()
            {
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var dirName = openFileDialog.SelectedPath;
                FileList.Add(dirName);
                settings.AutoLoadDirectories = FileList.Select(r => r).ToList();
            }
        }

        #endregion

        #region Selection Changed Events

        private void cbConnection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                var cb = sender as ComboBox;
                if (cb.SelectedItem is ConnectionInformation)
                {
                    settings.AutoLoadConnection = cb.SelectedItem as ConnectionInformation;
                }
            }
        }
        
        private void cbStartTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                var cb = sender as ComboBox;
                if (cb.SelectedItem is KeyValuePair<int, string>)
                {
                    var val = (KeyValuePair<int, string>)cb.SelectedItem;
                    settings.AutoLoadStartTime = val.Key;
                }
            }
        }

        private void cbEndTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                var cb = sender as ComboBox;
                if (cb.SelectedItem is KeyValuePair<int, string>)
                {
                    var val = (KeyValuePair<int, string>)cb.SelectedItem;
                    settings.AutoLoadEndTIme = val.Key;
                }
            }
        }

        #endregion
    }
}

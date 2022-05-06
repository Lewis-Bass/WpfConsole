﻿using Common;
using Common.Settings;
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

        AutoLoadSettings settings;

        /// <summary>
        /// List of directories to scan
        /// </summary>
        public ObservableCollection<DirectoriesToScan> FileList { get; set; } = new ObservableCollection<DirectoriesToScan>();

        /// <summary>
        /// start and end time values
        /// </summary>
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

        #region Constructor and initialization

        public AutoLoadMain()
        {
            InitializeComponent();

            settings = AutoLoadSettings.Load(false);
            if (settings == null)
            {
                settings = new AutoLoadSettings();
            }

            DataContext = this;

            LoadScreenValues();
        }

        private void LoadScreenValues()
        {
            var globalSettings = LocalSettings.Load();
            cbConnection.ItemsSource = globalSettings.ConnectionsData;
            for (int i = 0; i < cbConnection.Items.Count; i++)
            {
                var connect = cbConnection.Items[i] as ConnectionInformation;
                if (settings != null &&
                    (connect.AccessKeyName == ((settings.AutoLoadConnection == null) ? globalSettings.LastConnection : settings.AutoLoadConnection).AccessKeyName))
                {
                    cbConnection.SelectedIndex = i;
                    break;
                }
            }

            if (settings != null)
            {
                SetTimeControl(cbStartTime, timeValues, settings.AutoLoadStartTime);
                SetTimeControl(cbEndTime, timeValues, settings.AutoLoadEndTime);

                tbLastProcessed.Text = settings.AutoLoadLastProcessed.ToShortDateString();
                tbTotalUpload.Text = settings.AutoLoadLastTotalUpload.ToString();
                tbPin.Text = settings.AutoLoadPin;
                FileList = new ObservableCollection<DirectoriesToScan>(settings.AutoLoadDirectories);
            }
            lvDirectories.ItemsSource = FileList;
        }

        private void SetTimeControl(ComboBox control, Dictionary<int, string> bindSource, int selection)
        {
            control.ItemsSource = bindSource;
            control.SelectedIndex = selection;
        }

        #endregion

        #region Button Clicks

        /// <summary>
        /// remove an entry from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var btn = (Button)sender;
                FileList.Remove((DirectoriesToScan)btn.DataContext);
                settings.AutoLoadDirectories = FileList.Select(r => r).ToList();
            }
        }

        /// <summary>
        /// browse windows for the directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDirectoryBrowse_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.FolderBrowserDialog()
            {
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var dirName = openFileDialog.SelectedPath;
                CreateEntry(dirName);
            }
        }

        /// <summary>
        /// Create an entry in the directories list
        /// if the entry already exists it is skipped
        /// </summary>
        /// <param name="pathName"></param>
        private void CreateEntry(string pathName)
        {
            var userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            if (!FileList.Any(r => r.PathName == pathName && r.UserName == userName))
            {
                var dirSplit = pathName.Split(new char[] { '/', '\\' });

                var dirScan = new DirectoriesToScan
                {
                    DirectoryName = dirSplit[(dirSplit.Length - 1)],
                    PathName = pathName,
                    UserName = userName,
                };
                FileList.Add(dirScan);
                settings.AutoLoadDirectories = FileList.Select(r => r).ToList();
            }
        }

        /// <summary>
        /// Save the configuration - send it to the back end
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            settings.WriteToWebSite();
        }

        /// <summary>
        /// Get the system directories and make certain that they are in the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSystemDirectory_Click(object sender, RoutedEventArgs e)
        {
            CreateEntry(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            CreateEntry(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            CreateEntry(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            CreateEntry(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
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
                    settings.AutoLoadEndTime = val.Key;
                }
            }
        }

        private void tbPin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox)
            {
                var tb = sender as TextBox;
                settings.AutoLoadPin = tb.Text;
            }
        }



        //private void tbPin_PasswordChanged(object sender, RoutedEventArgs e)
        //{
        //    if (sender is PasswordBox)
        //    {
        //        var tb = sender as PasswordBox;
        //        settings.AutoLoadPin = tb.Password;
        //    }
        //}
        #endregion

        
    }
}

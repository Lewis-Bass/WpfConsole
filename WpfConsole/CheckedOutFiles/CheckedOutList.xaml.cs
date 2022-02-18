using Common;
using Common.ServerCommunication.Helpers;
using Common.ServerCommunication.Requests;
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
//using WpfConsole.Properties;
using System.Linq;
using Microsoft.Win32;
using System.Collections.ObjectModel;
//using WpfConsole.Search;
using WpfConsole.SearchMaster;
using WpfConsole.Resources;
using Common.Settings;

namespace WpfConsole.CheckedOutFiles
{
    /// <summary>
    /// Interaction logic for CheckedOutList.xaml
    /// </summary>
    public partial class CheckedOutList : UserControl
    {
        #region Globals and Constructor

        /// <summary>
        /// List of files that are checked out
        /// </summary>
        public ObservableCollection<LocalFileStatus> FilesCheckedOut { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CheckedOutList()
        {
            InitializeComponent();

            DataContext = this;

            var settings = LocalSettings.Load();
            cbUserFile.IsChecked = settings.CheckOutShowOnlyMyFiles;
            cbRemoveFile.IsChecked = settings.CheckOutRemoveFileAtCheckin;
        }

        /// <summary>
        /// When the form is visible get the data to display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                LoadCheckOuts();
            }
        }

        #endregion

        #region Load files that are checked out

        /// <summary>
        /// load the files that are checked out locally and from the vault
        /// </summary>
        private void LoadCheckOuts()
        {
            // get list of files that have been checked out locally
            var lf = new LocalFiles();
            var localFiles = lf.LocalFileCheckedOut;           

            // combine the 2 lists
            var combineFiles = new List<LocalFileStatus>();
            if (cbUserFile.IsChecked.HasValue && cbUserFile.IsChecked.Value)
            {
                combineFiles.AddRange(localFiles);
            }
            else
            {
                // get list of files that have been checked out on the server
                var request = new BaseRequest
                {
                    Connection = GlobalValues.LastConnection
                };
                var serverFiles = CheckInOutHelpers.GetAllCheckedOutFiles(request);
                combineFiles.AddRange(localFiles);
                var temp = localFiles.Select(r => r.VaultID);
                combineFiles.AddRange(serverFiles.CheckOutList.Where(r => !temp.Contains(r.VaultID)));
            }
            FilesCheckedOut = new ObservableCollection<LocalFileStatus>(combineFiles);

            DisplayResults.ItemsSource = FilesCheckedOut;
        }

        #endregion

        #region Routed Event Handlers

        // Create RoutedEvent
        // This creates a static property on the UserControl, CheckOutFileEvent, which 
        // will be used by the Window, or any control up the Visual Tree, that wants to 
        // handle the event. 
        public static readonly RoutedEvent CheckInFileEvent =
            EventManager.RegisterRoutedEvent("CheckInFileEvent", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(CheckedOutList));

        // Create RoutedEventHandler
        // This adds the Custom Routed Event to the WPF Event System and allows it to be 
        // accessed as a property from within xaml if you so desire
        public event RoutedEventHandler CheckInFile
        {
            add { AddHandler(CheckInFileEvent, value); }
            remove { RemoveHandler(CheckInFileEvent, value); }
        }

        #endregion

        #region View File

        /// <summary>
        /// View the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var button = (Button)sender;
                if (button.DataContext != null && button.DataContext is LocalFileStatus)
                {
                    var file = (LocalFileStatus)button.DataContext;
                    var lf = new LocalFiles();
                    lf.ViewFile(file);
                }
            }

        }

        #endregion

        #region Check In

        /// <summary>
        /// Check the file into the vault
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckIn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var button = (Button)sender;
                if (button.DataContext != null && button.DataContext is LocalFileStatus)
                {
                    var file = (LocalFileStatus)button.DataContext;
                    var lf = new LocalFiles();
                    if (lf.CheckInFile(file, (cbRemoveFile.IsChecked == true)))
                    {
                        RaiseEvent(new RoutedEventArgs(CheckInFileEvent));
                        FilesCheckedOut.Remove(file);
                    }
                    else
                    {
                        // error occurred
                        MessageBox.Show(Resource.CheckInError, Resource.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        #endregion

        #region Cancel Check In

        /// <summary>
        /// cancel the check in on the object - remove from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelCheckIn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var button = (Button)sender;
                if (button.DataContext != null && button.DataContext is LocalFileStatus)
                {
                    var file = (LocalFileStatus)button.DataContext;
                    var lf = new LocalFiles();
                    if (lf.CancelCheckInFile(file))
                    {
                        RaiseEvent(new RoutedEventArgs(CheckInFileEvent));
                        FilesCheckedOut.Remove(file);
                    }
                    else
                    {
                        // error occurred
                        MessageBox.Show(Resource.CheckInCancelError, Resource.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        #endregion

        #region Match

        /// <summary>
        /// Match the vault file with a local file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Match_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var button = (Button)sender;
                if (button.DataContext != null && button.DataContext is LocalFileStatus)
                {
                    var file = (LocalFileStatus)button.DataContext;

                    // let the user find the file to match up with
                    var openFileDialog = new OpenFileDialog
                    {
                        CheckFileExists = true,
                        CheckPathExists = true,
                        AddExtension = true,
                        Multiselect = false,
                        DereferenceLinks = true,
                        Title = Resource.FileBrowserTypeTitle,
                        Filter = $"{file.DocumentName} | {file.DocumentName} | {Resource.AllFiles} (*.*)|*.*"
                    };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        file.LocalFileLocation = openFileDialog.FileName;
                        var check = FilesCheckedOut.FirstOrDefault(r => r.DocumentName == file.DocumentName);
                    }
                }
            }
        }

        #endregion

        #region Check-boxes

        /// <summary>
        /// Display files that are checked out
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbUserFile_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
            {
                var cb = (CheckBox)sender;
                var settings = LocalSettings.Load();
                settings.CheckOutShowOnlyMyFiles = cb.IsChecked == true;
                LoadCheckOuts();
            }
        }

        /// <summary>
        /// When the file is checked in - does it get removed?
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbRemoveFile_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
            {
                var cb = (CheckBox)sender;
                var settings = LocalSettings.Load();
                settings.CheckOutRemoveFileAtCheckin = cb.IsChecked == true;
            }
        }

        #endregion

    }
}

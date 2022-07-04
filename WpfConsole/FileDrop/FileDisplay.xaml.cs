using Common;
using Common.ServerCommunication.Helpers;
using Common.ServerCommunication.Requests;
using DialogLibrary.SystemDialogs;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WindowsData;
using WpfConsole.Resources;

namespace WpfConsole.FileDrop
{
    /// <summary>
    /// Interaction logic for FileDisplay.xaml
    /// </summary>
    public partial class FileDisplay : UserControl
    {
        #region Properties

        /// <summary>
        /// List of Files to display
        /// </summary>               
        public ObservableCollection<FileTrack> FileList
        {
            get
            {
                return _fileList;
            }
            set
            {
                _fileList = value;
                OnPropertyChanged("FileList");
            }
        }
        ObservableCollection<FileTrack> _fileList = new ObservableCollection<FileTrack>();

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public FileDisplay()
        {
            InitializeComponent();
            DataContext = this;

        }

        /// <summary>
        /// Constructor that passes in the file list to display
        /// </summary>
        /// <param name="inList"></param>
        public FileDisplay(string[] inList)
        {
            InitializeComponent();
            AddMultipleFiles(inList);
            DataContext = this;
        }

        #endregion

        #region File Browser

        /// <summary>
        /// open the file browser to select the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFileBrowser_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                AddExtension = true,
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    CheckAndAddFile(fileName);
                }
            }
        }

        #endregion

        #region File Load

        /// <summary>
        /// load the list of files for display on the screen
        /// </summary>
        /// <param name="inList"></param>
        private void AddMultipleFiles(string[] inList)
        {
            if (inList.Length > 0)
            {
                foreach (var name in inList)
                {
                    if (File.Exists(name))
                    {
                        CheckAndAddFile(name);
                    }
                    else if (Directory.Exists(name))
                    {
                        LoadDirectory(name);
                    }
                }
            }
        }

        /// <summary>
        /// load all of the files from a directory - recursive
        /// </summary>
        /// <param name="dirName"></param>
        private void LoadDirectory(string dirName)
        {
            foreach (var fname in Directory.GetFiles(dirName))
            {
                CheckAndAddFile(fname);
            }

            foreach (var subDir in Directory.GetDirectories(dirName))
            {
                LoadDirectory(subDir);
            }
        }

        #endregion

        #region Drag Drop

        /// <summary>
        /// start drag a file to be placed on the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragDropZone_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        /// <summary>
        /// drag over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragDropZone_DragOver(object sender, DragEventArgs e)
        {

        }

        /// <summary>
        /// A file was dropped in the drag zone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragDropZone_Drop(object sender, DragEventArgs e)
        {
            string[] inList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            AddMultipleFiles(inList);
        }

        #endregion

        #region Button Events

        /// <summary>
        /// Remove all files from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            FileList.Clear();
        }

        /// <summary>
        /// remove a file from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var obj = ((Button)sender).DataContext;
                if (obj is WindowsData.FileTrack)
                {
                    var fi = (WindowsData.FileTrack)obj;
                    FileList.Remove(fi);
                }
            }
        }

        /// <summary>
        /// Send the files to the ark
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            // TODO: can this be done in parallel - wait for the site to be active first!
            //foreach (var userFile in ProcessFileList.FileList.ToArray())
            foreach (var userFile in FileList.ToArray())
            {
                if (File.Exists(userFile.FileName))
                {
                    var fileInfo = new FileInfo(userFile.FileName);


                    // setup the transfer 
                    var fileSave = new RequestFileSend
                    {
                        Connection = GlobalValues.LastConnection,
                        ConnectionToken = GlobalValues.ConnectionToken,
                        FileName = userFile.FileName,
                        FileContents = File.ReadAllBytes(userFile.FileName),

                        Attributes = fileInfo.Attributes.ToString(),
                        CreationTime = fileInfo.CreationTime,
                        ComputerName = Environment.MachineName,
                        Directory = fileInfo.Directory.ToString(),
                        Extension = fileInfo.Extension,
                        IsReadOnly = fileInfo.IsReadOnly,
                        LastAccessTime = fileInfo.LastAccessTime,
                        LastWriteTime = fileInfo.LastWriteTime,
                        Length = fileInfo.Length,
                        OperatingSystemVersion = Environment.OSVersion.ToString(),
                        UserName = Environment.UserName
                    };

                    // send to web site
                    var result = FileHelpers.SendFiles(fileSave);


                    // did it process successfully? remove from list
                    if (result.ErrorList == null || result.ErrorList.Length <= 0)
                    {
                        FileList.Remove(userFile);
                    }

                    //TODO:  display some kind of status to the user...

                }
            }

            MessageBox.Show(Resource.FileTransferCompleted, Resource.Notice, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        /// <summary>
        /// Add tags to the file 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTags_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.Write("DDD");
            var data = (FileTrack)((Button)sender).DataContext;

            var dlg = new TagChange();
            var results = new SearchResults
            {
                DocumentName = data.FileName
            };
            results.Tags = data.Tags;
            dlg.ResultInfo = results;
            dlg.Owner = Application.Current.MainWindow;
            dlg.ShowDialog();


            // update the display source with the new tag values
            foreach (var rec in FileList)
            {
                if (rec.FileName == dlg.ResultInfo.DocumentName)
                {
                    rec.Tags = new ObservableCollection<MetaTags>(dlg.ResultInfo.Tags);
                }
            }
            OnPropertyChanged("FileList");
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Add a file to the list
        /// It will check to make certain that the file is not already in the list
        /// </summary>
        /// <param name="fileName"></param>
        private void CheckAndAddFile(string fileName)
        {
            if (!FileList.Any(r => r.FileName == fileName))
            {
                FileList.Add(new WindowsData.FileTrack { FileName = fileName });
            }
        }


        #endregion

    }
}

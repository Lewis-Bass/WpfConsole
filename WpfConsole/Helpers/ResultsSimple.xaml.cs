using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using WpfConsole.Dialogs;
using WpfConsole.SearchMaster;

namespace WpfConsole.Helpers
{
    /// <summary>
    /// Interaction logic for ResultsSimple.xaml
    /// </summary>
    public partial class ResultsSimple : UserControl, INotifyPropertyChanged
    {
        #region Properties/Constructor 

        ObservableCollection<SearchResults> _SearchResultsInfo = new ObservableCollection<SearchResults>();

        public ObservableCollection<SearchResults> SearchResultsInfo
        {
            get { return _SearchResultsInfo; }
            set
            {
                _SearchResultsInfo = value;
                DisplayResults.ItemsSource = SearchResultsInfo;
                OnPropertyChanged("SearchResultsInfo");
            }
        }

        public ResultsSimple()
        {
            InitializeComponent();
            this.DataContext = this;
            DisplayResults.ItemsSource = SearchResultsInfo;
        }

        #endregion

        #region View File

        private void View_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var button = (Button)sender;
                if (button.DataContext != null && button.DataContext is SearchResults)
                {
                    var searchResult = (SearchResults)button.DataContext;

                    var file = new LocalFileStatus
                    {
                        //VaultID = searchResult.
                        DocumentName = searchResult.DocumentName,
                        IsCheckedOut = false,
                        DateRecieved = DateTime.Now
                    };

                    // ask the user where the file is to be placed
                    var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                    saveFileDialog.OverwritePrompt = true;
                    saveFileDialog.CheckPathExists = true;
                    saveFileDialog.FileName = searchResult.DocumentName;
                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        file.LocalFileLocation = saveFileDialog.FileName;
                        var localFiles = new Common.LocalFiles();
                        RaiseEvent(new RoutedEventArgs(MainSearch.ViewFileEvent, null));
                    }

                }
            }
        }

        #endregion

        #region Checkout file
        private void CheckOut_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var button = (Button)sender;
                if (button.DataContext != null && button.DataContext is SearchResults)
                {
                    var searchResult = (SearchResults)button.DataContext;

                    var file = new LocalFileStatus
                    {
                        //VaultID = searchResult.
                        DocumentName = searchResult.DocumentName,
                        IsCheckedOut = true,
                        DateRecieved = DateTime.Now
                    };

                    // ask the user where the file is to be placed
                    var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                    saveFileDialog.OverwritePrompt = true;
                    saveFileDialog.CheckPathExists = true;
                    saveFileDialog.FileName = searchResult.DocumentName;
                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        file.LocalFileLocation = saveFileDialog.FileName;
                        var localFiles = new Common.LocalFiles();
                        localFiles.CheckOutFile(file);
                        RaiseEvent(new RoutedEventArgs(MainSearch.CheckOutFileEvent, null));
                    }
                }
            }
        }

        #endregion

        #region Tag Change

        /// <summary>
        /// Change the tags associated with the document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TagChange_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            // show the tag change window
            var dlg = new TagChange();
            dlg.Owner = Application.Current.MainWindow;
            dlg.ResultInfo = (SearchResults)btn.DataContext;
            dlg.ShowDialog();

            // update the display source with the new tag values
            foreach (var rec in SearchResultsInfo)
            {
                if (rec.DocumentName == dlg.ResultInfo.DocumentName)
                {
                    rec.Tags = new ObservableCollection<MetaTags>(dlg.ResultInfo.Tags);
                }
            }
            OnPropertyChanged("SearchResultsInfo");
        }

        #endregion


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

    }
}

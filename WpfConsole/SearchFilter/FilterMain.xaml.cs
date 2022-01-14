using System;
using System.Collections.Generic;
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
using Common;
using Common.ServerCommunication.Helpers;
using Common.ServerCommunication.Requests;
using Microsoft.Win32;
using WpfConsole.Resources;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WindowsData;
using WpfConsole.Search;



using WindowsData;
using WpfConsole.Connection;
using WpfConsole.Resources;
using static System.Net.Mime.MediaTypeNames;

namespace WpfConsole.SearchFilter
{
    /// <summary>
    /// Interaction logic for FilterMain.xaml
    /// </summary>
    public partial class FilterMain : UserControl
    {
        public FilterMain()
        {
            InitializeComponent();
        }

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
                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.OverwritePrompt = true;
                    saveFileDialog.CheckPathExists = true;
                    saveFileDialog.FileName = searchResult.DocumentName;
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        file.LocalFileLocation = saveFileDialog.FileName;
                        var localFiles = new Common.LocalFiles();
                        localFiles.ViewFile(file);
                        //RaiseEvent(new RoutedEventArgs(ViewFileEvent));
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
                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.OverwritePrompt = true;
                    saveFileDialog.CheckPathExists = true;
                    saveFileDialog.FileName = searchResult.DocumentName;
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        file.LocalFileLocation = saveFileDialog.FileName;
                        var localFiles = new Common.LocalFiles();
                        localFiles.CheckOutFile(file);
                        //RaiseEvent(new RoutedEventArgs(CheckOutFileEvent));
                    }
                }
            }

        }

        #endregion

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var criteria = new List<SearchCriteriaBase>();
            //if (!string.IsNullOrWhiteSpace(txtPath.Text))
            //{
            //    var search = new SearchCriteriaBase
            //    {
            //        Field = "Path",
            //        Criteria = "Matches",
            //        Relationship = "&",
            //        ValueMin = txtPath.Text
            //    };
            //    criteria.Add(search);
            //}
            if (!string.IsNullOrWhiteSpace(txtFileName.Text))
            {
                var search = new SearchCriteriaBase
                {
                    Field = "FileName",
                    Criteria = "Matches",
                    Relationship = "&",
                    ValueMin = txtFileName.Text
                };
                criteria.Add(search);
            }
            if (!string.IsNullOrWhiteSpace(txtFileExtension.Text))
            {
                var search = new SearchCriteriaBase
                {
                    Field = "Extension",
                    Criteria = "Matches",
                    Relationship = "&",
                    ValueMin = txtFileExtension.Text
                };
                criteria.Add(search);
            }
            if (dtBegin.SelectedDate != null && dtEnd.SelectedDate != null)
            {
                var search = new SearchCriteriaBase
                {
                    Field = "Date",
                    Criteria = "Matches",
                    Relationship = "&",
                    ValueMinDate = dtBegin.SelectedDate.Value,
                    ValueMaxDate = dtEnd.SelectedDate.Value
                };
                criteria.Add(search);
            }
            else if (dtBegin.SelectedDate != null)
            {
                var search = new SearchCriteriaBase
                {
                    Field = "Date",
                    Criteria = ">=",
                    Relationship = "&",
                    ValueMinDate = dtBegin.SelectedDate.Value
                };
                criteria.Add(search);
            }
            else if (dtEnd.SelectedDate != null)
            {
                var search = new SearchCriteriaBase
                {
                    Field = "Date",
                    Criteria = "<=",
                    Relationship = "&",
                    ValueMaxDate = dtEnd.SelectedDate.Value
                };
                criteria.Add(search);
            }

            if (!criteria.Any())
            {
                txtError.Text = Resource.RequiredCriteria; // "Error: Filter Criteria Required";
                return;
            }

            var request = new RequestSearch
            {
                Connection = GlobalValues.LastConnection,
                ConnectionToken = GlobalValues.ConnectionToken,
                StartingEntry = 0,
                Search = criteria
            };

            var results = SearchHelpers.ProcessSearch(request);
            var SearchResultsInfo = new ObservableCollection<SearchResults>(results);
            DisplayResults.ItemsSource = SearchResultsInfo;
        }
    }
}

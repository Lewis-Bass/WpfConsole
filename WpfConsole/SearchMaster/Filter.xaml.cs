using Common.ServerCommunication.Helpers;
using Common.ServerCommunication.Requests;
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
using WpfConsole.Connection;
using WpfConsole.Resources;


namespace WpfConsole.SearchMaster
{
    /// <summary>
    /// Interaction logic for Filter.xaml
    /// </summary>
    public partial class Filter : UserControl
    {
        public UserControl DisplayResult { get; set; }

        public Filter()
        {
            
            InitializeComponent();
        }

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

            // pass the query to the results screen
            RaiseEvent(new RoutedEventArgs(MainSearch.PerformSearchEvent, criteria));
                       
        }

    }
}

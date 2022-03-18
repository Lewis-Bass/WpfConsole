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
using System.ComponentModel;
using Common.MailSend;

namespace WpfConsole.SearchMaster
{
    /// <summary>
    /// Interaction logic for Filter.xaml
    /// </summary>
    public partial class Filter : UserControl, INotifyPropertyChanged
    {
        #region Properties

        public UserControl DisplayResult { get; set; }

        #endregion

        #region constructor

        public Filter()
        {

            InitializeComponent();
            this.DataContext = this;

        }

        #endregion

        #region Bind Values to those passed in

        public void BindInitialValues(List<SearchCriteriaBase> initialValues)
        {
            foreach (var value in initialValues)
            {
                if (value.Field == "FileName")
                {
                    txtFileName.Text = value.ValueMin;
                }
                else if (value.Field == "Extension")
                {
                    txtFileExtension.Text = value.ValueMin;
                }
                else if (value.Field == "Tag")
                {
                    txtTagName.Text = value.ValueMin;
                }
                else if (value.Field == "Date")
                {
                    dtBegin.Text = value.ValueMinDate.ToShortDateString();
                    dtEnd.Text = value.ValueMaxDate.ToShortDateString();
                }
            }
        }

        #endregion


        #region Button Clicks

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var criteria = new List<SearchCriteriaBase>();

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
            if (!string.IsNullOrWhiteSpace(txtTagName.Text))
            {
                var search = new SearchCriteriaBase
                {
                    Field = "Tag",
                    Criteria = "Matches",
                    Relationship = "&",
                    ValueMin = txtTagName.Text
                };
                criteria.Add(search);
            }

            // dates require a bit of special handling
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

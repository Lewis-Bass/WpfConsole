using Common.ServerCommunication.Helpers;
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
using System.ComponentModel;
using Common.ServerCommunication.Requests;

namespace WpfConsole.SearchMaster
{
    /// <summary>
    /// Interaction logic for Previous.xaml
    /// </summary>
    public partial class Previous : UserControl
    {
        #region Properties

        ObservableCollection<BinderInformation> _BinderList = new ObservableCollection<BinderInformation>();
        /// <summary>
        /// Collection of binders to show on the screen
        /// </summary>
        public ObservableCollection<BinderInformation> BinderList
        {
            get { return _BinderList; }
            set
            {
                if (BinderList != value)
                {
                    _BinderList = value;
                    OnPropertyChanged("BinderList");
                }
            }
        }
        public UserControl DisplayResult { get; set; }

        #endregion

        #region Constructor

        public Previous()
        {
            InitializeComponent();
            DataContext = this;

            LoadBinderList();
        }

        #endregion

        #region Load Properties

        private void LoadBinderList()
        {
            var request = new RequestBinderList
            {
                Connection = Common.GlobalValues.LastConnection,
                ConnectionToken = GlobalValues.ConnectionToken,
                MaxEntries = 1000
            };
            var values = BinderHelpers.GetBinderList(request);
            if (values.Any())
            {
                BinderList = new ObservableCollection<BinderInformation>(values.OrderBy(r => r.BinderName));
            }
        }

        #endregion

        #region Buttons

        /// <summary>
        /// the selected search binder changed - load the query
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DocumentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSelectedBinder((BinderInformation)DocumentList.SelectedItem);
        }

        /// <summary>
        /// button click - get the results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResultsBinder_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.DataContext is BinderInformation)
            {
                var data = (BinderInformation)button.DataContext;
                LoadSelectedBinder(data);
            }            
        }

        /// <summary>
        /// Process the selection changed
        /// </summary>
        /// <param name="binder"></param>
        private void LoadSelectedBinder(BinderInformation binder)
        {
            if (binder == null)
            {
                return;
            }

            var newCriteria = BinderHelpers.GetBinderSearchCriteria(GlobalValues.LastConnection, binder.BinderID);

            // convert the base to the gui model
            var newGui = newCriteria.Select(r => new SearchCriteriaGUI
            {
                Field = r.Field,
                Relationship = r.Relationship,
                Criteria = r.Criteria,
                ValueMin = r.ValueMin,
                ValueMax = r.ValueMax,
                ValueBool = r.ValueBool,
                ValueMinDate = r.ValueMinDate,
                ValueMaxDate = r.ValueMaxDate
            }).ToArray();
            newGui[0].ShowRelationship = false;
            newGui[0].ShowGroupRelationship = false;

            // pass the query to the results screen
            RaiseEvent(new RoutedEventArgs(MainSearch.PerformSearchEvent, newGui));
        }

        /// <summary>
        /// delete a specific binder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteBinder_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.DataContext is BinderInformation)
            {
                var data = (BinderInformation)button.DataContext;
                BinderList.Remove(data);   
            }
        }

        /// <summary>
        /// The user wants to modify a binders - same as selection changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyBinder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var button = (Button)sender;
                if (button.DataContext is BinderInformation)
                {
                    LoadSelectedBinder((BinderInformation)button.DataContext);
                }
            }
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

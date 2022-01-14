using Common;
using Common.ServerCommunication.Helpers;
using Common.ServerCommunication.Requests;
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

namespace WpfConsole.SearchAdvanced
{
    /// <summary>
    /// Interaction logic for Documents.xaml
    /// </summary>
    public partial class Documents : UserControl
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

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public Documents()
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
                Connection = GlobalValues.LastConnection,
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

        #region Button Clicks

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

            }
        }

        /// <summary>
        /// the selected search binder changed - load the query
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DocumentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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

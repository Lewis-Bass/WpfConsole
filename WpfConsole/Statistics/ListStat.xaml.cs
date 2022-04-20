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

namespace WpfConsole.Statistics
{
    /// <summary>
    /// Interaction logic for ListStat.xaml
    /// </summary>
    public partial class ListStat : UserControl, INotifyPropertyChanged
    {


        List<SearchResults> _SearchResultsInfo = new List<SearchResults>();

        public List<SearchResults> SearchResultsInfo
        {
            get { return _SearchResultsInfo; }
            set
            {
                _SearchResultsInfo = value;
                //////////////////////////////////////////////DisplayResults.SearchResultsInfo = new ObservableCollection<SearchResults>( _SearchResultsInfo);
                OnPropertyChanged("SearchResultsInfo");
            }
        }

        public ListStat()
        {
            InitializeComponent();
        }

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

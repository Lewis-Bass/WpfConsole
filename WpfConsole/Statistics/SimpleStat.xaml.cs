using System;
using System.Collections.Generic;
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
using static System.Net.Mime.MediaTypeNames;

namespace WpfConsole.Statistics
{

    /// <summary>
    /// Interaction logic for SimpleStat.xaml
    /// </summary>
    public partial class SimpleStat : UserControl, INotifyPropertyChanged
    {

        #region Properties

        string _TextDescription = string.Empty;
        public string TextDescription
        {
            get { return _TextDescription; }
            set
            {
                if (value != _TextDescription)
                {
                    _TextDescription = value;
                    OnPropertyChanged("TextDescription");
                }
            }
        }

        string _CalculatedValue = string.Empty;
        public string CalculatedValue
        {
            get { return _CalculatedValue; }
            set
            {
                if (value != _CalculatedValue)
                {
                    _CalculatedValue = value;
                    OnPropertyChanged("CalculatedValue");
                }
            }
        }

        #endregion

        #region Constructor
        public SimpleStat()
        {
            InitializeComponent();
            this.DataContext = this;

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

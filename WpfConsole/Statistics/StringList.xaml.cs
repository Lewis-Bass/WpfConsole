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

namespace WpfConsole.Statistics
{
    /// <summary>
    /// Interaction logic for StringList.xaml
    /// </summary>
    public partial class StringList : UserControl
    {
        List<string> _Results = new List<string>();
        public List<String> Results
        {
            get { return _Results; }
            set
            {
                _Results = value;
                //DisplayResults.ItemsSource = Results;
                StringBuilder sb = new StringBuilder();
                int cnt = 0;
                foreach (string s in value)
                {
                    cnt++;
                    if (cnt > 100)
                    {
                        sb.Append("...More than 100 Files");
                        break;
                    }

                    sb.Append(s + "\r");
                }
                DisplayResults.Text = sb.ToString();
            }
        }

        public StringList()
        {
            InitializeComponent();

        }
    }
}

using Common.Settings;
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
using System.Windows.Shapes;
using static Themes.Enumerations.ThemeEnums;
using Themes.Helpers;

namespace DialogLibrary.SystemDialogs
{
    /// <summary>
    /// Interaction logic for PurchaseKey.xaml
    /// </summary>
    public partial class PurchaseKey : Window
    {
        public PurchaseKey(string question, string defaultAnswer = "")
        {

            LocalSettings settings = LocalSettings.Load();
            ThemeSelector.ApplyTheme(
                new Uri(ThemeSelector.ThemeEnumToURIString((ETheme)settings.ActiveTheme),
                UriKind.Relative));

            InitializeComponent();

            tbQuestion.Text = question;
            txtAnswer.Text = defaultAnswer;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            IsRedirectAnswer = false;
            this.DialogResult = true;
        }
        private void btnPurchaseWeb_Click(object sender, RoutedEventArgs e)
        {
            IsRedirectAnswer = true;
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtAnswer.SelectAll();
            txtAnswer.Focus();
        }

        public string Answer
        {
            get { return txtAnswer.Text; }
        }

        public bool IsRedirectAnswer { get; private set; } = false;



    }
}

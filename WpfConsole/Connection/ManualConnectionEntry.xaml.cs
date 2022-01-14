using WpfConsole.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WindowsData;


namespace WpfConsole.Connection
{
    /// <summary>
    /// Interaction logic for ManualConnectionEntry.xaml
    /// </summary>
    public partial class ManualConnectionEntry : Window
    {
        private ConnectionInformation _Connection = new ConnectionInformation();
        public ConnectionInformation Connection
        {
            get
            {
                return _Connection;
            }
            set
            {
                if (_Connection != value)
                {
                    _Connection = value;
                    //NameTextBox.Text = _Connection.ConnectionName;
                    AddressTextBox.Text = _Connection.IPAddress;
                    //PortTextBox.Text = _Connection.Port.ToString();
                    //LoginTextBox.Text = _Connection.AccessKeyName;
                    //PasswordTextBox.Password = _Connection.Password;
                }
            }
        }

        public ManualConnectionEntry()
        {
            InitializeComponent();
            this.DataContext = Connection;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;
            StringBuilder sb = new StringBuilder();
            ErrorText.Text = string.Empty;
            //int port;

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                sb.AppendLine(Resource.RequiredName);
                isValid = false;
            }
            else
            {
                //Connection.ConnectionName = NameTextBox.Text;
            }

            if (string.IsNullOrWhiteSpace(AddressTextBox.Text))
            {
                sb.AppendLine(Resource.RequiredAddress);
                isValid = false;
            }
            else
            {
                Connection.IPAddress = AddressTextBox.Text;
            }

            ////////if (string.IsNullOrWhiteSpace(PortTextBox.Text))
            ////////{
            ////////    sb.AppendLine(Resource.RequiredPort);
            ////////    isValid = false;
            ////////}
            ////////else if (!int.TryParse(PortTextBox.Text, out port))
            ////////{
            ////////    sb.AppendLine(Resource.RequiredPortNumber);
            ////////    isValid = false;
            ////////}
            ////////else
            ////////{
            ////////    Connection.Port = port;
            ////////}

            ////////if (string.IsNullOrWhiteSpace(LoginTextBox.Text))
            ////////{
            ////////    sb.AppendLine(Resource.RequiredLogin);
            ////////    isValid = false;
            ////////}
            ////////else
            ////////{
            ////////    Connection.AccessKeyName = LoginTextBox.Text;
            ////////}

            ////////if (string.IsNullOrWhiteSpace(PasswordTextBox.Password))
            ////////{
            ////////    sb.AppendLine(Resource.RequiredPassword);
            ////////    isValid = false;
            ////////}
            ////////else
            ////////{
            ////////    Connection.Password = PasswordTextBox.Password;
            ////////}

            if (isValid)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                ErrorText.Text = sb.ToString();
            }
        }

    }
}

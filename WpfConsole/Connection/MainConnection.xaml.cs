using Common;
using Common.ServerCommunication;
using Common.ServerCommunication.Response;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
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
using Common.ServerCommunication.Helpers;
using WpfConsole.Resources;
using System.ComponentModel;
using WpfConsole.Dialogs;

namespace WpfConsole.Connection
{
    /// <summary>
    /// Interaction logic for ConnectionMain.xaml
    /// </summary>
    public partial class MainConnection : UserControl
    {

        ObservableCollection<ConnectionInformation> _LanAddress;
        public ObservableCollection<ConnectionInformation> LanAddressInfo
        {
            get { return _LanAddress; }
            set
            {
                _LanAddress = value;
                OnPropertyChanged("LanAddressInfo");
            }
        }

        public MainConnection()
        {
            InitializeComponent();

            GetLanAddress();
            this.DataContext = LanAddressInfo;
        }

        /// <summary>
        /// load up the model
        /// </summary>
        private void GetLanAddress()
        {
            var settings = LocalSettings.Load();
            LanAddressInfo = new ObservableCollection<ConnectionInformation>(settings.ConnectionsData);
            lanPrior.ItemsSource = LanAddressInfo; // TODO: Fix this
        }

        #region Routed Event Handlers

        // Create RoutedEvent
        // This creates a static property on the UserControl, ViewFileEvent, which 
        // will be used by the Window, or any control up the Visual Tree, that wants to 
        // handle the event. 
        public static readonly RoutedEvent ConnectionChangedEvent =
            EventManager.RegisterRoutedEvent("ViewFileEvent", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(MainConnection));

        // Create RoutedEventHandler
        // This adds the Custom Routed Event to the WPF Event System and allows it to be 
        // accessed as a property from within xaml if you so desire
        public event RoutedEventHandler ConnectionChanged
        {
            add { AddHandler(ConnectionChangedEvent, value); }
            remove { RemoveHandler(ConnectionChangedEvent, value); }
        }

        #endregion

        #region Button Clicks

        /// <summary>
        /// Enter a new connection info and try it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnManualIP_Click(object sender, RoutedEventArgs e)
        {
            // show the pop window
            var dlg = new LibraryCardConnection();
            dlg.Owner = Application.Current.MainWindow;
            var result = dlg.ShowDialog();

            // did the window get user input
            if (result.HasValue && result.Value)
            {
                LibraryCard card = dlg.LibraryCard;
                var info = new ConnectionInformation()
                {
                    AccessKeyName = card.Name,
                    IPAddress = card.Url
                };

                // output the connections model to the default settings
                var settings = LocalSettings.Load();
                settings.AddConnection(info);

                // connect to the location               
                HandleLogin(info);

                // refresh the display
                GetLanAddress();
            }
        }

        /// <summary>
        /// delete or remove a connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var btn = (Button)sender;
                if (btn.DataContext is ConnectionInformation)
                {
                    var data = (ConnectionInformation)btn.DataContext;
                    var mbox = MessageBox.Show($"{Resource.DeleteMessageBox} {data.AccessKeyName} {Resource.Connection}", Resource.Confirm, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (mbox == MessageBoxResult.Yes)
                    {
                        LanAddressInfo.Remove(data);
                        var settings = LocalSettings.Load();
                        settings.RemoveConnection(data);

                        RaiseEvent(new RoutedEventArgs(ConnectionChangedEvent));
                    }
                }
            }
        }

        /// <summary>
        /// try the connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var btn = (Button)sender;
                if (btn.DataContext is ConnectionInformation)
                {
                    var data = (ConnectionInformation)btn.DataContext;
                    HandleLogin(data);
                }
            }
        }

        #endregion

        #region helpers

        /// <summary>
        /// Try the connection 
        /// </summary>
        /// <param name="data"></param>
        private void HandleLogin(ConnectionInformation data)
        {
            // do we need to get a PIN code?
            string pin = GetPinValue.GetPinValueFromUser(data);

            var response = AccountHelpers.Login(data, pin);

            data.IsCurrentConnection = response.IsLoginValid;

            // do we have a valid connection?
            string msg = string.Empty;
            if (response.IsLoginValid)
            {
                GlobalValues.LastConnection = data;
                GlobalValues.ConnectionToken = response.ConnectionToken;
                msg = Resource.LoginSuccessful;
            }
            else
            {
                GlobalValues.LastConnection = null;
                GlobalValues.ConnectionToken = null; ;
                msg = Resource.LoginFailed;
            }

            MessageBox.Show(msg, Resource.LoginName, MessageBoxButton.OK, MessageBoxImage.Asterisk);


            // tell the world..
            RaiseEvent(new RoutedEventArgs(ConnectionChangedEvent));
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

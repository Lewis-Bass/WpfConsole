using Common.ConnectionInfo;
using Common.Settings;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WindowsData;
using WpfConsole.Resources;

namespace WpfConsole.Connection
{
    /// <summary>
    /// Interaction logic for ConnectionMain.xaml
    /// </summary>
    public partial class MainConnection : UserControl
    {
        #region Globals

        ObservableCollection<ConnectionInformation> _LanAddresInfo;
        public ObservableCollection<ConnectionInformation> LanAddressInfo
        {
            get { return _LanAddresInfo; }
            set
            {
                _LanAddresInfo = value;
                OnPropertyChanged("LanAddressInfo");
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
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
            var conHelper = new ConnectionHelper();
            LanAddressInfo = new ObservableCollection<ConnectionInformation>(conHelper.GetAllConnections());
            lanPrior.ItemsSource = LanAddressInfo;
        }

        #endregion

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

        #region Helpers

        /// <summary>
        /// Try the connection 
        /// </summary>
        /// <param name="data"></param>
        private void HandleLogin(ConnectionInformation data)
        {
            var helper = new Helpers.LoginHelper();
            helper.ProcessLogin(data);
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

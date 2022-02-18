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

namespace WpfConsole.SearchMaster
{
    /// <summary>
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class MainSearch : UserControl
    {
        public MainSearch()
        {
            InitializeComponent();
            Filter.DisplayResult = Results;
            Previous.DisplayResult = Results;
            Advanced.DisplayResult = Results;

            AddHandler(MainSearch.PerformSearchEvent,
               new RoutedEventHandler(HandlePerformSearch));
        }

        #region Events

        // Create RoutedEvent
        // This creates a static property on the UserControl, ViewFileEvent, which 
        // will be used by the Window, or any control up the Visual Tree, that wants to 
        // handle the event. 
        public static readonly RoutedEvent PerformSearchEvent =
            EventManager.RegisterRoutedEvent("PerformSearch", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(MainSearch));

        // Create RoutedEventHandler
        // This adds the Custom Routed Event to the WPF Event System and allows it to be 
        // accessed as a property from within xaml if you so desire
        public event RoutedEventHandler PerformSearch
        {
            add { AddHandler(PerformSearchEvent, value); }
            remove { RemoveHandler(PerformSearchEvent, value); }
        }

        // Create RoutedEvent
        // This creates a static property on the UserControl, ViewFileEvent, which 
        // will be used by the Window, or any control up the Visual Tree, that wants to 
        // handle the event. 
        public static readonly RoutedEvent ViewFileEvent =
            EventManager.RegisterRoutedEvent("ViewFileEvent", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(MainSearch));

        // Create RoutedEventHandler
        // This adds the Custom Routed Event to the WPF Event System and allows it to be 
        // accessed as a property from within xaml if you so desire
        public event RoutedEventHandler ViewFile
        {
            add { AddHandler(ViewFileEvent, value); }
            remove { RemoveHandler(ViewFileEvent, value); }
        }


        // Create RoutedEvent
        // This creates a static property on the UserControl, CheckOutFileEvent, which 
        // will be used by the Window, or any control up the Visual Tree, that wants to 
        // handle the event. 
        public static readonly RoutedEvent CheckOutFileEvent =
            EventManager.RegisterRoutedEvent("CheckOutFileEvent", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(MainSearch));

        // Create RoutedEventHandler
        // This adds the Custom Routed Event to the WPF Event System and allows it to be 
        // accessed as a property from within xaml if you so desire
        public event RoutedEventHandler CheckOutFile
        {
            add { AddHandler(CheckOutFileEvent, value); }
            remove { RemoveHandler(CheckOutFileEvent, value); }
        }

        #endregion

        /// <summary>
        /// Handle the event from the MainConnection user control - a new connection was just established
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandlePerformSearch(object sender, RoutedEventArgs e)
        {
            List<SearchCriteriaBase> criteria = e.OriginalSource as List<SearchCriteriaBase>;
            MainTabControl.SelectedIndex = 3;            
            Results.PerformSearch(criteria);
        }
    }
}

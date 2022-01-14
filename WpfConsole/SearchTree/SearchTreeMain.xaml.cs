using Common;
using Common.ServerCommunication.Helpers;
using Common.ServerCommunication.Requests;
using Microsoft.Win32;
using WpfConsole.Resources;
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


namespace WpfConsole.SearchTree
{
	/// <summary>
	/// Interaction logic for SearchTreeMain.xaml
	/// </summary>
	public partial class SearchTreeMain : UserControl
	{
		#region Constructors

		public SearchTreeMain()
		{
			InitializeComponent();

			DataContext = this;

			LoadBinderList();
			LoadCriteria();
			cbSelect_Click(null, null);
		}

		#endregion

		#region Binders - previous searches

		#region Properties

		ObservableCollection<BinderInformation> _BinderList = new ObservableCollection<BinderInformation>();
		/// <summary>
		/// Collection of binders to show on the screen
		/// </summary>
		public ObservableCollection<BinderInformation> BinderList {
			get { return _BinderList; }
			set {
				if (BinderList != value)
				{
					_BinderList = value;
					OnPropertyChanged("BinderList");
				}
			}
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
			LoadSelectedBinder((BinderInformation)DocumentList.SelectedItem);
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

		/// <summary>
		/// Process the selection changed
		/// </summary>
		/// <param name="binder"></param>
		private void LoadSelectedBinder(BinderInformation binder)
		{

			var newCriteria = BinderHelpers.GetBinderSearchCriteria(GlobalValues.LastConnection, binder.BinderID);

			// convert the base to the gui model
			var newGui = newCriteria.Select(r => new SearchTreeGUI
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

			// update the display...
			LoadCriteria();
			SearchCriteriaInfo = new ObservableCollection<SearchTreeGUI>(newGui);
			BindCriteraToControl();

			// switch the tab
			MainTabs.SelectedIndex = 1;
		}

		#endregion

		#endregion

		#region Search

		#region Properties

		/// <summary>
		/// Search criteria 
		/// </summary>
		public ObservableCollection<SearchTreeGUI> SearchCriteriaInfo {
			get {
				return _SearchCriteriaInfo;

			}
			set {
				_SearchCriteriaInfo = value;
				OnPropertyChanged("SearchCriteriaInfo");
			}
		}

		ObservableCollection<SearchTreeGUI> _SearchCriteriaInfo = new ObservableCollection<SearchTreeGUI>();

		#endregion

		#region initial load

		private void LoadCriteria()
		{
			SearchCriteriaInfo = new ObservableCollection<SearchTreeGUI> {
				new SearchTreeGUI()
			};
			SetFirstRecordRelationships();
			BindCriteraToControl();
		}

		private void BindCriteraToControl()
		{
			CriteriaTree.Items.Clear();
			SetFirstRecordRelationships();
			foreach (var rec in SearchCriteriaInfo)
			{
				CriteriaTree.Items.Add(rec);
			}
		}
		
		#endregion

		#region Button Clicks

		/// <summary>
		///  remove the search criteria from the screen
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RemoveCriteria_Click(object sender, RoutedEventArgs e)
		{
			var button = (Button)sender;
			if (button.DataContext is WindowsData.SearchTreeGUI)
			{
				var data = (WindowsData.SearchTreeGUI)button.DataContext;
				bool isEntryRemoved = false;
				var criteria = RecursiveRemoveCriteria(SearchCriteriaInfo.ToList(), data, out isEntryRemoved);

				// add a blank back if needed
				if (criteria.Count <= 0)
				{
					var rec = new SearchTreeGUI();
					criteria.Add(rec);
				}
				SearchCriteriaInfo = new ObservableCollection<SearchTreeGUI>(criteria);
				BindCriteraToControl();
			}
		}

		private List<SearchTreeGUI> RecursiveRemoveCriteria(List<SearchTreeGUI> searchMe, SearchTreeGUI removeMe, out bool isEntryRemoved)
		{
			isEntryRemoved = false;
			for (int i = 0; i < searchMe.Count(); i++)
			{
				if (searchMe[i] == removeMe)
				{
					searchMe.Remove(removeMe);
					isEntryRemoved = true;
					break;
				}
				else
				{
					// recursive call to search sub queryies..
					if (searchMe[i].SubQuery.Count() > 0)
					{
						var recs = RecursiveRemoveCriteria(searchMe[i].SubQuery.ToList(), removeMe, out isEntryRemoved);
						if (isEntryRemoved)
						{
							searchMe[i].SubQuery = new ObservableCollection<SearchTreeGUI>(recs);
							break;
						}
					}
				}
			}

			return searchMe;
		}

		/// <summary>
		/// add a new search criteria to the screen
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddCriteria_Click(object sender, RoutedEventArgs e)
		{
			var button = (Button)sender;
			if (button.DataContext is WindowsData.SearchTreeGUI)
			{
				var data = (WindowsData.SearchTreeGUI)button.DataContext;
				bool isEntryAdded;
				var criteria = RecursiveAddCriteria(SearchCriteriaInfo.ToList(), data, out isEntryAdded);

				SearchCriteriaInfo = new ObservableCollection<SearchTreeGUI>(criteria);
				BindCriteraToControl();
			}
		}
		
		private List<SearchTreeGUI> RecursiveAddCriteria(List<SearchTreeGUI> searchMe, SearchTreeGUI addAfterMe, out bool isEntryAdded)
		{
			isEntryAdded = false;
			for (int i = 0; i < searchMe.Count(); i++)
			{
				if (searchMe[i] == addAfterMe)
				{
					searchMe.Insert(i+1,new SearchTreeGUI());
					isEntryAdded = true;
					break;
				}
				else
				{
					// recursive call to search sub queryies..
					if (searchMe[i].SubQuery.Count() > 0)
					{
						var recs = RecursiveAddCriteria(searchMe[i].SubQuery.ToList(), addAfterMe, out isEntryAdded);
						if (isEntryAdded)
						{
							searchMe[i].SubQuery = new ObservableCollection<SearchTreeGUI>(recs);
							break;
						}
					}
				}
			}
			return searchMe;
		}

		/// <summary>
		/// add to an existing sub group or create a new one
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddGroup_Click(object sender, RoutedEventArgs e)
		{
			var button = (Button)sender;
			if (button.DataContext is WindowsData.SearchTreeGUI)
			{
				var data = (WindowsData.SearchTreeGUI)button.DataContext;
				if (data.SubQuery == null)
				{
					data.SubQuery = new ObservableCollection<SearchTreeGUI>();
				}
				var rec = new SearchTreeGUI();
				rec.ShowGroupRelationship = true;
				rec.ShowRelationship = false;
				data.SubQuery.Add(rec);
				BindCriteraToControl();
			}
		}

		private void SetFirstRecordRelationships()
		{
			SearchCriteriaInfo.First().ShowGroupRelationship = false;
			SearchCriteriaInfo.First().ShowRelationship = false;
		}

		/// <summary>
		/// a line was selected or unselected to become part of a group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbSelect_Click(object sender, RoutedEventArgs e)
		{
			//AddGroup.IsEnabled = SearchCriteriaInfo.Any(r => r.Select);
		}

		#endregion

		#endregion


		#region Results

		#region Properties

		/// <summary>
		/// Results of the last executed search
		/// </summary>
		public ObservableCollection<SearchResults> SearchResultsInfo { get; set; } = new ObservableCollection<SearchResults>();

		#endregion

		//////#region initial load

		//////private void LoadCriteria()
		//////{
		//////    SearchCriteriaInfo = new ObservableCollection<SearchCriteriaGUI> {
		//////        new SearchCriteriaGUI()
		//////    };

		//////}

		//////private void LoadBinderList()
		//////{
		//////    var request = new RequestBinderList
		//////    {
		//////        Connection = GlobalValues.LastConnection,
		//////        ConnectionToken = GlobalValues.ConnectionToken,
		//////        MaxEntries = 1000
		//////    };
		//////    var values = BinderHelpers.GetBinderList(request);
		//////    if (values.Any())
		//////    {
		//////        values.Add(new BinderInformation());
		//////        BinderList = new ObservableCollection<BinderInformation>(values.OrderBy(r => r.BinderName));
		//////    }
		//////}
		//////#endregion

		#region Routed Event Handlers

		////////////////// Create RoutedEvent
		////////////////// This creates a static property on the UserControl, ViewFileEvent, which 
		////////////////// will be used by the Window, or any control up the Visual Tree, that wants to 
		////////////////// handle the event. 
		////////////////public static readonly RoutedEvent ViewFileEvent =
		////////////////    EventManager.RegisterRoutedEvent("ViewFileEvent", RoutingStrategy.Bubble,
		////////////////    typeof(RoutedEventHandler), typeof(MainSearch));

		////////////////// Create RoutedEventHandler
		////////////////// This adds the Custom Routed Event to the WPF Event System and allows it to be 
		////////////////// accessed as a property from within xaml if you so desire
		////////////////public event RoutedEventHandler ViewFile
		////////////////{
		////////////////    add { AddHandler(ViewFileEvent, value); }
		////////////////    remove { RemoveHandler(ViewFileEvent, value); }
		////////////////}


		////////////////// Create RoutedEvent
		////////////////// This creates a static property on the UserControl, CheckOutFileEvent, which 
		////////////////// will be used by the Window, or any control up the Visual Tree, that wants to 
		////////////////// handle the event. 
		////////////////public static readonly RoutedEvent CheckOutFileEvent =
		////////////////    EventManager.RegisterRoutedEvent("CheckOutFileEvent", RoutingStrategy.Bubble,
		////////////////    typeof(RoutedEventHandler), typeof(MainSearch));

		////////////////// Create RoutedEventHandler
		////////////////// This adds the Custom Routed Event to the WPF Event System and allows it to be 
		////////////////// accessed as a property from within xaml if you so desire
		////////////////public event RoutedEventHandler CheckOutFile
		////////////////{
		////////////////    add { AddHandler(CheckOutFileEvent, value); }
		////////////////    remove { RemoveHandler(CheckOutFileEvent, value); }
		////////////////}


		#endregion

		#region Search

		/// <summary>
		/// Execute the search
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Execute_Click(object sender, RoutedEventArgs e)
		{
			BinderInformation binder = null;
			if (DocumentList.SelectedIndex >= 0)
			{
				binder = (BinderInformation)DocumentList.SelectedItem;
			}
			GoToWebSiteSearch(binder, false);
		}

		private void ExecuteSave_Click(object sender, RoutedEventArgs e)
		{
			// get a name for the search
			Dialogs.UserInput userInput = new Dialogs.UserInput(Resource.SearchSaveName, "");
			if (userInput.ShowDialog() == true)
			{
				BinderInformation binder = new BinderInformation { BinderID = string.Empty, BinderName = userInput.Answer };
				GoToWebSiteSearch(binder, true);
			}
		}

		private void GoToWebSiteSearch(BinderInformation binder, bool keepResults)
		{
			var request = new RequestSearch
			{
				Connection = GlobalValues.LastConnection,
				ConnectionToken = GlobalValues.ConnectionToken,
				BinderName = (binder != null) ? binder.BinderName : string.Empty,
				BinderID = (binder != null) ? binder.BinderID : string.Empty,
				StartingEntry = 0,
				UpdateBinder = keepResults,
				Search = SearchCriteriaInfo.Select(r => new SearchCriteriaBase
				{
					Criteria = r.Criteria,
					Field = r.Field,
					Relationship = r.Relationship,
					ValueBool = r.ValueBool,
					ValueMax = r.ValueMax,
					ValueMaxDate = r.ValueMaxDate,
					ValueMin = r.ValueMin,
					ValueMinDate = r.ValueMinDate
				}).ToList()
			};

			var criteria = SearchHelpers.ProcessSearch(request);
			SearchResultsInfo = new ObservableCollection<SearchResults>(criteria);
			DisplayResults.ItemsSource = SearchResultsInfo;
			MainTabs.SelectedIndex = 2;
		}

		#endregion

		//#region Results Show
		//private void HideDetails_Click(object sender, RoutedEventArgs e)
		//{
		//    var button = (Button)sender;
		//    if (button.DataContext is SearchResults)
		//    {
		//        var data = (SearchResults)button.DataContext;
		//        data.IsDetailsShown = true;
		//    }
		//}

		//private void ShowDetails_Click(object sender, RoutedEventArgs e)
		//{
		//    var button = (Button)sender;
		//    if (button.DataContext is SearchResults)
		//    {
		//        var data = (SearchResults)button.DataContext;
		//        data.IsDetailsShown = false;
		//    }
		//}
		//#endregion

		////////#region Get Binder details

		/////////// <summary>
		/////////// event for when loading a default binder
		/////////// </summary>
		/////////// <param name="sender"></param>
		/////////// <param name="e"></param>
		////////private void cbBinderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		////////{
		////////    if (sender is ComboBox)
		////////    {
		////////        var cb = (ComboBox)sender;
		////////        if (cb.SelectedItem != null && cb.SelectedItem is BinderInformation)
		////////        {
		////////            var data = (BinderInformation)cb.SelectedItem;

		////////            var newCriteria = BinderHelpers.GetBinderSearchCriteria(GlobalValues.LastConnection, data.BinderID);

		////////            // convert the base to the gui model
		////////            var newGui = newCriteria.Select(r => new SearchCriteriaGUI
		////////            {
		////////                Field = r.Field,
		////////                Criteria = r.Criteria,
		////////                ValueMin = r.ValueMin,
		////////                ValueMax = r.ValueMax,
		////////                ValueBool = r.ValueBool,
		////////                ValueMinDate = r.ValueMinDate,
		////////                ValueMaxDate = r.ValueMaxDate
		////////            });

		////////            // update the display...
		////////            SearchCriteriaInfo = new ObservableCollection<SearchCriteriaGUI>(newGui);
		////////            UserSearchInput.ItemsSource = SearchCriteriaInfo;
		////////            //if (newGui.Any())
		////////            //{
		////////            //    Relationship = newGui.First().Relationship;
		////////            //}
		////////            //else
		////////            //{
		////////            //    Relationship = "&";
		////////            //}
		////////        }
		////////    }
		////////}

		////////#endregion

		#region View File

		private void View_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button)
			{
				var button = (Button)sender;
				if (button.DataContext != null && button.DataContext is SearchResults)
				{
					var searchResult = (SearchResults)button.DataContext;

					var file = new LocalFileStatus
					{
						//VaultID = searchResult.
						DocumentName = searchResult.DocumentName,
						IsCheckedOut = false,
						DateRecieved = DateTime.Now
					};

					// ask the user where the file is to be placed
					var saveFileDialog = new SaveFileDialog();
					saveFileDialog.OverwritePrompt = true;
					saveFileDialog.CheckPathExists = true;
					saveFileDialog.FileName = searchResult.DocumentName;
					if (saveFileDialog.ShowDialog() == true)
					{
						file.LocalFileLocation = saveFileDialog.FileName;
						var localFiles = new Common.LocalFiles();
						localFiles.ViewFile(file);
						//RaiseEvent(new RoutedEventArgs(ViewFileEvent));
					}

				}
			}

		}

		#endregion

		#region Checkout file
		private void CheckOut_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button)
			{
				var button = (Button)sender;
				if (button.DataContext != null && button.DataContext is SearchResults)
				{
					var searchResult = (SearchResults)button.DataContext;

					var file = new LocalFileStatus
					{
						//VaultID = searchResult.
						DocumentName = searchResult.DocumentName,
						IsCheckedOut = true,
						DateRecieved = DateTime.Now
					};

					// ask the user where the file is to be placed
					var saveFileDialog = new SaveFileDialog();
					saveFileDialog.OverwritePrompt = true;
					saveFileDialog.CheckPathExists = true;
					saveFileDialog.FileName = searchResult.DocumentName;
					if (saveFileDialog.ShowDialog() == true)
					{
						file.LocalFileLocation = saveFileDialog.FileName;
						var localFiles = new Common.LocalFiles();
						localFiles.CheckOutFile(file);
						//RaiseEvent(new RoutedEventArgs(CheckOutFileEvent));
					}
				}
			}

		}

		#endregion

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

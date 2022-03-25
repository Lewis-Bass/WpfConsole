using Common.ServerCommunication.Helpers;
using Common.ServerCommunication.Requests;
using Common;
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
using WpfConsole.Resources;
using WpfConsole.Dialogs;

namespace WpfConsole.SearchMaster
{
	/// <summary>
	/// Interaction logic for Results.xaml
	/// </summary>
	public partial class Results : UserControl
	{

		#region Properties

		List<SearchCriteriaBase> Criterias;

		ObservableCollection<BinderInformation> _BinderList = new ObservableCollection<BinderInformation>();

		public ObservableCollection<SearchResults> SearchResultsInfo { get; set; } = new ObservableCollection<SearchResults>();

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

		public Results()
		{
			InitializeComponent();
			this.DataContext = this;
		}

		#endregion

		#region External Event

		/// <summary>
		/// update the screen display with the results of the search
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		public bool PerformSearch (List<SearchCriteriaBase> criteria)
		{
			Criterias = criteria;
			BinderInformation binder = null;
			GoToWebSiteSearch(binder, false);
			return true;
		}

		#endregion

		#region Search buttons

		private void ExecuteSave_Click(object sender, RoutedEventArgs e)
		{
			// get a name for the search
			Dialogs.UserInput userInput = new Dialogs.UserInput(Resource.SearchSaveName, "");
			if (userInput.ShowDialog() == true)
			{
				BinderInformation binder = new BinderInformation { BinderID = string.Empty, BinderName = userInput.Answer };
				GoToWebSiteSearch(binder, true); // TODO: The query has already been run - maybe we just have to save the name
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
				Search = Criterias
			};

			var criteria = SearchHelpers.ProcessSearch(request);
			SearchResultsInfo = new ObservableCollection<SearchResults>(criteria);
			DisplayResults.ItemsSource = SearchResultsInfo;
		}

		/// <summary>
		/// Change the tags associated with the document
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TagChange_Click(object sender, RoutedEventArgs e)
		{
			var btn = sender as Button;
			// show the tag change window
			var dlg = new TagChange();
			dlg.Owner = Application.Current.MainWindow;
			dlg.ResultInfo = (SearchResults)btn.DataContext;
			dlg.ShowDialog();

			// update the display source with the new tag values
			foreach(var rec in SearchResultsInfo)
			{
				if (rec.DocumentName == dlg.ResultInfo.DocumentName)
				{
					rec.Tags = new ObservableCollection<MetaTags>(dlg.ResultInfo.Tags);
				}
			}
			OnPropertyChanged("SearchResultsInfo");
		}

		#endregion

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
					var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
					saveFileDialog.OverwritePrompt = true;
					saveFileDialog.CheckPathExists = true;
					saveFileDialog.FileName = searchResult.DocumentName;
					if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						file.LocalFileLocation = saveFileDialog.FileName;
						var localFiles = new Common.LocalFiles();
						RaiseEvent(new RoutedEventArgs(MainSearch.ViewFileEvent, null));
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
					var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
					saveFileDialog.OverwritePrompt = true;
					saveFileDialog.CheckPathExists = true;
					saveFileDialog.FileName = searchResult.DocumentName;
					if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						file.LocalFileLocation = saveFileDialog.FileName;
						var localFiles = new Common.LocalFiles();
						localFiles.CheckOutFile(file);
						RaiseEvent(new RoutedEventArgs(MainSearch.CheckOutFileEvent, null));
					}
				}
			}
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

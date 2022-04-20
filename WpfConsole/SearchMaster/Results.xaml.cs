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
	public partial class Results : UserControl, INotifyPropertyChanged
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
			DisplayResults.SearchResultsInfo = SearchResultsInfo;
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

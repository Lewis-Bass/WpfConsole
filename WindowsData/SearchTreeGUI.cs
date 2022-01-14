using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsData
{
	public class SearchTreeGUI : SearchCriteriaBase, INotifyPropertyChanged
	{

		#region Constructor

		public SearchTreeGUI() { }

		#endregion

		#region Binding Sources

		public Dictionary<string, string> SearchFields {
			get {
				return new Dictionary<string, string>
				{

					{ "Path", Resources.Resource.Path},
					{ "FileName", Resources.Resource.FileName },
					{ "Extension", Resources.Resource.Extension },
					{ "Date", Resources.Resource.Date },
					{ "Locked", Resources.Resource.Locked},
					{ "Prior Versions", Resources.Resource.Prior_Versions },

				};
			}
		}

		public Dictionary<string, string> SearchHow {
			get {
				return new Dictionary<string, string>
				{
					{ "=", Resources.Resource.Equal},
					{ "<", Resources.Resource.LessThan},
					{ "<=", Resources.Resource.LessThanEqual},
					{ ">", Resources.Resource.GreaterThan},
					{ ">=", Resources.Resource.GreaterThanEqual},
					{ "<>", Resources.Resource.NotEqual},
					{ "Range", Resources.Resource.Range},
					{ "Matches", Resources.Resource.Matches},
                    //{ "List", "List"},
                };
			}
		}

		//public Dictionary<string, string> RelationshipFields
		//{
		//    get
		//    {
		//        return new Dictionary<string, string>
		//        {
		//            { "&", "/Resources/VenAnd100-71.png"},
		//            { "^", "/Resources/VenOr100-71.png"},
		//            { "!", "/Resources/VenNot100-71.png"},
		//        };
		//    }
		//}

		public List<RelationshipCriteria> RelationshipFields {
			get {
				return new List<RelationshipCriteria>
				{
					new RelationshipCriteria
					{
						Key = "&",
						Value ="/Resources/VenAnd100-71.png",
						ToolTip = Resources.Resource.RelationshipAndHelp
					},
					new RelationshipCriteria
					{
						Key = "^",
						Value ="/Resources/VenOr100-71.png",
						ToolTip = Resources.Resource.RelationshipOrHelp
					}
				};
			}
		}

		public List<RelationshipCriteria> GroupRelationshipFields {
			get {
				return new List<RelationshipCriteria>
				{
					new RelationshipCriteria
					{
						Key = "&",
						Value ="/Resources/GroupAnd100-71.png",
						ToolTip = Resources.Resource.RelationshipAndHelp
					},
					new RelationshipCriteria
					{
						Key = "^",
						Value ="/Resources/GroupOr100-71.png",
						ToolTip = Resources.Resource.RelationshipOrHelp
					}
				};
			}
		}

		#endregion

		#region GUI only properties
			
		public bool ShowValueMin {
			get {
				return (Field != "Locked" && Field != "Prior Versions" && Field != "Date");
			}
		}

		public bool ShowValueMax {
			get {
				return (Criteria == "Range" && Field != "Locked" && Field != "Prior Versions" && Field != "Date");
			}
		}

		public bool ShowValueBool {
			get {
				return (Field == "Locked" || Field == "Prior Versions");
			}
		}

		public bool ShowCriteria {
			get {
				return (Field != "Locked" && Field != "Prior Versions");
			}
		}

		public bool ShowValueMinDate {
			get {
				return (Field == "Date");
			}
		}

		public bool ShowValueMaxDate {
			get {
				return (Field == "Date" && Criteria == "Range");
			}
		}

		public bool ShowTo {
			get {
				return (Criteria == "Range");
			}
		}

		public bool ShowGroupBorder {
			get {
				return (GroupID > 0);
			}
		}

		bool _showRelationship = true;
		public bool ShowRelationship {
			get { return _showRelationship; }
			set {
				if (value != _showRelationship)
				{
					_showRelationship = value;
					OnPropertyChanged("ShowRelationship");
				}
			}
		}

		bool _showGroupRelationship = false;
		public bool ShowGroupRelationship {
			get { return _showGroupRelationship; }
			set {
				if (value != _showGroupRelationship)
				{
					_showGroupRelationship = value;
					OnPropertyChanged("ShowGroupRelationship");
				}
			}
		}

		public ObservableCollection<SearchTreeGUI> SubQuery { get; set; } = new ObservableCollection<SearchTreeGUI>();

		#endregion

		#region Input Fields

		bool _select = false;
		/// <summary>
		/// Is the row currently selected
		/// </summary>
		public bool Select {
			get { return _select; }
			set {
				if (value != _select)
				{
					_select = value;
					OnPropertyChanged("Select");
				}
			}
		}

		string _groupColor = "Transparent";
		public string GroupColor {
			get { return _groupColor; }
			set {
				if (value != _groupColor)
				{
					_groupColor = value;
					OnPropertyChanged("GroupColor");
				}
			}
		}
		#endregion

		#region Helper Methods

		/// <summary>
		/// raise additional events that are found in the GUI
		/// </summary>
		public override void NotifyShows()
		{
			OnPropertyChanged("ShowValueMin");
			OnPropertyChanged("ShowValueMax");
			OnPropertyChanged("ShowValueBool");
			OnPropertyChanged("ShowCriteria");
			OnPropertyChanged("ShowValueMinDate");
			OnPropertyChanged("ShowValueMaxDate");
			OnPropertyChanged("ShowTo");
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsData
{
    public class FileTags : INotifyPropertyChanged
    {

		string _TagId = string.Empty;
		/// <summary>
		/// Unique Identifier for the tag in the vault
		/// </summary>
        public string TagId
		{
			get { return _TagId; }
			set
			{
				if (_TagId != value)
				{
					_TagId = value;
					OnPropertyChanged("TagId");
				}
			}
		}

		string _TagName= string.Empty;
		/// <summary>
		/// Name of the tag
		/// </summary>
        public string TagName
		{
			get { return _TagName; }
			set
			{
				if (_TagName != value)
				{
					_TagName = value;
					OnPropertyChanged("TagName");
				}
			}
		}

		//////////////string _RenameValue = string.Empty;
		///////////////// <summary>
		///////////////// GUI ONLY - Rename the tag to be this value
		///////////////// </summary>
		//////////////public string RenameValue
		//////////////{
		//////////////	get { return _RenameValue; }
		//////////////	set
		//////////////	{
		//////////////		if (_RenameValue != value)
		//////////////		{
		//////////////			_RenameValue = value;
		//////////////			OnPropertyChanged("RenameValue");
		//////////////		}
		//////////////	}
		//////////////}

		int _TotalTagUsage = 0;
		/// <summary>
		/// How Many documents are tagged with this value
		/// </summary>
		public int TotalTagUsage
		{
			get { return _TotalTagUsage; }
			set
			{
				if (_TotalTagUsage != value)
				{
					_TotalTagUsage = value;
					OnPropertyChanged("TotalTagUsage");
				}
			}
		}

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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsData
{
	public class AccessKey : INotifyPropertyChanged
	{

		string _KeyId;
		/// <summary>
		/// unique key - identifier for the back end
		/// </summary>
		public string KeyId
		{
			get { return _KeyId; }
			set
			{
				if (_KeyId != value)
				{
					_KeyId = value;
					OnPropertyChanged("KeyId");
				}
			}
		}

		string _KeyName;
		/// <summary>
		/// name of the key - this should be unique
		/// </summary>
		public string KeyName
		{
			get { return _KeyName; }
			set
			{
				if (_KeyName != value)
				{
					_KeyName = value;
					OnPropertyChanged("KeyName");
				}
			}
		}

		int _PIN;
		/// <summary>
		/// the user is prompted for this value when using the key
		/// </summary>
		public int PIN
		{
			get { return _PIN; }
			set
			{
				if (_PIN != value)
				{
					_PIN = value;
					OnPropertyChanged("PIN");
				}
			}
		}

		bool _CanCheckInOut;
		/// <summary>
		/// can the user check in or out files
		/// </summary>
		public bool CanCheckInOut
		{
			get { return _CanCheckInOut; }
			set
			{
				if (_CanCheckInOut != value)
				{
					_CanCheckInOut = value;
					OnPropertyChanged("CanCheckInOut");
				}
			}
		}

		bool _CanView;
		/// <summary>
		/// can the user view the files
		/// </summary>
		public bool CanView
		{
			get { return _CanView; }
			set
			{
				if (_CanView != value)
				{
					_CanView = value;
					OnPropertyChanged("CanView");
				}
			}
		}

		bool _CanAdmin;
		/// <summary>
		/// can the user access other admin calls
		/// </summary>
		public bool CanAdmin {
			get { return _CanAdmin; }
			set
			{
				if (_CanAdmin != value)
				{
					_CanAdmin = value;
					OnPropertyChanged("CanAdmin");
				}
			}
		}


		bool _CanSubmit;
		/// <summary>
		/// can the user submit files 
		/// </summary>
		public bool CanSubmit
		{
			get { return _CanSubmit; }
			set
			{
				if (_CanSubmit != value)
				{
					_CanSubmit = value;
					OnPropertyChanged("CanSubmit");
				}
			}
		}

		List<string> _Devices;
		/// <summary>
		/// Device names that have installed the key
		/// </summary>
		public List<string> Devices
		{
			get { return _Devices; }
			set
			{
				if (_Devices != value)
				{
					_Devices = value;
					OnPropertyChanged("Devices");
				}
			}
		}

		/// <summary>
		/// for GUI display
		/// </summary>
		public string DevicesString
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				if (Devices != null && Devices.Count() > 0)
					foreach (var dev in Devices)
					{
						sb.AppendLine(dev);
					}
				return sb.ToString();
			}
		}

		DateTime _ExpirationDate;
		/// <summary>
		/// Date that the key will expire
		/// </summary>
		public DateTime ExpirationDate
		{
			get { return _ExpirationDate; }
			set
			{
				if (_ExpirationDate != value)
				{
					_ExpirationDate = value;
					OnPropertyChanged("ExpirationDate");
				}
			}
		}

		/// <summary>
		/// string value for the GUI
		/// </summary>
		public string ExpirationDateString
		{
			get
			{
				return ExpirationDate.ToShortDateString();
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


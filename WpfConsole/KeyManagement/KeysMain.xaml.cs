using Common;
using Common.MailSend;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsData;
using WpfConsole.Connection;
using WpfConsole.Resources;

namespace WpfConsole.KeyManagement
{
	/// <summary>
	/// Interaction logic for KeysMain.xaml
	/// </summary>
	public partial class KeysMain : UserControl
	{

		#region Globals and Constructors

		ObservableCollection<AccessKey> _ExistingKeyInformation;
		private ObservableCollection<AccessKey> ExistingKeyInformation
		{
			get { return _ExistingKeyInformation; }
			set
			{
				if (_ExistingKeyInformation != value)
				{
					_ExistingKeyInformation = value;
					OnPropertyChanged("ExistingKeyInformation");
				}
			}
		}
		public KeysMain()
		{
			InitializeComponent();

			DataContext = this;

			LoadKeys();
		}

		#endregion

		#region Button Clicks

		private void BtnAddNew_Click(object sender, RoutedEventArgs e)
		{
			var k = new AccessKey()
			{
				KeyId = Guid.NewGuid().ToString(),
				CanCheckInOut = false,
				CanSubmit = true,
				CanView = true,
				ExpirationDate = DateTime.Now.AddYears(10),
				PIN = 12345
			};
			AddUpdateKey(k);

			BtnDownload_Click(sender, e);
		}

		private void BtnUpdate_Click(object sender, RoutedEventArgs e)
		{
			var tb = (Button)sender;
			AddUpdateKey((AccessKey)tb.DataContext);
		}

		private void BtnDownload_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button)
			{
				var btn = (Button)sender;
				if (btn.DataContext is AccessKey)
				{
					var key = (AccessKey)btn.DataContext;

					string cardString = CreateCard(key);

					var saveFileDialog = new SaveFileDialog()
					{
						CheckFileExists = false,
						CheckPathExists = true,
						AddExtension = true,
						Filter = "Library Card|*.Card"
					};

					if (saveFileDialog.ShowDialog() == true)
					{
						string fileName = saveFileDialog.FileName;
						if (!string.IsNullOrWhiteSpace(fileName))
						{
							if (File.Exists(fileName))
							{
								File.Delete(fileName);
							}
							File.WriteAllText(fileName, cardString);
						}
					}
				}
			}
		}

		private void BtnEmail_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button)
			{
				var btn = (Button)sender;
				if (btn.DataContext is AccessKey)
				{
					// get the address of who to send the email to
					Dialogs.UserInput userInput = new Dialogs.UserInput(Resource.EmailEnterText, "");
					if (userInput.ShowDialog() != true)
					{
						return;
					}

					string emailTo = userInput.Answer;

					var key = (AccessKey)btn.DataContext;

					string cardString = CreateCard(key);

					int port = 0;
					if (!int.TryParse(WpfConsole.Resources.Resource.EmailHostPort, out port))
					{
						Serilog.Log.Error($"Bad Configuration EmailHostPort{WpfConsole.Resources.Resource.EmailHostPort}");
						return;
					}
					var send = new Send(
						WpfConsole.Resources.Resource.EmailHost,
						WpfConsole.Resources.Resource.EmailHostLogin,
						WpfConsole.Resources.Resource.EmailHostPassword,
						port,
						WpfConsole.Resources.Resource.EmailFromAddress,
						WpfConsole.Resources.Resource.EmailFromName);
					send.Attachments = new Dictionary<string, byte[]>();
					send.Attachments.Add(key.KeyName.Replace(" ", ""), Encoding.ASCII.GetBytes(cardString));
					send.Body = string.Format(WpfConsole.Resources.Resource.EmailBody, cardString);
					send.Subject = WpfConsole.Resources.Resource.EmailCardSubject;
					send.SendEmail(new string[] { emailTo });

				}
			}
		}

		private void BtnExpire_Click(object sender, RoutedEventArgs e)
		{

		}

		#endregion

		#region functions

		string CreateCard(AccessKey key)
		{
			// create download file similar to below
			//{
			//	"Name":"Blue",
			//  "Url":"localhost"
			//}
			return $"{{\n\t\"Name\":\"{key.KeyName}\",\n\t\"Url\":\"{GlobalValues.LastConnection.IPAddress}\"\n}}";
		}

		/// <summary>
		/// get the key information
		/// </summary>
		private void LoadKeys()
		{
			// TODO: Connect with the server and get the key information

			var keys = new List<AccessKey>();
			for (int i = 1; i <= 5; i++)
			{
				var k = new AccessKey()
				{
					//CanAdmin = false,
					CanCheckInOut = true,
					CanSubmit = true,
					CanView = true,
					KeyId = i.ToString(),
					KeyName = $"Name-{i}",
					PIN = 5000 + i,
					Devices = new List<string>() { "One", "Two", i.ToString() },
					ExpirationDate = DateTime.Now.AddDays(30 + i)
				};
				keys.Add(k);
			}

			ExistingKeyInformation = new ObservableCollection<AccessKey>(keys.OrderBy(r => r.KeyName));

			KeyList.ItemsSource = ExistingKeyInformation;
		}

		private void AddUpdateKey(AccessKey initialKey)
		{
			// show the pop window
			var dlg = new KeyInput(initialKey);
			dlg.Owner = Application.Current.MainWindow;

			var result = dlg.ShowDialog();

			// did the window get user input
			if (result.HasValue && result.Value)
			{
				// does the key exist?
				AccessKey k = _ExistingKeyInformation.FirstOrDefault(r => r.KeyId == initialKey.KeyId);
				if (k != null)
				{
					k.CanCheckInOut = dlg.CanSubmitCheckBox.IsChecked == true;
					k.CanSubmit = dlg.CanSubmitCheckBox.IsChecked == true;
					k.CanView = dlg.CanViewCheckBox.IsChecked == true;
					k.ExpirationDate = DateTime.Parse(dlg.KeyExpirationDate.Text);
					k.KeyName = dlg.KeyNameTextBox.Text;
					k.PIN = int.Parse(dlg.KeyPinTextBox.Text);
				}
				else
				{
					// add new key and download it
					k = new AccessKey()
					{
						KeyId = Guid.NewGuid().ToString(),
						CanCheckInOut = dlg.CanSubmitCheckBox.IsChecked == true,
						CanSubmit = dlg.CanSubmitCheckBox.IsChecked == true,
						CanView = dlg.CanViewCheckBox.IsChecked == true,
						ExpirationDate = DateTime.Parse(dlg.KeyExpirationDate.Text),
						KeyName = dlg.KeyNameTextBox.Text,
						PIN = int.Parse(dlg.KeyPinTextBox.Text)
					};
					_ExistingKeyInformation.Add(k);
				}

				// update the screen
				ExistingKeyInformation = new ObservableCollection<AccessKey>(_ExistingKeyInformation);

				KeyList.ItemsSource = null;
				KeyList.ItemsSource = ExistingKeyInformation;

				KeyList.SelectedItem = k;
				//Application.Current.Dispatcher
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

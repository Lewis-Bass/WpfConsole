using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using WindowsData;
using WpfConsole.Resources;
using static System.Net.Mime.MediaTypeNames;

namespace WpfConsole.KeyManagement
{
	/// <summary>
	/// Interaction logic for KeyInput.xaml
	/// </summary>
	public partial class KeyInput : Window
	{
		public KeyInput(AccessKey accessKey)
		{
			InitializeComponent();

			KeyId = accessKey.KeyId;
			KeyNameTextBox.Text = accessKey.KeyName;
			KeyPinTextBox.Text = accessKey.PIN.ToString();
			CanCheckInOutCheckBox.IsChecked = accessKey.CanCheckInOut;
			CanSubmitCheckBox.IsChecked = accessKey.CanSubmit;
			CanViewCheckBox.IsChecked = accessKey.CanView;
			KeyExpirationDate.Text = accessKey.ExpirationDateString;
		}

		public string KeyId { get; set; }

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			bool isValid = true;

			if (string.IsNullOrWhiteSpace(KeyNameTextBox.Text))
			{
				sb.Append($" {Resource.KeyNameRequired}.");
				isValid = false;
			}

			if (string.IsNullOrWhiteSpace(KeyPinTextBox.Text))
			{
				sb.Append($" {Resource.KeyPinRequired}.");
				isValid = false;
			}
			else if(!int.TryParse(KeyPinTextBox.Text, out var val))
			{
				KeyPinTextBox.Text = "12345";
				sb.Append($" {Resource.KeyPinNumeric}.");
				isValid = false;
			}
			
			if (string.IsNullOrWhiteSpace(KeyExpirationDate.Text))
			{
				sb.Append($" {Resource.KeyExpiredRequired}.");
				isValid = false;
			}
			else if (DateTime.TryParse(KeyExpirationDate.Text, out var dt))
			{
				if (dt <= DateTime.Now)
				{
					KeyExpirationDate.Text = DateTime.Now.AddYears(10).ToString();
					sb.Append($" {Resource.KeyDateAfterNow}.");
					isValid = false;
				}
			}		
				
			if (isValid)
			{
				this.DialogResult = true;
				this.Close();
			}
			else
			{
				ErrorText.Text = sb.ToString();
			}
		}
	}
}

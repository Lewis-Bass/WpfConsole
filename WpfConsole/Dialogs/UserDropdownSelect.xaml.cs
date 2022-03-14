using Common.Settings;
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
using System.Windows.Shapes;
using Themes.Helpers;
using WindowsData;
using static Themes.Enumerations.ThemeEnums;

namespace WpfConsole.Dialogs
{
	/// <summary>
	/// Interaction logic for UserDropdownSelect.xaml
	/// </summary>
	public partial class UserDropdownSelect : Window
	{

		public ObservableCollection<DropDownRecords> DropdownValues { get; set; }

		public UserDropdownSelect(List<DropDownRecords> dropDownValues, string question)
		{
			LocalSettings settings = LocalSettings.Load();
			ThemeSelector.ApplyTheme(
				new Uri(ThemeSelector.ThemeEnumToURIString((ETheme)settings.ActiveTheme),
				UriKind.Relative));

			InitializeComponent();
			
			DropdownValues = new ObservableCollection<DropDownRecords>(dropDownValues);
			cbAnswer.ItemsSource = DropdownValues;

			lblQuestion.Content = question;
		}

		private void btnDialogOk_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

		private void Window_ContentRendered(object sender, EventArgs e)
		{
			cbAnswer.Focus();
		}

		public DropDownRecords Answer
		{
			get { return (DropDownRecords)cbAnswer.SelectedItem; }
		}
	}
}

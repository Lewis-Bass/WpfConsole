using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Themes.Helpers;
using static Themes.Enumerations.ThemeEnums;

namespace WpfConsole.Dialogs
{
	/// <summary>
	/// Interaction logic for UserInput.xaml
	/// </summary>
	public partial class UserInput : Window
	{
		public UserInput(string question, string defaultAnswer = "")
		{ 
			
			LocalSettings settings = LocalSettings.Load();
			ThemeSelector.ApplyTheme(
				new Uri(ThemeSelector.ThemeEnumToURIString((ETheme)settings.ActiveTheme),
				UriKind.Relative));

			InitializeComponent();

			lblQuestion.Content = question;
			txtAnswer.Text = defaultAnswer;
		}

		private void btnDialogOk_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

		private void Window_ContentRendered(object sender, EventArgs e)
		{
			txtAnswer.SelectAll();
			txtAnswer.Focus();
		}

		public string Answer
		{
			get { return txtAnswer.Text; }
		}

	}
}

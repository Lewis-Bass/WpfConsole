using Common;
using Common.ServerCommunication.Helpers;
using Common.ServerCommunication.Requests;
using WpfCommon.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Themes.Enumerations;
using Themes.Helpers;
using static Themes.Enumerations.ThemeEnums;

using System.IO;
using Common.Settings;

namespace WpfCommon.Preference
{
    /// <summary>
    /// Interaction logic for PreferenceSetup.xaml
    /// </summary>
    public partial class PreferenceSetup : UserControl
    {
        #region Properties

        LocalSettings settings = LocalSettings.Load();

        public List<string> ThemeList
        {
            get
            {
                return Common.EnumHelpers.GetEnumDescriptions(typeof(ETheme));
            }
        }

        public Dictionary<String, String> LanguageList
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"English", Resource.LanguageEnglish },
                    {"Spanish", Resource.LanguageSpanish }
                };
            }
        }

        private string CurrentLanguageSelection = string.Empty;

        #endregion

        #region Constructor
        public PreferenceSetup()
        {
            InitializeComponent();

            // setup combo box of themes
            cbThemeSelect.ItemsSource = ThemeList;
            cbThemeSelect.SelectedIndex = settings.ActiveTheme;

            // setup max connections
            cbMaxConnections.SelectedIndex = settings.MaxConnections - 1;

            // setup max files
            cbMaxFiles.SelectedIndex = settings.MaxFilesViewed - 1;
            cbMaxCheckedFiles.SelectedIndex = settings.MaxFilesCheckedOutViewed - 1;

            // setup language
            CurrentLanguageSelection = settings.Language;
            cbLanguage.ItemsSource = LanguageList;
            cbLanguage.SelectedValue = LanguageList.FirstOrDefault(r => r.Key == settings.Language).Key;

        }

        #endregion

        #region Combo box selection changed
        private void cbThemeSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                var cb = (ComboBox)sender;

                // set the theme value for the application                
                var value = Common.EnumHelpers.GetValueFromDescription<ETheme>(cb.SelectedValue.ToString());
                settings.ActiveTheme = (int)value;

                ThemeSelector.ApplyTheme(
                    new Uri(ThemeSelector.ThemeEnumToURIString((ETheme)settings.ActiveTheme),
                    UriKind.Relative));
            }
        }

        private void cbMaxConnections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                var cb = (ComboBox)sender;

                // set the theme value for the application                
                settings.MaxConnections = cb.SelectedIndex + 1;
            }

        }

        private void cbMaxFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                var cb = (ComboBox)sender;

                // set the theme value for the application
                settings.MaxFilesViewed = cb.SelectedIndex + 1;
            }
        }

        private void cbMaxCheckedFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                var cb = (ComboBox)sender;

                // set the theme value for the application
                settings.MaxFilesCheckedOutViewed = cb.SelectedIndex + 1;
            }
        }

        /// <summary>
        /// Change the language selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                var cb = (ComboBox)sender;

                // set the theme value for the application
                if (cb.SelectedValue != null && CurrentLanguageSelection != cb.SelectedValue.ToString())
                {
                    settings.Language = cb.SelectedValue.ToString();

                    // restart the application
                    var filePath = Assembly.GetEntryAssembly().Location.Replace(".dll", ".exe");
                    System.Diagnostics.Process.Start($"{filePath}");
                    Process.GetCurrentProcess().Kill();
                }
            }
        }

        #endregion

        private void btnFileBrowser_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                AddExtension = true,
                Multiselect = false,
                Filter = "License File|*.lic"
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var fileName = openFileDialog.FileName;
                txtFileName.Text = fileName;
                var fileContent = File.ReadAllText(fileName);

                if (File.Exists(GlobalValues.LicenseFileName))
                {
                    File.Delete(GlobalValues.LicenseFileName);
                }
                File.Copy(fileName, GlobalValues.LicenseFileName);

            }
        }

        private void StackPanel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // are we connected so that the license can be updated
            grpLicense.IsEnabled = GlobalValues.IsConnectionValid;
        }
    }
}

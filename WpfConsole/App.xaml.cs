using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
//using WpfConsole.Properties;

namespace WpfConsole
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetLanguageDictionary();
        }

        public void SetLanguageDictionary()
        {
            string lang = "en";
            var settings = LocalSettings.Load();
            switch (settings.Language )
            {
                case "English":
                    lang = "en";
                    break;
                case "Spanish":
                    lang = "es";
                    break;
                case "French":
                    lang = "es";
                    break;
                default:
                    lang = "en";
                    break;
            }

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
        }       
    }
}

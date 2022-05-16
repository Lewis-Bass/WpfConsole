using Common.ServerCommunication;
using Common.Settings;
using Common;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsData;
using Common.ServerCommunication.Helpers;
using WpfConsole.SearchMaster;

namespace WpfConsole.Statistics
{
    /// <summary>
    /// Interaction logic for StatisticsMain.xaml
    /// </summary>
    public partial class StatisticsMain : UserControl
    {

        #region constructor

        public StatisticsMain()
        {
            InitializeComponent();            
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // parallel run
            List<string> lastAuto = new List<string>();
            List<SearchResults> missingTags = new List<SearchResults>();
            Parallel.Invoke(
                () => lastAuto = LoadLastAutoLoad(), 
                () => missingTags = LoadUntagedFiles()
                );
            
            AutoLoadLastFiles.Results = lastAuto;
            foreach (var result in missingTags)
            {
                result.Tags = new System.Collections.ObjectModel.ObservableCollection<MetaTags>();
            }
            MissingTags.SearchResultsInfo = missingTags;
        }

        #endregion

        private List<string> LoadLastAutoLoad()
        {
            List<string> retval = new List<string>();
            try
            {
                LoginData loginData = SetupAutoLoadLogin();
                //https://localhost:5001/settings/readsettings
                var uri = new Uri($"http://{loginData.IPAddress}:{GlobalValues.FileScanPort}/autoload/lastfilessent");
                var returnStr = SendToServer.SendRest(loginData, uri).Result;
                if (!string.IsNullOrEmpty(returnStr))
                {
                    var response = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(returnStr.ToString());
                    retval = response.ToList();
                }
            }
            catch (Exception ex)
            {
               
            }
            return retval;
        }

        private LoginData SetupAutoLoadLogin()
        {
            // always use the localhost address
            return new LoginData
            {
                AccessKeyName = "Local Admin",
                Port = 5001,
                IPAddress = "localhost",
                Pin = string.Empty
            };
        }


        private List<SearchResults> LoadUntagedFiles()
        {
            // dummy up the data
            var results = SearchHelpers.ProcessSearch( null);
            return results;
           
        }


    }
}

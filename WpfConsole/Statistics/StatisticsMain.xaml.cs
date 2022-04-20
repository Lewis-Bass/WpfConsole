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
            LoadLastAutoLoad();
        }

        #endregion

        private void LoadLastAutoLoad()
        {
            try
            {
                LoginData loginData = SetupAutoLoadLogin();
                //https://localhost:5001/settings/readsettings
                var uri = new Uri($"http://{loginData.IPAddress}:{GlobalValues.FileScanPort}/autoload/lastfilessent");
                var returnStr = SendToServer.SendRest(loginData, uri).Result;
                if (!string.IsNullOrEmpty(returnStr))
                {
                    var response = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(returnStr.ToString());

                    AutoLoadLastFiles.Results = response.ToList();
                }
            }
            catch (Exception ex)
            {
               
            }
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

       
    }
}

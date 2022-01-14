using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WindowsData;
using WpfConsole.Resources;

namespace WpfConsole.Dialogs
{
    public class GetPinValue
    {
        public static string GetPinValueFromUser(ConnectionInformation data)
        {
            // do we need to get a PIN code?
            string pin = string.Empty;
            if (data.AccessKeyName != ConnectionInformation.LOCALADMIN)
            {
                var dlg = new UserInput(string.Format(Resource.EnterPIN, data.AccessKeyName));
                if (Application.Current.MainWindow.IsLoaded)
                {
                    dlg.Owner = Application.Current.MainWindow;
                }
                if (dlg.ShowDialog() == true)
                {
                    pin = dlg.Answer;
                }
                else
                {
                    // fail the login
                    MessageBox.Show(Resource.PINRequired, Resource.LoginName, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
            return pin;
        }
    }
}

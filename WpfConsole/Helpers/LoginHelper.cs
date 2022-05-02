using Common.ServerCommunication.Helpers;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WindowsData;
using WpfConsole.Dialogs;
using System.Windows.Controls;
using WpfConsole.Connection;

namespace WpfConsole.Helpers
{
    public class LoginHelper : UserControl
    {

        public LoginResult ProcessLogin(ConnectionInformation data)
        {
            string pin = string.Empty;
            bool isPinValid = false;
            LoginResult result = new LoginResult();

            if (data.IsLocalAdmin)
            {
                isPinValid = true;
            }
            else
            {
                // TODO: Push pin retrieval to GUI level
                pin = GetPinValue.GetPinValueFromUser(data);
                isPinValid = !string.IsNullOrWhiteSpace(pin);
            }

            Common.ServerCommunication.Response.ResponseLogin response = null;
            if (!isPinValid)
            {
                GlobalValues.LastConnection = null;
                GlobalValues.ConnectionToken = null;
                result.Result = LoginResult.ResultList.Failure;
                result.Message = $"{WpfConsole.Resources.Resource.LoginFailed}. {WpfConsole.Resources.Resource.PINRequired}";
            }
            else
            {
                response = AccountHelpers.Login(data, pin);
                data.IsCurrentConnection = response.IsLoginValid;

                // do we have a valid connection?               
                if (response.IsLoginValid)
                {
                    GlobalValues.LastConnection = data;
                    GlobalValues.ConnectionToken = response.ConnectionToken;
                    result.Result = LoginResult.ResultList.Success;
                    result.Message = WpfConsole.Resources.Resource.LoginSuccessful;

                }
                else
                {
                    GlobalValues.LastConnection = null;
                    GlobalValues.ConnectionToken = null;
                    result.Result = LoginResult.ResultList.Failure;
                    result.Message = WpfConsole.Resources.Resource.LoginFailed;
                }

            }
            return result;
        }

        #region Routed Event Handlers

        /// Create RoutedEvent
        /// This creates a static property on the UserControl, ViewFileEvent, which 
        /// will be used by the Window, or any control up the Visual Tree, that wants to 
        /// handle the event. 
        public static readonly RoutedEvent LoginChangedEvent =
            EventManager.RegisterRoutedEvent("ViewFileEvent", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(LoginHelper));

        /// Create RoutedEventHandler
        /// This adds the Custom Routed Event to the WPF Event System and allows it to be 
        /// accessed as a property from within xaml if you so desire
        public event RoutedEventHandler LoginChanged
        {
            add { AddHandler(LoginChangedEvent, value); }
            remove { RemoveHandler(LoginChangedEvent, value); }
        }

        #endregion

    }
}

using Common;
using Common.ServerCommunication.Helpers;
using Common.ServerCommunication.Requests;
using WpfConsole.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfConsole.AccountChange
{
    /// <summary>
    /// Interaction logic for MyPasswordChange.xaml
    /// </summary>
    public partial class MyPasswordChange : UserControl
    {
        #region Constructor

        public MyPasswordChange()
        {
            InitializeComponent();
        }

        #endregion

        #region Password Change

        /// <summary>
        /// Change the users password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            bool hasError = false;
            ErrorDisplay.Text = string.Empty;
            if (String.IsNullOrWhiteSpace(OldPassword.Password))
            {
                ErrorDisplay.Text += Resource.ErrorOldPasswordMissing + Environment.NewLine;
                hasError = true;
            }
            if (string.IsNullOrWhiteSpace(NewPassword.Password))
            {
                ErrorDisplay.Text += Resource.ErrorNewPasswordMissing + Environment.NewLine;
                hasError = true;
            }
            if (string.IsNullOrWhiteSpace(RekeyPassword.Password))
            {
                ErrorDisplay.Text += Resource.ErrorNewPasswordMissing + Environment.NewLine;
                hasError = true;
            }
            if (NewPassword.Password != RekeyPassword.Password)
            {
                ErrorDisplay.Text += Resource.ErrorNewPasswordMatch + Environment.NewLine;
                hasError = true;
            }
            //if (GlobalValues.IsConnectionValid && OldPassword.Password != GlobalValues.LastConnection.Password)
            //{
            //    ErrorDisplay.Text += Resource.ErrorBadPassword + Environment.NewLine;
            //    hasError = true;
            //}
            if (hasError)
            {
                return;
            }

            // sent the new passwords to the server
            var request = new RequestAccountUpdate
            {
                Connection = GlobalValues.LastConnection,
                ConnectionToken = GlobalValues.ConnectionToken,
                NewPassword = NewPassword.Password,
                OldAccountName = GlobalValues.LastConnection.AccessKeyName,
                OldPassword = OldPassword.Password
            };
            var response = AccountHelpers.AccountChange(request);

            if (response.ErrorList != null && response.ErrorList.Any())
            {
                ErrorDisplay.Text = response.ErrorList[0];
            }
        }

        #endregion
    }
}

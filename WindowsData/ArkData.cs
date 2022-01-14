
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WindowsData
{
    public class ArkData : INotifyPropertyChanged
    {

        string _arkName;
        public string ArkName
        {
            get { return _arkName; }
            set
            {
                if (value != _arkName)
                {
                    _arkName = value;                    
                    OnPropertyChanged("ArkName");
                }
            }
        }

        string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (value != _fileName)
                {
                    _fileName = value;
                    OnPropertyChanged("FileName");
                }
            }
        }

        string _adminName;
        public string AdminName
        {
            get { return _adminName; }
            set
            {
                if (value != _adminName)
                {
                    _adminName = value;
                    OnPropertyChanged("AdminName");
                }
            }
        }

        string _password1;
        public string Password1
        {
            get { return _password1; }
            set
            {
                if (value != _password1)
                {
                    _password1 = value;
                    OnPropertyChanged("Password1");
                    OnPropertyChanged("PasswordsValid");
                }
            }
        }

        string _password2;
        public string Password2
        {
            get { return _password2; }
            set
            {
                if (value != _password2)
                {
                    _password2 = value;
                    OnPropertyChanged("Password2");
                    OnPropertyChanged("PasswordsValid");
                }
            }
        }

        public bool PasswordsValid
        {
            get
            {
                return Password1 == Password2;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        //////public void LoadFromConfig()
        //////{
        //////    ArkName = "ToBeDetermined";
        //////    FileName = $"C:\\Documents\\{_arkName}.ark";
        //////    AdminName = "administrator";
        //////    Password1 = "abcd1234";
        //////    Password2 = Password1;
        //////}

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using WindowsData;

namespace WpfConsole.Dialogs
{
    /// <summary>
    /// Interaction logic for TagChange.xaml
    /// </summary>
    public partial class TagChange : Window, INotifyPropertyChanged

    {
        #region globals and constructor

        SearchResults _ResultInfo = null;
        public SearchResults ResultInfo
        {
            get { return _ResultInfo; }
            set
            {
                if (_ResultInfo != value)
                {
                    _ResultInfo = value;
                    SetupExisingTags();
                    OnPropertyChanged("ResultInfo");                    
                }
            }
        }

        ObservableCollection<MetaTags> _ExistingTags = new ObservableCollection<MetaTags>();
        public ObservableCollection<MetaTags> ExistingTags
        {
            get { return _ExistingTags; }
            set
            {
                if (_ExistingTags != value)
                {
                    _ExistingTags = value;
                    OnPropertyChanged("ExistingTags");
                }
            }
        }

        public TagChange()
        {
            InitializeComponent();

            this.DataContext = this;

            //SetupExisingTags();
        }

        #endregion

        #region button clicks

        /// <summary>
        /// Delete tags
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var delList = ResultInfo.Tags.Where(r => r.Selected).ToArray();
            foreach (var delete in delList)
            {
                ResultInfo.Tags.Remove(delete);
            }
            SetupExisingTags();
        }

        /// <summary>
        /// add tags from the existing list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddExisting_Click(object sender, RoutedEventArgs e)
        {
            var addList = ExistingTags.Where(r => r.Selected).ToArray();
            foreach (var add in addList)
            {
                add.Selected = false;
                ResultInfo.Tags.Add(add);
            }
            SetupExisingTags();
        }

        #endregion

        #region Helpers

        private void SetupExisingTags()
        {
            // TODO: Get list from vault

            // DUMMY UP THE EXISTING TAGS
            var tagList = new List<MetaTags> {
                new MetaTags { TagId = "A", TagName = "A" },
                new MetaTags { TagId = "Big", TagName = "Big" },
                new MetaTags { TagId = "Black", TagName = "Black" },
                new MetaTags { TagId = "Bug", TagName = "Bug" },
                new MetaTags { TagId = "Bit", TagName = "Bit" },
                new MetaTags { TagId = "A Big", TagName = "A Big" },
                new MetaTags { TagId = "Brown", TagName = "Brown" },
                new MetaTags { TagId = "How", TagName = "How" },
                new MetaTags { TagId = "Much", TagName = "Much" },
                new MetaTags { TagId = "Wood", TagName = "Wood" },
                new MetaTags { TagId = "Would", TagName = "Would" },
                new MetaTags { TagId = "Could", TagName = "Could" },
                new MetaTags { TagId = "WoodChuck", TagName = "WoodChuck" },
                new MetaTags { TagId = "Chuck", TagName = "Chuck" }
            };

            // remove from the tag list any values that are already selected
            var newTags = tagList.Where(r => !ResultInfo.Tags.Any(t => t.TagId == r.TagId)).ToList();

            ExistingTags = new ObservableCollection<MetaTags>(newTags);
        }


        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty (tbNewTag.Text))
            {
                return;
            }

            // TODO: Send to back end
            // TODO: Get TagId from the back end
            var newTag = new MetaTags
            {
                TagId = tbNewTag.Text,
                TagName = tbNewTag.Text,
                Selected = false
            };

            ResultInfo.Tags.Add(newTag);
        }

        #endregion

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

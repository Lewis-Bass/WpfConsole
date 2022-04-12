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
using System.IO;

namespace INOSetupFileCreate
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        const string SetupFileName = "Setup.txt";
        const string BottomFileName = "Bottom.txt";
        const string DirectoryScanFileName = "DirectoryScan.txt";

        public MainWindow()
        {
            InitializeComponent();
            LoadScreen();
        }

        private void LoadScreen()
        {
            if (File.Exists(SetupFileName))
            {
                tbSetup.Text = File.ReadAllText(SetupFileName);
            }
            if (File.Exists(BottomFileName))
            {
                tbBottom.Text = File.ReadAllText(BottomFileName);  
            }
            if (File.Exists(DirectoryScanFileName))
            {
                tbFiles.Text = File.ReadAllText(DirectoryScanFileName);
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            // validate that there are values in
            if (string.IsNullOrWhiteSpace(tbSetup.Text) || string.IsNullOrWhiteSpace(tbBottom.Text) || string.IsNullOrWhiteSpace(tbFiles.Text))
            {
                MessageBox.Show("Need values in all 3 fields!","ERROR", MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }

            // write out the changes to the 3 files
            File.WriteAllText(SetupFileName, tbSetup.Text);            
            File.WriteAllText(BottomFileName, tbBottom.Text);
            File.WriteAllText(DirectoryScanFileName, tbFiles.Text);

            // put together the output string
            var sb = new StringBuilder();
            sb.AppendLine(tbSetup.Text);
            sb.AppendLine("[Files]");

            // walk the directory structure
            var dirs = tbFiles.Text.Split(new char[] { '\n', '\r' });
            foreach (var dir in dirs)
            {
                if (!string.IsNullOrEmpty(dir))
                {
                    // split the directory from the install location <directory>|<install location>
                    string[] parts = dir.Split(new char[] { '|' });
                    if (parts.Length < 2)
                    {
                        MessageBox.Show($"Invalid Setup on directory {dir}", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    sb.AppendLine(RecursiveDirWalk(parts[0], parts[(parts.Length - 1)]));
                }
            }

            sb.AppendLine(tbBottom.Text);

            // write out the file
            File.WriteAllText(tbName.Text, sb.ToString());

            MessageBox.Show($"File {tbName.Text} has been created", "Notice", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private string RecursiveDirWalk(string directoryName, string installDirectoryName)
        {
            var sb = new StringBuilder();
            if (!Directory.Exists(directoryName))
            {
                return string.Empty;
            }
            foreach (var file in Directory.GetFiles(directoryName))
            {
                if (string.IsNullOrWhiteSpace(installDirectoryName))
                {
                    sb.AppendLine($"Source: \"{file}\"; DestDir: \"{{app}}\"");
                }
                else
                {
                    sb.AppendLine($"Source: \"{file}\"; DestDir: \"{{app}}\\{installDirectoryName}\"");
                }
            }
            // recursive call to any directories that are found
            foreach(var dir in Directory.GetDirectories(directoryName))
            {
                string partDir = GetLastDirectory(dir);
                if (!string.IsNullOrWhiteSpace(installDirectoryName))
                {
                    partDir = $"{installDirectoryName}\\{partDir}";
                }
                var str = RecursiveDirWalk(dir, partDir);
                if (str != null)
                {
                    sb.AppendLine(str);
                }
            }


            return sb.ToString();
        }

        private static string GetLastDirectory(string dir)
        {
            var dirs = dir.Split(new char[] { '\\', '/' });
            var partDir = dirs[(dirs.Length - 1)];
           

            return partDir;
        }
    }
}

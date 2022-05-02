using Serilog;
using System.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.DirectoryServices;
using Common.Settings;
using static System.Net.WebRequestMethods;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;

namespace FileScaner.Scan
{
    public class FileScan : IDisposable
    {

        AutoLoadSettings _settings;
        //string _logDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Logs");
        //string _SentFileLog = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Logs", "Sent.txt");
        //string _SkipFileLog = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Logs", "DidNotSend.txt");
        int _FilesLoaded = 0;
        int _FilesSkipped = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings"></param>
        public FileScan(AutoLoadSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Actually perform the scan
        /// </summary>
        public void ExecuteScan()
        {
            DateTime startedAt = DateTime.Now;

            if (!Directory.Exists(Constants.LogDirectory))
            {
                Directory.CreateDirectory(Constants.LogDirectory);
            }

            // clear the log files
            if (System.IO.File.Exists(Constants.SentFileLog))
            {
                System.IO.File.Delete(Constants.SentFileLog);
            }
            if (System.IO.File.Exists(Constants.SkipFileLog))
            {
                System.IO.File.Delete(Constants.SkipFileLog);
            }

            // create a list of directories to process
            var directoryList = _settings.AutoLoadDirectories.Select(r => r.PathName);

            // process the directories
            EnumerationOptions opts = new EnumerationOptions();
            opts.RecurseSubdirectories = true;
            opts.IgnoreInaccessible = false;
            opts.ReturnSpecialDirectories = true;
            string matchPattern = "*.*";

            foreach (string dirName in directoryList)
            {
                foreach (string fileName in Directory.EnumerateFiles(dirName, matchPattern, opts))
                {
                    // double check and make certain that the file exists
                    if (System.IO.File.Exists(fileName))
                    {
                        SendFile(fileName);
                    }
                }
            }

            // update the last processed date
            _settings.AutoLoadLastProcessed = startedAt;
            _settings.AutoLoadLastTotalUpload = _FilesLoaded;
            _settings.WriteFile();
        }

        //////        /// <summary>
        //////        /// Get users
        //////        /// this will only work on windows!
        //////        /// </summary>
        //////        /// <returns></returns>        
        //////        private List<string> FindWindowsUsers()
        //////        {
        //////#pragma warning disable CA1416 // Validate platform compatibility
        //////            var users = new List<string>();
        //////            // get list of users
        //////            string sPath = "WinNT://" + Environment.MachineName + ",computer";

        //////            using (var computerEntry = new System.DirectoryServices.DirectoryEntry(sPath))

        //////            {
        //////                foreach (System.DirectoryServices.DirectoryEntry childEntry in computerEntry.Children)
        //////                {
        //////                    if (childEntry.SchemaClassName == "User")
        //////                    {
        //////                        users.Add(childEntry.Name);
        //////                        //System.Diagnostics.Debug.WriteLine(childEntry.Name);
        //////                    }
        //////                }
        //////            }
        //////            return users;
        //////#pragma warning restore CA1416 // Validate platform compatibility
        //////        }

        ///////// <summary>
        ///////// Create the users directory for specfic documents
        ///////// </summary>
        ///////// <param name=""></param>
        ///////// <param name=""></param>
        ///////// <returns></returns>
        //////private List<string> FindDirectory(string scanDirectory, List<string> users)
        //////{
        //////    var dirs = new List<string>();
        //////    string dirName = string.Empty;
        //////    foreach (var user in users)
        //////    {
        //////        dirName = $"C:\\Users\\{user}\\{scanDirectory}";
        //////        //dirName = String.Format(@"C:\Users\{0}\{1}", user, scanDirectory);
        //////        if (Directory.Exists(dirName))
        //////        {
        //////            dirs.Add(dirName);
        //////        }
        //////    }
        //////    return dirs;
        //////}

        private void SendFile(string fileName)
        {
            // get file properties
            FileInfo info = new FileInfo(fileName);
            DateTime lastProcessed = _settings.AutoLoadLastProcessed;

            // has any date changed since the last scan?
            if ((info.CreationTime >= lastProcessed) || (info.LastWriteTime >= lastProcessed))
            {
                //System.Diagnostics.Debug.WriteLine($"Sent: {fileName}");
                System.IO.File.AppendAllText(Constants.SentFileLog, $"{fileName}\n\r");
                _FilesLoaded++;
                //TODO: Send to the Ark 
            }
            else
            {
                //System.Diagnostics.Debug.WriteLine($"Skip: {fileName}");
                System.IO.File.AppendAllText(Constants.SkipFileLog, $"{fileName}\n\r");
                _FilesSkipped++;
            }
        }

        public void Dispose()
        {

        }
    }
}

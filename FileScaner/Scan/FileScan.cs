using Serilog;
using System.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.DirectoryServices;
using Common.Settings;
using static System.Net.WebRequestMethods;

namespace FileScaner.Scan
{
    public class FileScan : IDisposable
    {

        AutoLoadSettings _settings;
        string _SentFileLog = "Sent.txt";
        string _SkipFileLog = "DidNotSend.txt";
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

            // clear the log files
            if (System.IO.File.Exists(_SentFileLog))
            {
                System.IO.File.Delete(_SentFileLog);
            }
            if (System.IO.File.Exists(_SkipFileLog))
            {
                System.IO.File.Delete(_SkipFileLog);
            }

            // create a list of directories to process
            var users = FindWindowsUsers();
            var directoryList = new List<string>();
            string scanDirectory = string.Empty;
            foreach (string dirName in _settings.AutoLoadDirectories)
            {
                // determine the directory to scan
                if (dirName == "[Documents]")
                {
                    scanDirectory = @"Documents";
                    directoryList.AddRange(FindDirectory(scanDirectory, users));
                }
                else if (dirName == "[Pictures]")
                {
                    scanDirectory = @"Pictures";
                    directoryList.AddRange(FindDirectory(scanDirectory, users));
                }
                else if (dirName == "[Music]")
                {
                    scanDirectory = @"Music";
                    directoryList.AddRange(FindDirectory(scanDirectory, users));
                }
                else if (dirName == "[Videos]")
                {
                    scanDirectory = @"Videos";
                    directoryList.AddRange(FindDirectory(scanDirectory, users));
                }
                else if (Directory.Exists(dirName))
                {
                    directoryList.Add(dirName);
                }
            }

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
                    SendFile(fileName);
                }
            }

            // update the last processed date
            _settings.AutoLoadLastProcessed = startedAt;
            _settings.AutoLoadLastTotalUpload = _FilesLoaded;
            _settings.WriteFile();
        }

        /// <summary>
        /// Get users
        /// this will only work on windows!
        /// </summary>
        /// <returns></returns>        
        private List<string> FindWindowsUsers()
        {
#pragma warning disable CA1416 // Validate platform compatibility
            var users = new List<string>();
            // get list of users
            string sPath = "WinNT://" + Environment.MachineName + ",computer";

            using (var computerEntry = new System.DirectoryServices.DirectoryEntry(sPath))

            {
                foreach (System.DirectoryServices.DirectoryEntry childEntry in computerEntry.Children)
                {
                    if (childEntry.SchemaClassName == "User")
                    {
                        users.Add(childEntry.Name);
                        //System.Diagnostics.Debug.WriteLine(childEntry.Name);
                    }
                }
            }
            return users;
#pragma warning restore CA1416 // Validate platform compatibility
        }

        /// <summary>
        /// Create the users directory for specfic documents
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        private List<string> FindDirectory(string scanDirectory, List<string> users)
        {
            var dirs = new List<string>();
            string dirName = string.Empty;
            foreach (var user in users)
            {
                dirName = $"C:\\Users\\{user}\\{scanDirectory}";
                //dirName = String.Format(@"C:\Users\{0}\{1}", user, scanDirectory);
                if (Directory.Exists(dirName))
                {
                    dirs.Add(dirName);
                }
            }
            return dirs;
        }

        private void SendFile(string fileName)
        {
            // get file properties
            FileInfo info = new FileInfo(fileName);
            DateTime lastProcessed = _settings.AutoLoadLastProcessed;

            // has any date changed since the last scan?
            if ((info.CreationTime >= lastProcessed) || (info.LastWriteTime >= lastProcessed))
            {
                //System.Diagnostics.Debug.WriteLine($"Sent: {fileName}");
                System.IO.File.AppendAllText(_SentFileLog, $"{fileName}\n\r");
                _FilesLoaded++;
                //TODO: Send to the Ark 
            }
            else
            {
                //System.Diagnostics.Debug.WriteLine($"Skip: {fileName}");
                System.IO.File.AppendAllText(_SkipFileLog, $"{fileName}\n\r");
                _FilesSkipped++;
            }
        }

        public void Dispose()
        {

        }
    }
}

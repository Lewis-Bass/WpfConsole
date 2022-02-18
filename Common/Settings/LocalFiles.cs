using Common.ServerCommunication.Requests;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using WindowsData;
using Common.ServerCommunication.Helpers;
using System.Diagnostics;

namespace Common
{
    /// <summary>
    /// Keeps track of all the local files that have been loaded from the ark to the users computer
    /// </summary>
    public class LocalFiles
    {

        #region Properties

        /// <summary>
        /// List of files that have been downloaded
        /// </summary>
        private List<LocalFileStatus> LocalFile = new List<LocalFileStatus>();

        /// <summary>
        /// Files that have been viewed
        /// </summary>
        public LocalFileStatus[] LocalFileViews
        {
            get
            {
                return LocalFile.Where(r => !r.IsCheckedOut).OrderByDescending(r => r.DateRecieved).ToArray();
            }
        }

        /// <summary>
        /// Files that have been checked out
        /// </summary>
        public LocalFileStatus[] LocalFileCheckedOut
        {
            get
            {
                return LocalFile.Where(r => r.IsCheckedOut).OrderByDescending(r => r.DateRecieved).ToArray();
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// constructor
        /// </summary>
        public LocalFiles()
        {
            ReadFile();
        }

        #endregion

        #region Read/Write

        /// <summary>
        /// read the configuration from the local storage
        /// </summary>
        private void ReadFile()
        {
            if (File.Exists(GlobalValues.LocalFilesStorageFileName))
            {
                XmlSerializer serializer = new XmlSerializer(LocalFile.GetType());
                TextReader textReader = new StreamReader(GlobalValues.LocalFilesStorageFileName);
                LocalFile = (List<LocalFileStatus>)serializer.Deserialize(textReader);
                textReader.Close();
            }
        }

        /// <summary>
        /// write the configuration to the local storage
        /// </summary>
        private void WriteFile()
        {
            if (LocalFile.Count() <= 0)
            {
                if (File.Exists(GlobalValues.LocalFilesStorageFileName))
                {
                    File.Delete(GlobalValues.LocalFilesStorageFileName);
                }
            }
            else
            {
                XmlSerializer serializer = new XmlSerializer(LocalFile.GetType());
                TextWriter textWriter = new StreamWriter(GlobalValues.LocalFilesStorageFileName);
                serializer.Serialize(textWriter, LocalFile);
                textWriter.Close();
            }
        }

        #endregion

        #region Add/Delete

        /// <summary>
        /// Add a file that has been checked out or viewed
        /// </summary>
        /// <param name="file"></param>
        public void AddFile(LocalFileStatus file)
        {
            var existing = LocalFile.FirstOrDefault(r => r.DocumentName == file.DocumentName);
            if (existing != null)
            {
                RemoveFile(existing);
            }
            LocalFile.Add(file);
        }

        /// <summary>
        /// Remove a file that has been checked out or viewed
        /// </summary>
        /// <param name="file"></param>
        public void RemoveFile(LocalFileStatus file)
        {
            LocalFile = LocalFile.Where(r => r.DocumentName != file.DocumentName).ToList();
            WriteFile();
        }

        #endregion

        #region View File

        /// <summary>
        /// View a file - download it from the vault
        /// </summary>
        /// <param name="viewFile"></param>
        public void ViewFile(LocalFileStatus viewFile)
        {
            if (GlobalValues.IsConnectionValid)
            {
                // View the document            
                string filePath = viewFile.LocalFileLocation;

                // get the file from the vault
                var request = new RequestFileRead
                {
                    Connection = GlobalValues.LastConnection,
                    ConnectionToken = GlobalValues.ConnectionToken,
                    DocumentName = viewFile.DocumentName,
                    VaultID = viewFile.VaultID,
                    IsCheckedOut = false
                };

                var response = FileHelpers.ReadFile(request);

                File.WriteAllBytes(viewFile.LocalFileLocation, response.FileContents);


                // view the file
                ViewLocalFile(viewFile);

                // Add the file to the recently viewed process
                AddFile(viewFile);

                // TODO: Purge local file down to the max view count
                WriteFile();

                // TODO: Add some kind of purge to the directory we do not need to keep view files around forever...???
            }
        }


        public void ViewLocalFile(LocalFileStatus viewFile)
        {
            if (!string.IsNullOrWhiteSpace(viewFile.LocalFileLocation))
            {
                //Process p = new Process();
                //p.StartInfo.CreateNoWindow = true;
                //p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //p.Start($"cmd.exe /c {viewFile.LocalFileLocation}");

                //var si = new System.Diagnostics.ProcessStartInfo();
                //si.CreateNoWindow = true;
                //si.FileName = "$cmd.exe / c { viewFile.LocalFileLocation}";
                //System.Diagnostics.Process.Start(si);

                Process fileopener = new Process();
                fileopener.StartInfo.FileName = "explorer";
                fileopener.StartInfo.Arguments = viewFile.LocalFileLocation;
                fileopener.Start();


                //System.Diagnostics.Process.Start("cmd.exe ", $"/c {viewFile.LocalFileLocation}");
            }
        }

        #endregion

        #region Check Out File

        public void CheckOutFile(LocalFileStatus checkOutFile)
        {
            // View the document            
            string filePath = checkOutFile.LocalFileLocation;

            // get the file from the vault
            var request = new RequestFileRead
            {
                Connection = GlobalValues.LastConnection,
                ConnectionToken = GlobalValues.ConnectionToken,
                DocumentName = checkOutFile.DocumentName,
                VaultID = checkOutFile.VaultID,
                IsCheckedOut = true
            };

            var response = FileHelpers.ReadFile(request);

            File.WriteAllBytes(checkOutFile.LocalFileLocation, response.FileContents);

            // view the file
            //System.Diagnostics.Process.Start("cmd.exe ", $"/c {filePath}");
            ViewFile(checkOutFile);

            // Add the file to the recently viewed process
            AddFile(checkOutFile);
            WriteFile();
        }

        #endregion

        #region Check In File

        /// <summary>
        /// check in a file - send the changes back to the vault
        /// </summary>
        /// <param name="checkInFile"></param>
        /// <param name="removeFile"></param>
        public bool CheckInFile(LocalFileStatus checkInFile, bool removeFile)
        {
            if (string.IsNullOrWhiteSpace(checkInFile.LocalFileLocation))
            {
                return false;
            }

            if (!File.Exists(checkInFile.LocalFileLocation))
            {
                return false;
            }

            // Load the file into the ark
            string filePath = checkInFile.LocalFileLocation;

            var request = new RequestCheckInChange
            {
                Connection = GlobalValues.LastConnection,
                ConnectionToken = GlobalValues.ConnectionToken,
                IsCheckin = true,
                VaultID = checkInFile.VaultID,
                FileContents = File.ReadAllBytes(checkInFile.LocalFileLocation)
            };

            var response = CheckInOutHelpers.UpdateCheckInOut(request);

            if (response.ErrorList != null && response.ErrorList.Any())
            {
                return false;
            }

            // remove local file?
            if (removeFile)
            {
                File.Delete(filePath);
            }

            // Add the file to the recently viewed process
            RemoveFile(checkInFile);

            return true;
        }

        /// <summary>
        /// Cancel the check in of the file
        /// </summary>
        /// <param name="checkInFile"></param>
        /// <param name="removeFile"></param>
        /// <returns></returns>
        public bool CancelCheckInFile(LocalFileStatus checkInFile)
        {

            // Send Cancellation to the ark

            var request = new RequestCheckInChange
            {
                Connection = GlobalValues.LastConnection,
                ConnectionToken = GlobalValues.ConnectionToken,
                IsCheckin = false,
                VaultID = checkInFile.VaultID
            };

            var response = CheckInOutHelpers.UpdateCheckInOut(request);

            if (response.ErrorList != null && response.ErrorList.Any())
            {
                return false;
            }

            // Add the file to the recently viewed process
            RemoveFile(checkInFile);

            return true;
        }

        #endregion

    }
}
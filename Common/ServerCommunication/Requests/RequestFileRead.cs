using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ServerCommunication.Requests
{
    public class RequestFileRead: BaseRequest
    {

        /// <summary>
        /// Unique Vault identifier for the file
        /// </summary>
        public string VaultID { get; set; }
        
        /// <summary>
        /// Name of the document to get from the vault
        /// </summary>
        public string DocumentName { get; set; }

        /// <summary>
        /// is the document being checked out?
        /// </summary>
        public bool IsCheckedOut { get; set; }
    }
}

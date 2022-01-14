using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ServerCommunication.Requests
{
    public class RequestCheckInChange: BaseRequest
    {

        /// <summary>
        /// Is this file to be checked in
        /// true - process the file contents
        /// false - cancel the checkin
        /// </summary>
        public bool IsCheckin { get; set; }

        /// <summary>
        /// unique identifier of the file in the vault
        /// </summary>
        public string VaultID { get; set; }

        /// <summary>
        /// Contents of the file to be checked in
        /// </summary>
        public byte[] FileContents { get; set; }
    }
}

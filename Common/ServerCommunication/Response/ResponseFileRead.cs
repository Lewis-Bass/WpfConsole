using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ServerCommunication.Response
{
    public class ResponseFileRead : BaseResponse
    {

        /// <summary>
        /// Actual Contents of the file
        /// </summary>
        public byte[] FileContents { get; set; }

        /// <summary>
        /// Length of the file
        /// </summary>
        public long FileLength { get; set; }

        //TODO: Add something to allow verification that the file was not tampered with
    }
}

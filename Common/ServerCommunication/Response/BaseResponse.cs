using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ServerCommunication.Response
{
    public class BaseResponse
    {
        /// <summary>
        /// Array of responses from the rest service
        /// </summary>
        public string[] ResponseString { get; set; }

        /// <summary>
        /// Array of exception messages from the rest service
        /// </summary>
        public string[] ErrorList { get; set; }


    }
}

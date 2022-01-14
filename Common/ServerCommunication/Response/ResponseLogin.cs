using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ServerCommunication.Response
{
    public class ResponseLogin : BaseResponse
    {
        /// <summary>
        /// Was the login successful?
        /// </summary>
        public bool IsLoginValid { get; set; } = false;

        /// <summary>
        /// The connection token used by the back end for processing.
        /// If the login was successful this field must have a value.
        /// The value will be sent on all subsequent requests.
        /// </summary>
        public long? ConnectionToken { get; set; } = null;
    }
}

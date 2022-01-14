using System;
using System.Collections.Generic;
using System.Text;
using WindowsData;

namespace Common.ServerCommunication.Requests
{
    /// <summary>
    /// Base class for all requests - they all must have the connection information
    /// </summary>
    public class BaseRequest
    {

        /// <summary>
        /// How to contact the server
        /// </summary>
        public ConnectionInformation Connection { get; set; }

        /// <summary>
        /// Connection Token returned from the Ark
        /// This field should be null until the GUI logs into the ark
        /// </summary>
        public long? ConnectionToken { get; set; } = null;
    }
}

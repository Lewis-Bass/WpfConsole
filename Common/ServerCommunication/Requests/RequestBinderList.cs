using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ServerCommunication.Requests
{
    /// <summary>
    /// Used to get a list of all binders - this may have to be expanded to pass search information
    /// </summary>
    public class RequestBinderList : BaseRequest
    {

        /// <summary>
        /// Maximum number of entries to return
        /// </summary>
        public int MaxEntries { get; set; } = 1000;

        /// <summary>
        /// When more than the Max Entries is returned start this set with this entry
        /// </summary>
        public int StartingEntry { get; set; } = 0;
    }
}

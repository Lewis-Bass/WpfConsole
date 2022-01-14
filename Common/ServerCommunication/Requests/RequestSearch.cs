using System;
using System.Collections.Generic;
using System.Text;
using WindowsData;

namespace Common.ServerCommunication.Requests
{
    /// <summary>
    /// Request that the ark be searched according to the criteria
    /// </summary>
    public class RequestSearch: BaseRequest
    {
        /// <summary>
        /// The name that should define the new binder or use this binders results
        /// </summary>
        public string BinderName { get; set; }

        /// <summary>
        /// The ID of the binder
        /// </summary>
        public string BinderID { get; set; }

        /// <summary>
        /// Should an existing binder be updated with the criteria or just used for the search?
        /// </summary>
        public bool UpdateBinder { get; set; } = false;

        /// <summary>
        /// Search the binders
        /// </summary>
        public List<SearchCriteriaBase> Search { get; set; }

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

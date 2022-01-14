using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ServerCommunication.Requests
{
    /// <summary>
    /// Request all of the search criteria that was used to create the binder
    /// </summary>
    public class RequestBinderCriteria : BaseRequest
    {
        /// <summary>
        /// Identifier of the binder to load
        /// </summary>
        public string BinderID { get; set; }
    }
}

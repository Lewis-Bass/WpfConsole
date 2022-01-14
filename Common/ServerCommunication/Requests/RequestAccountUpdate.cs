using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ServerCommunication.Requests
{
    public class RequestAccountUpdate : BaseRequest
    {

        /// <summary>
        /// The old account names's old password
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// The old account name
        /// </summary>
        public string OldAccountName { get; set; }

        /// <summary>
        /// The new password value
        /// </summary>
        public string NewPassword { get; set; }
    }
}

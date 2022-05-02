using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsData
{
    public class LoginResult
    {

        /// <summary>
        /// possible results of the login call
        /// </summary>
        public enum ResultList
        {
            Success,
            Failure,
            MissingPin
        }

        /// <summary>
        /// Result of the login try
        /// </summary>
        public ResultList Result { get; set; } = ResultList.Failure;

        /// <summary>
        /// Message that should be displayed to the user
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}

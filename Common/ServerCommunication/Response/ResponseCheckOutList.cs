using System;
using System.Collections.Generic;
using System.Text;
using WindowsData;

namespace Common.ServerCommunication.Response
{
    public class ResponseCheckOutList : BaseResponse
    {

        public List<LocalFileStatus> CheckOutList { get; set; }

    }
}

using Common.ServerCommunication.Requests;
using Common.ServerCommunication.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ServerCommunication.Helpers
{
    public class CheckInOutHelpers
    {

        public static ResponseCheckOutList GetAllCheckedOutFiles(BaseRequest request)
        {
            ResponseCheckOutList response = null;
            try
            {
				//var uriBuilder = new UriBuilder($"https://{request.Connection.IPAddress}:{GlobalValues.Port}/api/CheckInOut/CheckoutList");
				//var retval = SendToServer.Send(request.Connection, uriBuilder.Uri);
				var retval = SendToServer.Send(request);
				if (string.IsNullOrWhiteSpace(retval))
                {
                    response = new ResponseCheckOutList
                    {
                        ErrorList = new string[] { Common.Resources.Resource.UnknownCommunicationError }
                    };
                }
                else
                {
                    response = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseCheckOutList>(retval);
                }                
            }
            catch (Exception ex)
            {
                response = new ResponseCheckOutList
                {
                    ErrorList = new string[] { Common.Resources.Resource.UnknownCommunicationError }
                };
                Serilog.Log.Logger.Error(ex, "GetAllCheckedOutFiles");
            }

			// dummy the response object
			response = new ResponseCheckOutList();
			response.CheckOutList = new List<WindowsData.LocalFileStatus>();
			for (int i = 90; i < 95; i++)
			{
				var rec = new WindowsData.LocalFileStatus
				{
					DocumentName = $"Checkout-{i}.txt",
					IsCheckedOut = true,
					VaultID = $"vault{i}"
				};
				response.CheckOutList.Add(rec);
			}
			return response;
        }

        public static BaseResponse UpdateCheckInOut(RequestCheckInChange request)
        {
            BaseResponse response = null;
            try
            {
                //var uriBuilder = new UriBuilder($"https://{request.Connection.IPAddress}:{GlobalValues.Port}/api/CheckInOut/CheckInOutUpdate");
                //var retval = SendToServer.Send(request.Connection, uriBuilder.Uri);
				var retval = SendToServer.Send(request);
				if (string.IsNullOrWhiteSpace(retval))
                {
                    response = new BaseResponse
                    {
                        ErrorList = new string[] { Common.Resources.Resource.UnknownCommunicationError }
                    };
                }
                else
                {
                    response = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseResponse>(retval);
                }
            }
            catch (Exception ex)
            {
                response = new BaseResponse
                {
                    ErrorList = new string[] { Common.Resources.Resource.UnknownCommunicationError }
                };
                Serilog.Log.Logger.Error(ex, "UpdateCheckInOut");
            }

            return response;
        }
    }
}

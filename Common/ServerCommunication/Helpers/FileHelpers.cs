using Common.ServerCommunication;
using Common.ServerCommunication.Requests;
using Common.ServerCommunication.Response;

using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ServerCommunication.Helpers
{
    public class FileHelpers
    {

        /// <summary>
        /// Send a request to the web server to search for files
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static BaseResponse SendFiles(RequestFileSend request)
        {
            BaseResponse response = null;
            try
            {
                //var uriBuilder = new UriBuilder($"https://{request.Connection.IPAddress}:{GlobalValues.Port}/api/File/Save");
                //var retval = SendToServer.Send(request, uriBuilder.Uri);

				var retval = SendToServer.Send(request);
				if (string.IsNullOrWhiteSpace(retval))
                {
                    response = new BaseResponse();
                    response.ErrorList = new string[] { Common.Resources.Resource.UnknownCommunicationError };
                }
                else
                {
                    response = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseResponse>(retval);
                }
            }
            catch (Exception ex)
            {
                response = new BaseResponse();
                response.ErrorList = new string[] { Common.Resources.Resource.UnknownCommunicationError };
                Serilog.Log.Logger.Error(ex, "SendFiles");
            }
            return response;
        }

        /// <summary>
        /// Contact the server and read the file
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static ResponseFileRead ReadFile(RequestFileRead request)
        {

            ResponseFileRead response = null;
            try
            {
                var uriBuilder = new UriBuilder($"https://{request.Connection.IPAddress}:{GlobalValues.Port}/api/File/Read");
                //var retval = SendToServer.Send(request, uriBuilder.Uri);
				var retval = SendToServer.Send(request);
				if (string.IsNullOrWhiteSpace(retval))
                {
                    response = new ResponseFileRead();
                    response.ErrorList = new string[] { Common.Resources.Resource.UnknownCommunicationError };
                }
                else
                {
                    response = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseFileRead>(retval);
                }
            }
            catch (Exception ex)
            {
                response = new ResponseFileRead();
                response.ErrorList = new string[] { Common.Resources.Resource.UnknownCommunicationError };
                Serilog.Log.Logger.Error(ex, "ReadFile");
            }

			response.FileContents = Encoding.ASCII.GetBytes($"{request.DocumentName} - HOW MUCH WOOD WOULD A WOODCHUCK CHUCK IF A WOOD CHUCK COULD CHUCK WOOD");
			response.FileLength = response.FileContents.Length;
			return response;
        }
    }
}

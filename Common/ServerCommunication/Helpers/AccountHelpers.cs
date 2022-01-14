
using Common.ServerCommunication;
using Common.ServerCommunication.Requests;
using Common.ServerCommunication.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using WindowsData;

namespace Common.ServerCommunication.Helpers
{
	public class AccountHelpers
	{
		/// <summary>
		/// Send Login Information to the server
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		public static ResponseLogin Login(ConnectionInformation connection, string pin)
		{

			ResponseLogin response = null;
			try
			{
				//var uriBuilder = new UriBuilder($"https://{connection.IPAddress}:{GlobalValues.Port}/api/Login");
				// var retval = SendToServer.Send(connection, uriBuilder.Uri);
				// socket test - uncomment 

				var retval = SendToServer.Send(connection);
				if (string.IsNullOrWhiteSpace(retval))
				{
					response = new ResponseLogin { IsLoginValid = false };
				}
				else
				{
					response = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseLogin>(retval);
				}
			}
			catch (Exception ex)
			{
				response = new ResponseLogin { IsLoginValid = false };
				Serilog.Log.Logger.Error(ex, "Login");
			}

			// process the login and validate it - stubbed out
			// DUMMY THE RESPONSE - SOCKET DOES NOT RETURN ANYTHING YET
			response.IsLoginValid = true;
			response.ConnectionToken = 12345;

			// Record it in the system settings
			if (response.IsLoginValid)
            {
				var settings = LocalSettings.Load();
				settings.LastConnection = connection;
            }

			return response;

		}

		/// <summary>
		/// Account change - currently only supports password change
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		public static BaseResponse AccountChange(RequestAccountUpdate request)
		{
			BaseResponse response = null;
			try
			{
				//var uriBuilder = new UriBuilder($"https://{GlobalValues.LastConnection.IPAddress}:{GlobalValues.Port}/api/AccountUpdate/UpdateAccount");
				//var retval = SendToServer.Send(request, uriBuilder.Uri);
				var retval = SendToServer.Send(request);
				if (string.IsNullOrWhiteSpace(retval))
				{
					response = new BaseResponse { ErrorList = new string[] { Common.Resources.Resource.UnknownCommunicationError } };
				}
				else
				{
					response = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseResponse>(retval);
				}
			}
			catch (Exception ex)
			{
				response = new BaseResponse { ErrorList = new string[] { Common.Resources.Resource.UnknownCommunicationError } };
				Serilog.Log.Logger.Error(ex, "AccountChange");
			}
			return response;
		}

		public static BaseResponse LogoutDisconnect(BaseRequest request)
		{
			BaseResponse response = null;
			try
			{
				//var uriBuilder = new UriBuilder($"https://{GlobalValues.LastConnection.IPAddress}:{GlobalValues.Port}/api/AccountUpdate/LogoutDisconnect");
				//var retval = SendToServer.Send(request, uriBuilder.Uri);
				var retval = SendToServer.Send(request);
				if (string.IsNullOrWhiteSpace(retval))
				{
					response = new BaseResponse { ErrorList = new string[] { Common.Resources.Resource.UnknownCommunicationError } };
				}
				else
				{
					response = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseResponse>(retval);
				}
			}
			catch (Exception ex)
			{
				response = new BaseResponse { ErrorList = new string[] { Common.Resources.Resource.UnknownCommunicationError } };
				Serilog.Log.Logger.Error(ex, "AccountChange");
			}
			return response;
		}

	}
}

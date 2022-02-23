using Common.ServerCommunication;
using Common.ServerCommunication.Requests;
using System;
using System.Collections.Generic;
using System.Text;
using WindowsData;

namespace Common.ServerCommunication.Helpers
{
	public class BinderHelpers
	{
		/// <summary>
		/// Get a list of all binders that are found within the ark - this may change to be more search oriented
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static List<BinderInformation> GetBinderList(RequestBinderList request)
		{
			List<BinderInformation> response = new List<BinderInformation>();
			try
			{
				//var uriBuilder = new UriBuilder($"https://{request.Connection.IPAddress}:{GlobalValues.Port}/api/Binder/GetBinderList");
				//var retval = SendToServer.Send(request, uriBuilder.Uri);
				var retval = SendToServer.Send(request);
				if (string.IsNullOrWhiteSpace(retval))
				{
					response = new List<BinderInformation>();
				}
				else
				{
					response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BinderInformation>>(retval);
				}
			}
			catch (Exception ex)
			{
				response = new List<BinderInformation>();
				Serilog.Log.Logger.Error(ex, "GetBinderList");
			}

			// DUMMY THE RESPONSE - SOCKET DOES NOT RETURN ANYTHING YET
			// process the login and validate it - stubbed out
			for (int i = 1; i <= 25; i++)
			{
				response.Add(new BinderInformation { BinderID = i.ToString(), BinderName = $"Binder {i}" });
			}

			return response;
		}

		/// <summary>
		/// Get the search criteria that was used to establish the binder
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="binderID"></param>
		/// <returns></returns>
		public static List<SearchCriteriaBase> GetBinderSearchCriteria(ConnectionInformation connection, string binderID)
		{
			List<SearchCriteriaBase> response = new List<SearchCriteriaBase>();
			var request = new RequestBinderCriteria
			{
				Connection = connection,
				BinderID = binderID
			};
			try
			{
				//var uriBuilder = new UriBuilder($"https://{connection.IPAddress}:{GlobalValues.Port}/api/Binder/GetBinderSearchCriteria");
				//var retval = SendToServer.Send(request, uriBuilder.Uri);
				var retval = SendToServer.Send(request);
				if (string.IsNullOrWhiteSpace(retval))
				{
					response = new List<SearchCriteriaBase>();
				}
				else
				{
					response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchCriteriaBase>>(retval);
				}
			}
			catch (Exception ex)
			{
				response = new List<SearchCriteriaBase>();
				Serilog.Log.Logger.Error(ex, "GetBinderSearchCriteria");
			}

			// process the login and validate it - stubbed out
			for (int i = 1; i <= 10; i++)
			{
				var criteria = new SearchCriteriaGUI
				{
					Field = $"FileName",
					Criteria = "=",
					ValueMin = $"{request.BinderID}-{i}.txt",
					Relationship = "^"
				};
				response.Add(criteria);
			}

			return response;
		}
	}
}

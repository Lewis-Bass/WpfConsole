using Common.ServerCommunication;
using Common.ServerCommunication.Requests;
using System;
using System.Collections.Generic;
using System.Text;
using WindowsData;

namespace Common.ServerCommunication.Helpers
{
    public class SearchHelpers
    {

        /// <summary>
        /// Send a request to the web server to search for files
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static List<SearchResults> ProcessSearch(RequestSearch request)
        {
            List<SearchResults> response = new List<SearchResults>();
            try
            {
                //var uriBuilder = new UriBuilder($"https://{request.Connection.IPAddress}:{GlobalValues.Port}/api/Search/SearchBinders");
                //var retval = SendToServer.Send(request, uriBuilder.Uri);
                var retval = SendToServer.Send(request);
                if (string.IsNullOrWhiteSpace(retval))
                {
                    response = new List<SearchResults>();
                }
                else
                {
                    response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchResults>>(retval);
                }
            }
            catch (Exception ex)
            {
                response = new List<SearchResults>();
                Serilog.Log.Logger.Error(ex, "ProcessSearch");
            }

            // DUMMY THE RESPONSE - SOCKET DOES NOT RETURN ANYTHING YET
            for (int i = 1; i < 20; i++)
            {
                var tagList = new List<MetaTags> {
                    new MetaTags { TagId = i.ToString(), TagName = i.ToString() },
                    new MetaTags { TagId = "A", TagName = "A" },
                    new MetaTags { TagId = "Big", TagName = "Big" },
                    new MetaTags { TagId = "Black", TagName = "Black" },
                    new MetaTags { TagId = "Bug", TagName = "Bug" },
                    new MetaTags { TagId = "Bit", TagName = "Bit" },
                    new MetaTags { TagId = "A Big", TagName = "A Big" },
                    new MetaTags { TagId = "Brown", TagName = "Brown" },
                    new MetaTags { TagId = "Bear", TagName = "Bear" }
                };
                var result = new SearchResults
                {
                    DocumentName = $"Document-{i}.txt",
                    DocumentDate = DateTime.Now.AddDays((0 - i)).ToShortDateString(),
                    PathName = $"C:\\MyDocuments\\Document-{i}.txt",
                    Tags = new System.Collections.ObjectModel.ObservableCollection<MetaTags>(tagList)
                };
                response.Add(result);
            }

            return response;
        }

    }
}

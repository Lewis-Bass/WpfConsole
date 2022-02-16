using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using Common.Encryption;
using Common.ServerCommunication.Response;
using System.Net.Sockets;
using WindowsData;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;

namespace Common.ServerCommunication
{
    public class SendToServer
    {

        /// <summary>
        /// Send the data to the web server and get the response back
        /// The response is sent back as a string
        /// </summary>
        /// <param name="data"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static string Send(Object data)
        {
            try
            {
                string dataString = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                //dataString = EncrypDecrypt.EncryptString(dataString); // BRING BACK SOMEDAY!
                //dataString = System.Net.WebUtility.HtmlEncode(dataString); // NOT NEEDED FOR SOCKET CONNECTIONS

                string outputString = string.Empty;
                if (GlobalValues.IsConnectionValid)
                {
                    outputString = AsynchronousClient.StartClient(dataString, GlobalValues.LastConnection.IPAddress, GlobalValues.Port);
                }
                else if (data is ConnectionInformation)
                {
                    var connect = (ConnectionInformation)data;
                    outputString = AsynchronousClient.StartClient(dataString, connect.IPAddress, GlobalValues.Port);
                }
                else
                {
                    throw new ArgumentException("Invalid Connection Information");
                }

                return outputString;

            }
            catch (Exception ex)
            {
                Serilog.Log.Logger.Error(ex, $"SendToServer.Send Sockets address={GlobalValues.LastConnection.IPAddress} port={GlobalValues.Port}");
            }
            return null;
        }

        /// <summary>
        /// send to a web rest server
        /// </summary>
        /// <param name="data"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        /// TODO: Make this async
        public static async Task<string> SendRest(Object data, Uri destination)
        {
            string dataString = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            //dataString = EncrypDecrypt.EncryptString(dataString);
            //dataString = System.Net.WebUtility.HtmlEncode(dataString);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(destination);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST"; // MUST USE POST TO WRITE TO THE BODY
            httpWebRequest.Accept = "application/json";
            httpWebRequest.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(dataString);
                streamWriter.Flush();
                streamWriter.Close();
            }

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var outputString = Payload.DecodePayload(httpResponse.GetResponseStream());
                return outputString;
            }
            catch (Exception ex)
            {
                Serilog.Log.Logger.Error(ex, "SendToServer.Send");
            }
            return null;
        }

    }
}

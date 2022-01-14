using Common.Encryption;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common.ServerCommunication
{
    public class Payload
    {

        public static string DecodePayload(string encryptedJson)
        {
            if (string.IsNullOrEmpty(encryptedJson))
            {
                return null;
            }
            string data = System.Net.WebUtility.HtmlDecode(encryptedJson);
            //data = EncrypDecrypt.DecryptString(data);

            return data;
        }

        public static string DecodePayload(Stream payload)
        {
            string encryptedJson = string.Empty;
            using (StreamReader reader = new StreamReader(payload, Encoding.UTF8))
            {
                encryptedJson = reader.ReadToEndAsync().GetAwaiter().GetResult();
            }

            return DecodePayload(encryptedJson);
        }

        public static string EncodePayload(object response)
        {
            string dataString = Newtonsoft.Json.JsonConvert.SerializeObject(response);            
            //dataString = EncrypDecrypt.EncryptString(dataString);
            dataString = System.Net.WebUtility.HtmlEncode(dataString);

            return dataString;
        }
    }
}

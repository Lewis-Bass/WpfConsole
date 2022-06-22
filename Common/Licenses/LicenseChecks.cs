using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using Common.ServerCommunication.Requests;
using Common.ServerCommunication;
using System.Security.Policy;
using WindowsData;
using System.ComponentModel;

namespace Common.Licenses
{
    public class LicenseChecks
    {

        public enum LicenseStatus
        {
            Missing,
            Expired,
            Valid,
            Demo,
            FailDeviceName
        }

        public LicenseStatus GetLicenseStatus()
        {
            // does the license file exist?
            if (!File.Exists(GlobalValues.LicenseFileName))
            {
                return LicenseStatus.Missing;
            }
            else
            {
                // can license file be decrypted?
                WindowsData.License license = GetLicense();

                if (license.Type == "Demo" && license.ExpirationDate > DateTime.Now)
                {
                    return LicenseStatus.Demo;
                }
                if (license.ExpirationDate <= DateTime.Now)
                {
                    return LicenseStatus.Expired;
                }
            }
            return LicenseStatus.Valid;
        }

        public WindowsData.License GetLicense()
        {
            // TODO: Encrypt License File
            //var decryptKey = GetDecyrptionKey();
            //byte[] licenseBytes = File.ReadAllBytes(GlobalValues.LicenseFileName);
            //string licenseString = Encryption.EncrypDecrypt.DecryptStringFromBytes_Aes(licenseBytes, decryptKey["key"], decryptKey["iv"]);

            ////// xml file
            //////var licenseString = File.ReadAllText(GlobalValues.LicenseFileName);
            //////XmlSerializer serializer = new XmlSerializer(typeof(WindowsData.License));
            //////StringReader rdr = new StringReader(licenseString);
            //////WindowsData.License license = (WindowsData.License)serializer.Deserialize(rdr);
            ///

            var licenseString = File.ReadAllText(GlobalValues.LicenseFileName);
            var license = Newtonsoft.Json.JsonConvert.DeserializeObject<WindowsData.License>(licenseString);

            return license;
        }

        public DateTime GetExpirationDate()
        {
            var license = GetLicense();
            return license.ExpirationDate;
        }

        public bool CreateDemoLicense()
        {

            var winInfo = new WindowsInformation();
            var devInfo = winInfo.PullAllStats()
                .Select(r => new LicenseDeviceInfo(r.StatisticType, r.StatisticCategory, r.StatisticName, r.StatisticValue)).ToList();
            var requestInfo = new RequestLicense
            {
                LicenseType = "Demo",
                DeviceName = Environment.MachineName,
                DeviceInfo = devInfo,
                Product = "Vault",
            };

            var uri = new UriBuilder(Common.Resources.Resource.LicenseServerCreateAddress).Uri;

            try
            {
                var returnStr = SendToServer.SendRest(requestInfo, uri).Result;

                System.Diagnostics.Debug.WriteLine(returnStr);

                File.WriteAllText(GlobalValues.LicenseFileName, returnStr);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error("CreateDemoLicense", ex);
                return false;
            }

            return true;

            //////////////TODO: reach out to the server to get the information - passing in the security keys

            ////////////var wi = new WindowsInformation();
            ////////////var systemName = wi.GetValue("DNSHostName");
            ////////////var decryptKey = GetDecyrptionKey();

            ////////////string license = @"<License>
            ////////////	<Type>Demo</Type>
            ////////////	<Expiration>" + DateTime.Now.AddDays(10).ToShortDateString() + @"</Expiration>
            ////////////	<DeviceName>" + systemName.StatisticValue + @"</DeviceName>
            ////////////	<Permissions>
            ////////////		<MaxKeys>5</MaxKeys>
            ////////////	</Permissions>
            ////////////</License>";

            //////////////string license = @"<License>
            //////////////	<Type>Full</Type>
            //////////////	<Expiration>" + DateTime.Now.AddDays(10).ToShortDateString() + @"</Expiration>
            //////////////	<DeviceName>" + systemName.StatisticValue + @"</DeviceName>
            //////////////	<Permissions>
            //////////////		<MaxKeys>5</MaxKeys>
            //////////////	</Permissions>
            //////////////</License>";

            ////////////File.WriteAllText(GlobalValues.LicenseFileName, license);

            ////////////// TODO: ENCRYPT THE LICENSE
            //////////////byte[] encriptLicense = Encryption.EncrypDecrypt.EncryptStringToBytes_Aes(license, decryptKey["key"], decryptKey["iv"]);
            //////////////// write out the file
            //////////////File.WriteAllBytes(GlobalValues.LicenseFileName, encriptLicense);

            ////////////return true;
        }

        private Dictionary<string, byte[]> GetDecyrptionKey()
        {
            var retval = new Dictionary<string, byte[]>();
            var wi = new WindowsInformation();
            var serial = wi.GetValue("SerialNumber");
            var sku = wi.GetValue("SystemSKUNumber");
            var iv1 = wi.GetValue("InstallDate");
            var iv2 = wi.GetValue("LastBootUpTime");

            //SystemDirectory
            // split the serial number into pieces
            var parts = serial.StatisticValue.Split('-');
            // first 3 pieces become the key last becomes the IV value
            if (parts.Length < 4)
            {
                throw new Exception("Can Not Create Decryption Keys");
            }
            // key must be 32 bytes
            string key = parts[0] + parts[1] + parts[2] + parts[2] + parts[1] + parts[0] + parts[3];
            key = key.Substring(1, 32);
            retval.Add("key", Encoding.UTF8.GetBytes(key));

            // IV must be 16 bytes
            string iv = iv1.StatisticValue + iv2.StatisticValue;
            iv = iv.Substring(3, 16);
            retval.Add("iv", Encoding.UTF8.GetBytes(iv));

            return retval;
        }
    }
}

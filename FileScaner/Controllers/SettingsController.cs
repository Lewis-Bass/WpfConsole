using Common.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsData;

namespace FileScaner.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(ILogger<SettingsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ReadSettings() ///LoginData data)
        {

            string jsonString = string.Empty;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                jsonString = await reader.ReadToEndAsync(); //reader.ReadToEnd();
            }
            var request = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginData>(jsonString);

            AutoLoadSettings settings = AutoLoadSettings.Load(true);

            return Json(settings);            
        }

        [HttpPost]
        public async Task<IActionResult> WriteSettings() //AutoLoadSettings newSettings)
        {

            string jsonString = string.Empty;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                jsonString = await reader.ReadToEndAsync(); 
            }
            var newSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<AutoLoadSettings>(jsonString);


            AutoLoadSettings oldsettings = AutoLoadSettings.Load(true);
            oldsettings.AutoLoadPin = newSettings.AutoLoadPin;
            oldsettings.AutoLoadEndTime = newSettings.AutoLoadEndTime; 
            //oldsettings.AutoLoadLastTotalUpload = newSettings.AutoLoadLastTotalUpload;
            oldsettings.AutoLoadStartTime = newSettings.AutoLoadStartTime;
            //oldsettings.AutoLoadLastProcessed = newSettings.AutoLoadLastProcessed;
            oldsettings.AutoLoadConnection = newSettings.AutoLoadConnection;
            oldsettings.AutoLoadDirectories = newSettings.AutoLoadDirectories;
            oldsettings.AutoLoadPin = newSettings.AutoLoadPin;
            oldsettings.WriteFile();

            return Json("{result:true}");
        }
    }
}

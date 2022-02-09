using Common.Settings;
using FileScaner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FileScaner.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(ILogger<SettingsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult ReadSettings()
        {
            AutoLoadSettings settings = AutoLoadSettings.Load();

            return Json(settings);            
        }

        [HttpPost]
        public IActionResult WriteSettings(AutoLoadSettings newSettings)
        {
            AutoLoadSettings oldsettings = AutoLoadSettings.Load();
            oldsettings.AutoLoadPin = newSettings.AutoLoadPin;
            oldsettings.AutoLoadEndTime = newSettings.AutoLoadEndTime; 
            //oldsettings.AutoLoadLastTotalUpload = newSettings.AutoLoadLastTotalUpload;
            oldsettings.AutoLoadStartTime = newSettings.AutoLoadStartTime;
            //oldsettings.AutoLoadLastProcessed = newSettings.AutoLoadLastProcessed;
            oldsettings.AutoLoadConnection = newSettings.AutoLoadConnection;
            oldsettings.AutoLoadDirectories = newSettings.AutoLoadDirectories;
            oldsettings.AutoLoadPin = newSettings.AutoLoadPin;

            return Json("{result:true}");
        }
    }
}

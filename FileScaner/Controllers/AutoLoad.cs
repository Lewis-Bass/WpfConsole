using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.IO;

namespace FileScaner.Controllers
{
    public class AutoLoad : Controller
    {
        [HttpPost]
        public async Task<IActionResult> LastFilesSent()
        {

            var fileName = Constants.SentFileLog;
            var fileContent = System.IO.File.ReadAllText(fileName);
            var names = fileContent.Split(new char[] { '\r', '\n' },StringSplitOptions.RemoveEmptyEntries);

            return Json(names);
        }
    }
}

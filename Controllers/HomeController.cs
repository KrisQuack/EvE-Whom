using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EveConnectionFinder.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace EveConnectionFinder.Controllers
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment _hostEnvironment;
        private string _errorLog;
        public HomeController(IWebHostEnvironment environment)
        {
            _hostEnvironment = environment;
            _errorLog = Path.Combine(_hostEnvironment.ContentRootPath, "logging/errorlog.txt");
        }

        public IActionResult Index()
        {
            ViewBag.error = "e";
            return View();
        }
        [HttpPost]
        public IActionResult Index(FormSubmit form)
        {
            //Time code execution
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                //Data is returned from the form here
                //Process Details for user character
                var userCharacter = new Character {charName = form.UserCharacter};
                userCharacter.GetCharID();
                userCharacter.GetCorps();
                //Process details for paste
                var pastedCharacters = new List<Character>();
                string[] charList =
                    form.PasteList.Replace(userCharacter.charName,"").Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                //Parallel to multi-thread and get things done faster
                Parallel.ForEach(charList, name =>
                {
                    var character = new Character {charName = name};
                    character.GetCharID();
                    character.GetCorps();
                    pastedCharacters.Add(character);
                });
                //Find connections
                ViewBag.connections = userCharacter.FindConnections(pastedCharacters)
                    .OrderByDescending(c => c.overlapStart);
            }
            catch (Exception ex)
            {
                ViewBag.error = "e";
                System.IO.File.AppendAllText(_errorLog, DateTime.UtcNow + " : " + ex + Environment.NewLine);
            }
            watch.Stop();
            ViewBag.timer = watch.Elapsed.TotalSeconds;
            //Return view
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}

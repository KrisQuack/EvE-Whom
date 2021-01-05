using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EveConnectionFinder.Models;

namespace EveConnectionFinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(FormSubmit form)
        {
            //Data is returned from the form here
            //Process Details for user character
            var userCharacter = new Character{charName = form.UserCharacter};
            userCharacter.GetCharID();
            userCharacter.GetCorps();
            //Process details for paste
            var pastedCharacters = new List<Character>();
            foreach(var name in form.PasteList.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var character = new Character { charName = name };
                character.GetCharID();
                character.GetCorps();
                pastedCharacters.Add(character);
            }
            //Find connections
            ViewBag.connections = userCharacter.FindConnections(pastedCharacters).OrderByDescending(c => c.entityStart);
            //Return view
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using System.Collections.Generic;

namespace EveConnectionFinder.Models
{
    public class Character
    {
        public string charName {get; set;}
        public int charID {get; set;}
        public List<Corp> charCorps {get; set;}
        public List<Alliance> charAlliances {get; set;}

        static void GetCorps()
        {

        }

        static void GetAlliances()
        {

        }
    }
}
using System.Collections.Generic;

namespace EveConnectionFinder.Models
{
    public class Character
    {
        string charName {get; set;}
        int charID {get; set;}
        List<Corp> charCorps = new List<Corp>();
        List<Alliance> charAlliances = new List<Alliance>();

        static void GetCorps()
        {

        }

        static void GetAlliances()
        {

        }



        


    }
}
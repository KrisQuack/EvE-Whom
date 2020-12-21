using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System;
namespace EveConnectionFinder.Models
{
    public class Character
    {
        public string charName {get; set;}
        public int charID {get; set;}
        public List<Corp> Corps {get; set;}
        public List<Alliance> Alliances {get; set;}

        public void GetCharID(){
            //Declare Web Client
            using(WebClient client = new WebClient()) {
                //Get Character ID
                string charSearchJson = client.DownloadString("https://esi.evetech.net/latest/search/?categories=character&datasource=tranquility&language=en-us&search="+this.charName+"&strict=true");
                var charSearchResult = JsonConvert.DeserializeObject<ESISearch>(charSearchJson);
                this.charID = int.Parse(charSearchResult.character[0].ToString());
            }
        }
        public void GetCorps()
        {
            //Declare Web Client
            using(WebClient client = new WebClient()) {
                //Get history list
                string corpHistoryJson = client.DownloadString("https://esi.evetech.net/latest/characters/"+this.charID+"/corporationhistory/?datasource=tranquility");
                var corpHistoryResult = JsonConvert.DeserializeObject<List<ESICorpHistory>>(corpHistoryJson);
                //Process into Corp class
                this.Corps = new List<Corp>();
                foreach(var history in corpHistoryResult){
                    //corp name
                    string corpNameSearch = client.DownloadString("https://esi.evetech.net/latest/corporations/"+history.corporation_id+"/?datasource=tranquility");
                    var corpNameSearchResult = JsonConvert.DeserializeObject<ESICorp>(corpNameSearch);
                    //
                    var corp = new Corp{
                        corpID = history.corporation_id,
                        corpName = corpNameSearchResult.name,
                        startDate = DateTime.Parse(history.start_date)
                        };
                    Corps.Add(corp);
                }
            }
        }

        static void GetAlliances()
        {
            
        }
    }
}
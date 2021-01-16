using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EveConnectionFinder.Models
{
    public class Character
    {
        public string charName {get; set;}
        public int charID {get; set;}
        public List<Corp> Corps {get; set;}
        public List<Alliance> Alliances {get; set;}

        public void GetCharID(){
            //Get Character ID
            try
            {
                string charSearchJson = DownloadString("https://esi.evetech.net/latest/search/?categories=character&datasource=tranquility&language=en-us&search=" + this.charName + "&strict=true");
                var charSearchResult = JsonConvert.DeserializeObject<ESISearch>(charSearchJson);
                this.charID = int.Parse(charSearchResult.character[0].ToString());
            }
            catch (Exception)
            {
                this.charID = 3004033;
            }
        }
        public void GetCorps()
        {
            //Get history list
            string corpHistoryJson = DownloadString("https://esi.evetech.net/latest/characters/" + this.charID + "/corporationhistory/?datasource=tranquility");
            var corpHistoryResult = JsonConvert.DeserializeObject<List<ESICorpHistory>>(corpHistoryJson);
            //Process into Corp class
            this.Corps = new List<Corp>();
            Parallel.ForEach(corpHistoryResult, history =>
            {
                //corp name
                string corpNameSearch = DownloadString("https://esi.evetech.net/latest/corporations/" + history.corporation_id + "/?datasource=tranquility");
                var corpNameSearchResult = JsonConvert.DeserializeObject<ESICorp>(corpNameSearch);
                //
                var corp = new Corp
                {
                    corpID = history.corporation_id,
                    corpName = corpNameSearchResult.name,
                    startDate = DateTime.Parse(history.start_date)
                };
                Corps.Add(corp);
            });
            //Populate end dates
            Corp previousCorp = null;
            foreach (var c in Corps.OrderByDescending(c => c.startDate))
            {
                if (previousCorp != null)
                {
                    c.endDate = previousCorp.startDate;
                }
                else
                {
                    c.endDate = DateTime.MaxValue;
                }
                previousCorp = c;
            }
        }
        public void GetAlliances()
        {
        }

        public List<CharacterConnection> FindConnections(List<Character> pastedCharacters)
        {
            var Connections = new List<CharacterConnection>();
            //Define rookie corps to ignore

            var rookieCorps = new string[] { "Hedion University", "Imperial Academy", "Royal Amarr Institute", "School of Applied Knowledge", "Science and Trade Institute", "State War Academy", "Center for Advanced Studies", "Federal Navy Academy", "University of Caille", "Pator Tech School", "Republic Military School", "Republic University", "Viziam", "Ministry of War", "Imperial Shipment", "Perkone", "Caldari Provisions", "Deep Core Mining Inc.", "The Scope", "Aliastra", "Garoun Investment Bank", "Brutor Tribe", "Sebiestor Tribe", "Native Freshfood", "Ministry of Internal Order", "Expert Distribution", "Impetus", "Brutor Tribe", "Amarr Imperial Navy", "Ytiri", "Federal Intelligence Office", "Republic Security Services", "Dominations", "Guristas"};
            //Loop Chars corp history
            foreach (var corp in this.Corps.Where(c => !rookieCorps.Contains(c.corpName)))
            {
                //get pasted chars who share that corp at some time
                foreach (var match in pastedCharacters.Where(p => p.Corps.Count(c => c.corpID == corp.corpID && c.startDate < corp.endDate && corp.startDate < c.endDate) > 0))
                {
                    var matchEntry = match.Corps.FirstOrDefault(c => c.corpID == corp.corpID && c.startDate < corp.endDate && corp.startDate < c.endDate);
                    //Calculate overlap dates
                    DateTime entityStart;
                    DateTime entityEnd;
                    if(matchEntry.startDate > corp.startDate) { entityStart = matchEntry.startDate;} else { entityStart = corp.startDate;}
                    if(matchEntry.endDate > corp.endDate) { entityEnd = corp.endDate;} else { entityEnd = matchEntry.endDate;}
                    var connection = new CharacterConnection()
                    {
                        charID = match.charID,
                        charName = match.charName,
                        entityID = corp.corpID,
                        entityName = corp.corpName,
                        overlapStart = entityStart,
                        overlapEnd = entityEnd
                    };
                    Connections.Add(connection);
                }
            }
            return Connections;
        }

        private string DownloadString(string url)
        {
            using var client = new WebClient();
            int currentTry = 0;
            while (true)
            {
                try
                {
                    return client.DownloadString(url);
                }
                catch (Exception)
                {
                    //If fails sleep for half a second and try again 3 times
                    if (++currentTry == 3) throw;
                }
            }
        }
    }
}
# Eve Whom? https://whom.kristentidmuss.dev
## Intro
This is a small web app I created as an into the the Blazor WebAssembly framework. I came up with the idea when more and more frequently I would login to the game EvE Online after long breaks and have people recognize me and I would have no idea who they were.

To solve this issue I have created Whom, Simply type your character name into the top box and either paste an individual or the entirety of the system into the second and hit submit. This will then take between a few seconds and a minute to process. Once complete a list of anyone you share mutual history with will be shown below.
![WebpageDemo](https://i.imgur.com/3X7cNYk.jpg?1)

## Code
### Architecture
For this project I decided to adopt a fully serverless architecture. To do this I am using WebAssembly for the app meaning all processing is done on the client site, this client then calls the game's public API and feeds all the data back into the UI. This means I can deploy the site to Cloudflare/Github pages as a static site and have no overhead costs at all.

### HTTP Calls to the API
This being a key feature was very simple to implament, All you need to do is initialize the client and make the calls straight into an object. See example below on getting a characters corp history from their ID.
```c#
@code
{
    static HttpClient httpClient = new();
    var corpHistoryJson = await httpClient.GetFromJsonAsync<List<ESICorpHistory>>("https://esi.evetech.net/latest/characters/" + charID + "/corporationhistory/?datasource=tranquility");
}
```
With the API requests themselves I faced one major issue being that EvE's API heavily used and often times out. To deal with this I added a while loop to simply re-try the character untill the API becomes responsive again.
```c#
//Process Character List
var pastedCharacters = new List<Character>();
while (charList.Any())
{
    try
    {
        var charName = charList[0];
        var character = new Character {charName = charName };
        await character.GetCharID();
        await character.GetCorps();
        pastedCharacters.Add(character);
        //Count Progress
        charList.Remove(charName);
        CurrentCount++;
        Progress = (int) Math.Round((CurrentCount / (decimal) TotalCount) * 100);
        StateHasChanged();
    }
    catch (Exception)
    {
        Console.WriteLine("ESI error from "+charList[0]+", trying again");
    }
}
```
### Processing Character History
The process for getting all the corp history is also rather simple once you get your head around the main concepts, the main difficulty was finding the end dates of corp membership however a small foreach loop solved that.

As you can see I chose to store the data in a List object which I find just make it easier to work with. Then all I needed to do was call for the corp history then loop through each entry to get the full details of the corp which all happens in a matter of milliseconds
```c#
public async Task GetCorps()
{
//Get history list
    var corpHistoryJson = await httpClient.GetFromJsonAsync<List<ESICorpHistory>>("https://esi.evetech.net/latest/characters/" + charID + "/corporationhistory/?datasource=tranquility");
//Process into Corp class
    Corps = new List<Corp>();
    var corpIDs = corpHistoryJson.Select(c => c.corporation_id).Distinct().ToArray();
    var corpNameSearch = await httpClient.PostAsJsonAsync("https://esi.evetech.net/latest/universe/names/?datasource=tranquility", corpIDs);
    var corpNames = await corpNameSearch.Content.ReadFromJsonAsync<List<ESINames>>();
    foreach (var history in corpHistoryJson)
    {
        var corp = new Corp
        {
            corpID = history.corporation_id,
            corpName = corpNames.FirstOrDefault(c => c.id == history.corporation_id).name,
            startDate = DateTime.Parse(history.start_date)
        };
        Corps.Add(corp);
    }
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
```

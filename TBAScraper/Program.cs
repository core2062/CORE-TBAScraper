using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Console;

namespace TBAScraper
{
    class Program
    {
        static HttpClient _client = new HttpClient();
        private static string _urlRoot = "https://www.thebluealliance.com/api/v2/event";
        private static string _tbaHeader = "frc2062:TBAScraper:0.1";
        private static string _fileName = "team_tuples.txt";
        private static string _fileJs = "selectoptions.txt";

        static void Main(string[] args)
        {
            var exit = false;
            WriteLine("Welcome to TBA Scraper!");
            while (!exit)
            {
                var yearIsNum = false;
                string yearString = "";
                while (!yearIsNum)
                {
                    Write("Please enter the competition year you'd like to pull match data for:");
                    yearString = ReadLine();
                    int year;
                    yearIsNum = Int32.TryParse(yearString, out year);
                    if (!yearIsNum)
                    {
                        WriteLine("The value entered is ");
                        continue;
                    }
                }
                WriteLine("Please enter the competition code for the event to pull match data for:");
                var codeString = ReadLine();
                var yearCode = yearString + codeString;

                WriteLine("Retrieving matches...");
                var responseJsonRaw = GetEventMatches(yearCode).GetAwaiter().GetResult();

                WriteLine("Processing matches...");
                var matches = CreateMatches(responseJsonRaw);

                if (!File.Exists(_fileName))
                {
                    var file = File.Create(_fileName);
                }
                if (!File.Exists(_fileJs))
                {
                    var file = File.Create(_fileJs);
                }

                var list = "list = {";
                var selectOptions = "selectOptions = {";
                
                for(var i = 0; i < matches.Count; i++)
                {
                    var tuple = matches[i].Tuple;
                    var option = matches[i].SelectOption(i + 1);
                    list = list + tuple;
                    selectOptions = selectOptions + option;
                    if(i < matches.Count - 1)
                    {
                        list = list + ",";
                        selectOptions = selectOptions + ",";
                    }
                }

                list = list + "}";
                selectOptions = selectOptions + "}";

                File.WriteAllText(_fileName, list);
                File.WriteAllText(_fileJs, selectOptions);

                WriteLine("Matches saved to file. Please check the directory this executable is located in. Would you like to exit?");
                var exitString = ReadLine();
                if(exitString.Substring(0, 1).Equals("y", StringComparison.InvariantCultureIgnoreCase))
                {
                    exit = true;
                }
            }
        }

        static async Task<string> GetEventMatches(string yearCode)
        {
            var url = _urlRoot + "/" + yearCode + "/matches";
            var message = new HttpRequestMessage(HttpMethod.Get, url);
            message.Headers.Add("X-TBA-App-Id", _tbaHeader);
            HttpResponseMessage response = await _client.SendAsync(message);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsByteArrayAsync();
                var contentString = Encoding.UTF8.GetString(content, 0, content.Length);
                return contentString;
            }

            throw new Exception("Something caused the http request to return an unsuccessful status code. Please check your inputs and try again.");
        }

        static List<Match> CreateMatches(string rawJson)
        {
            var array = JArray.Parse(rawJson);
            var results = array.Children().ToList();
            var matches = new List<Match>();
            foreach (var result in results.Where(r => r.Value<string>("comp_level").Equals("qm")))
            {
                var match = new Match();
                match.CompLevel = result.Value<string>("comp_level") ?? String.Empty;
                match.MatchNumber = result.Value<int?>("match_number") ?? 0;
                var alliances = result.SelectToken("alliances");
                var blueTeams = alliances.SelectToken("blue.teams");
                var redTeams = alliances.SelectToken("red.teams");
                match.Teams = new string[6];
                var blueStrings = blueTeams.Values<string>();
                var redStrings = redTeams.Values<string>();
                for (var i = 0; i < match.Teams.Length; i++)
                {
                    if (i/3 > 0)
                    {
                        match.Teams[i] = redStrings.ToList()[i%3].Remove(0, 3);
                    }
                    else
                    {
                        match.Teams[i] = blueStrings.ToList()[i].Remove(0, 3);
                    }
                }
                matches.Add(match);
            }
            matches = matches.OrderBy(m => m.MatchNumber).ToList();
            return matches;
        }
    }
}

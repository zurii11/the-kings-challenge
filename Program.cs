using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Program
{
    private readonly static HttpClient client = new HttpClient();
    private readonly static string url = "https://gist.githubusercontent.com/christianpanton/10d65ccef9f29de3acd49d97ed423736/raw/b09563bc0c4b318132c7a738e679d4f984ef0048/kings";

    public static async Task Main()
    {
        int amount_of_monarchs = 0;
        string longer_ruler_name;

        HttpResponseMessage response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string json_response = await response.Content.ReadAsStringAsync();

        List<KingRecord> kings_list = JsonSerializer.Deserialize<List<KingRecord>>(json_response);
        amount_of_monarchs = kings_list.Count();

        var longest_ruler_king = kings_list
                                    .Select(k => new
                                    {
                                        King = k,
                                        Years = ParseYears(k.Years)
                                    })
                                    .OrderByDescending(k => k.Years)
                                    .FirstOrDefault();

        var longest_ruler_house = kings_list
                                    .Select(k => new
                                    {
                                        King = k,
                                        Years = ParseYears(k.Years)
                                    })
                                    .AggregateBy(
                                        keySelector: record => record.King.House,
                                        seed: 0,
                                        func: (accumulator, record) => accumulator + record.Years
                                    )
                                    .FirstOrDefault();

        var most_common_name = kings_list
                                .Select(k => new
                                {
                                    Name = ParseName(k.Name)
                                })
                                .CountBy(k => k.Name)
                                .OrderByDescending(kp => kp.Value)
                                .FirstOrDefault();
        

        Console.WriteLine($"Amount of monarchs is {amount_of_monarchs}.");
        Console.WriteLine($"Longest reigning monarch {longest_ruler_king.King.Name} ruled for {longest_ruler_king.Years} years.");
        Console.WriteLine($"Longest reigning house {longest_ruler_house.Key} ruled for {longest_ruler_house.Value} years.");
        Console.WriteLine($"Most common name is {most_common_name.Key} with {most_common_name.Value} occurences.");
    }

    private static string ParseName(string name)
    {
        return name.Split(" ")[0];
    }

    private static int ParseYears(string years)
    {
        if (string.IsNullOrWhiteSpace(years) || string.IsNullOrEmpty(years))
            return 0;


        List<string> years_list = years.Split("-").Where(s => s != "").ToList();

        if (years_list.Count() == 1)
            return 1;

        return int.Parse(years_list[1]) - int.Parse(years_list[0]);
    }

    private class KingRecord
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("nm")]
        public string Name { get; set; }
        [JsonPropertyName("cty")]
        public string Country { get; set; }
        [JsonPropertyName("hse")]
        public string House { get; set; }
        [JsonPropertyName("yrs")]
        public string Years { get; set; }
    }
}
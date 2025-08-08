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
        amount_of_monarchs = kings_list.Count;

        int longest_ruled_years = 1;
        foreach (KingRecord king in kings_list)
        {
            Console.WriteLine($"Years: {king.Years}");
            List<int> years_ruled = new List<int>();
            king.Years.Split("-").ToList().ForEach(y => years_ruled.Add(int.Parse(y)));

            // Can be sure that years in the list are in ascending order
            if (years_ruled.Count == 2 && (years_ruled[1] - years_ruled[0]) > longest_ruled_years)
            {
                longest_ruled_years = years_ruled[1] - years_ruled[0];
                longer_ruler_name = king.Name;
            }
        }

        Console.WriteLine($"Amount of monarchs: {amount_of_monarchs}");
        Console.WriteLine($"Longest reigning monarch: {longer_ruler_name}");
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
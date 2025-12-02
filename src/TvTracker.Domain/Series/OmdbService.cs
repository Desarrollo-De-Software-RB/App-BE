using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace TvTracker.Series
{
    public class OmdbService : ISeriesApiService, ITransientDependency
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public OmdbService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<ICollection<Serie>> GetSeriesAsync(string title, string? genre)
        {
            var apiKey = _configuration["Omdb:ApiKey"];
            var baseUrl = "http://www.omdbapi.com/";

            using var client = _httpClientFactory.CreateClient();

            List<Serie> series = new List<Serie>();

            // 1. Search by title
            string searchUrl = $"{baseUrl}?s={title}&apikey={apiKey}&type=series";

            try
            {
                var response = await client.GetAsync(searchUrl);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var searchResponse = JsonConvert.DeserializeObject<SearchResponse>(jsonResponse);

                var searchResults = searchResponse?.Search ?? new List<SerieOmdb>();

                // 2. For each result, fetch full details
                foreach (var searchResult in searchResults)
                {
                    string detailUrl = $"{baseUrl}?i={searchResult.IMDBID}&apikey={apiKey}&plot=full";
                    var detailResponse = await client.GetAsync(detailUrl);
                    
                    if (detailResponse.IsSuccessStatusCode)
                    {
                        string detailJson = await detailResponse.Content.ReadAsStringAsync();
                        var fullDetail = JsonConvert.DeserializeObject<SerieOmdb>(detailJson);

                        // 3. Filter by genre if provided
                        if (!string.IsNullOrEmpty(genre))
                        {
                            Console.WriteLine($"Filtering by genre: '{genre}'. Found genre: '{fullDetail.Genre}'");
                            if (string.IsNullOrEmpty(fullDetail.Genre) || 
                                !fullDetail.Genre.Contains(genre, StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("Filtered out.");
                                continue;
                            }
                            Console.WriteLine("Match found.");
                        }

                        // 4. Map to Serie entity
                        float.TryParse(fullDetail.IMDBRating, NumberStyles.Any, CultureInfo.InvariantCulture, out float rating);

                        series.Add(new Serie
                        {
                            Title = fullDetail.Title,
                            Year = fullDetail.Year,
                            IMDBID = fullDetail.IMDBID,
                            Type = fullDetail.Type,
                            Poster = fullDetail.Poster,
                            Genre = fullDetail.Genre,
                            Plot = fullDetail.Plot,
                            Actors = fullDetail.Actors,
                            IMDBRating = rating
                        });
                    }
                }

                return series;
            }
            catch (HttpRequestException e)
            {
                throw new Exception("Se ha producido un error en la búsqueda de la serie", e);
            }
        }

        private class SearchResponse
        {
            [JsonProperty("Search")]
            public List<SerieOmdb> Search { get; set; }
        }
        
        private class SerieOmdb
        {
            public string Title { get; set; }
            public string Year { get; set; }
            public string IMDBID { get; set; }
            public string Type { get; set; }
            public string Poster { get; set; }
            public string Genre { get; set; }
            public string Plot { get; set; }
            public string Actors { get; set; }
            public string IMDBRating { get; set; }
        }
    }
}
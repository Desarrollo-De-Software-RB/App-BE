using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        public async Task<ICollection<Serie>> SearchByTitleAsync(string title, string? type = null)
        {
            var apiKey = _configuration["Omdb:ApiKey"];
            var baseUrl = "https://www.omdbapi.com/";

            using var client = _httpClientFactory.CreateClient();
            var searchResults = new List<SerieOmdb>();

            async Task FetchResults(string searchType)
            {
                string encodedTitle = Uri.EscapeDataString(title);
                string searchUrl = $"{baseUrl}?s={encodedTitle}&type={searchType}&apikey={apiKey}";
                var response = await client.GetAsync(searchUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var searchResponse = JsonConvert.DeserializeObject<SearchResponse>(jsonResponse);
                    if (searchResponse?.Search != null)
                    {
                        searchResults.AddRange(searchResponse.Search);
                    }
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(type) && (type.ToLower() == "movie" || type.ToLower() == "series"))
                {
                    await FetchResults(type.ToLower());
                }
                else
                {
                    await FetchResults("series");
                    await FetchResults("movie");
                }

                return searchResults.Select(s => new Serie
                {
                    Title = s.Title,
                    Year = s.Year,
                    IMDBID = s.IMDBID,
                    Type = s.Type,
                    Poster = s.Poster
                }).ToList();
            }
            catch (HttpRequestException e)
            {
                throw new Exception("Se ha producido un error en la búsqueda de la serie", e);
            }
        }

        public async Task<Serie> GetSerieDetailsAsync(string imdbId)
        {
            var apiKey = _configuration["Omdb:ApiKey"];
            var baseUrl = "https://www.omdbapi.com/";

            using var client = _httpClientFactory.CreateClient();
            string detailUrl = $"{baseUrl}?i={imdbId}&apikey={apiKey}";
            var detailResponse = await client.GetAsync(detailUrl);

            if (detailResponse.IsSuccessStatusCode)
            {
                string detailJson = await detailResponse.Content.ReadAsStringAsync();
                var fullDetail = JsonConvert.DeserializeObject<SerieOmdb>(detailJson);

                float.TryParse(fullDetail.IMDBRating, NumberStyles.Any, CultureInfo.InvariantCulture, out float rating);
                int.TryParse(fullDetail.TotalSeasons, out int totalSeasons);

                return new Serie
                {
                    Title = fullDetail.Title,
                    Year = fullDetail.Year,
                    IMDBID = fullDetail.IMDBID,
                    Type = fullDetail.Type,
                    Poster = fullDetail.Poster,
                    Genre = fullDetail.Genre,
                    Plot = fullDetail.Plot,
                    Actors = fullDetail.Actors,
                    Director = fullDetail.Director,
                    Writer = fullDetail.Writer,
                    Language = fullDetail.Language,
                    Country = fullDetail.Country,
                    Awards = fullDetail.Awards,
                    Metascore = fullDetail.Metascore,
                    IMDBRating = rating,
                    IMDBVotes = fullDetail.IMDBVotes,
                    Released = fullDetail.Released,
                    Runtime = fullDetail.Runtime,
                    Rated = fullDetail.Rated,
                    TotalSeasons = totalSeasons
                };
            }

            return null;
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
            public string Director { get; set; }
            public string Writer { get; set; }
            public string Language { get; set; }
            public string Country { get; set; }
            public string Awards { get; set; }
            public string Metascore { get; set; }
            public string IMDBRating { get; set; }
            public string IMDBVotes { get; set; }
            public string Released { get; set; }
            public string Runtime { get; set; }
            public string Rated { get; set; }
            public string TotalSeasons { get; set; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace TvTracker.Series
{
    public class OmdbService : ISeriesApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;  // Reemplaza con tu clave API de OMDb.
        private const string OmdbApiUrl = "http://www.omdbapi.com/";
        public class OmdbOptions
        {
            public string ApiKey { get; set; }
        }
        public OmdbService(HttpClient httpClient, IOptions<OmdbOptions> options)
        {
            _httpClient = httpClient;
            _apiKey = options.Value.ApiKey;
        }

        public async Task<ICollection<SerieDto>> GetSeriesAsync(string title, string genre)
        {
            var url = $"?apikey={_apiKey}&s={title}&type=series";

            var response = await _httpClient.GetStringAsync(url);
            Console.WriteLine(response); // Para depuración
            var omdbResponse = System.Text.Json.JsonSerializer.Deserialize<OmdbApiResponse>(response);

            var series = new List<SerieDto>();
            if (omdbResponse.Search != null)
            {
                foreach (var omdbSerie in omdbResponse.Search)
                {
                    series.Add(new SerieDto
                    {
                        Title = omdbSerie.Title,
                        Year = omdbSerie.Year,
                        IMDBID = omdbSerie.imdbID,
                        Poster = omdbSerie.Poster
                    });
                }
            }

            return series;
        }
        // Clases de apoyo para mapear la respuesta de OMDb.
        public class OmdbApiResponse
        {
            public List<OmdbSerie> Search { get; set; }
            public string totalResults { get; set; }
            public string Response { get; set; }
        }

        public class OmdbSerie
        {
            public string Title { get; set; }
            public string Year { get; set; }
            public string imdbID { get; set; }
            public string Type { get; set; }
            public string Poster { get; set; }
        }
    }
}

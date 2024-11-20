using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace TvTracker.Series
{
    public class OmdbService : ISeriesApiService
    {
        private static readonly string apiKey = "80da1536"; // Reemplaza con tu clave API de OMDb.
        private static readonly string baseUrl = "http://www.omdbapi.com/";

        public async Task<ICollection<SerieDto>> GetSeriesAsync(string title, string gender)
        {
            using HttpClient client = new HttpClient();

            List<SerieDto> series = new List<SerieDto>();

            string url = $"{baseUrl}?s={title}&apikey={apiKey}&type=series";

            try
            {
                // Hacer la solicitud HTTP y obtener la respuesta como string
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserializar la respuesta JSON a un objeto SearchResponse
                var searchResponse = JsonConvert.DeserializeObject<SearchResponse>(jsonResponse);

                // Retornar la lista de series si existen
                var seriesOmdb = searchResponse?.Search ?? new List<SerieOmdb>();

                foreach (var serieOmdb in seriesOmdb)
                {
                    series.Add(new SerieDto { 
                        Title = serieOmdb.Title ,
                        Year = serieOmdb.Year ,
                        IMDBID = serieOmdb.IMDBID,
                        Type = serieOmdb.Type ,
                        Poster = serieOmdb.Poster ,
                    });
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
        }
    }
}
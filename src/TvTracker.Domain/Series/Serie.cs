using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Auditing;

namespace TvTracker.Series
{
    public class Serie : AggregateRoot<int>, IMustHaveCreator<Guid>
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string Rated { get; set; }
        public string Released { get; set; }
        public string Runtime { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Writer { get; set; }
        public string Actors { get; set; }
        public string Plot { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string Awards { get; set; }
        public string Poster { get; set; }
        public string Metascore { get; set; }
        public float IMDBRating { get; set; }
        public string IMDBVotes { get; set; }
        public string IMDBID { get; set; }
        public string Type { get; set; }
        public int TotalSeasons { get; set; }
        public Guid Creator { get; set; }
        public Guid CreatorId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace TvTracker.Series
{
    public class Rating : AggregateRoot<int>
    {
        public int SerieId { get; set; }
        public Serie Serie { get; set; } // Relación con la serie
        public Guid UserId { get; set; } // Usuario que hizo la calificación
        public int Score { get; set; } // Puntuación de 1 a 5
        public string? Comment { get; set; } // Comentario opcional
    }
}
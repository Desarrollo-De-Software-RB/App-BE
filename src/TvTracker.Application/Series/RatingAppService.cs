using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TvTracker.Series
{
    public class RatingAppService : ApplicationService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IRepository<Serie, int> _serieRepository;

        public RatingAppService(IRatingRepository ratingRepository, IRepository<Serie, int> serieRepository)
        {
            _ratingRepository = ratingRepository;
            _serieRepository = serieRepository;
        }

        public async Task CreateOrUpdateRatingAsync(Guid userId, int serieId, int score, string? comment = null)
        {
            var serie = await _serieRepository.GetAsync(serieId);
            if (serie.CreatorId != userId)
            {
                throw new UnauthorizedAccessException("No puedes calificar series de otros usuarios.");
            }

            var existingRating = await _ratingRepository.GetRatingByUserAndSerieAsync(userId, serieId);
            if (existingRating != null)
            {
                if (score < 1 || score > 5)
                {
                    throw new ArgumentException("Score must be between 1 and 5.");
                }
                existingRating.Score = score;
                existingRating.Comment = comment;
                await _ratingRepository.UpdateAsync(existingRating);
            }
            else
            {
                if (score < 1 || score > 5)
                {
                    throw new ArgumentException("Score must be between 1 and 5.");
                }
                var newRating = new Rating
                {
                    SerieId = serieId,
                    UserId = userId,
                    Score = score,
                    Comment = comment
                };
                await _ratingRepository.InsertAsync(newRating);
            }
        }
    }

}

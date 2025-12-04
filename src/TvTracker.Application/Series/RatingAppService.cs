using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace TvTracker.Series
{
    public class RatingAppService : ApplicationService, IRatingAppService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;

        public RatingAppService(
            IRatingRepository ratingRepository, 
            IRepository<Serie, int> serieRepository,
            IRepository<IdentityUser, Guid> userRepository)
        {
            _ratingRepository = ratingRepository;
            _serieRepository = serieRepository;
            _userRepository = userRepository;
        }

        public async Task<List<RatingDto>> GetSeriesRatingsAsync(int serieId)
        {
            var ratings = await _ratingRepository.GetRatingsBySerieAsync(serieId);
            var userIds = ratings.Select(r => r.UserId).Distinct().ToList();
            var users = await _userRepository.GetListAsync(u => userIds.Contains(u.Id));
            var userDictionary = users.ToDictionary(u => u.Id, u => u.UserName);

            return ratings.Select(r => new RatingDto
            {
                Id = r.Id,
                SerieId = r.SerieId,
                UserId = r.UserId,
                UserName = userDictionary.ContainsKey(r.UserId) ? userDictionary[r.UserId] : "Unknown",
                Score = r.Score,
                Comment = r.Comment
            }).ToList();
        }

        public async Task RateSeriesAsync(CreateUpdateRatingDto input)
        {
            var userId = CurrentUser.Id;
            if (userId == null)
            {
                throw new UnauthorizedAccessException("You must be logged in to rate.");
            }

            var serie = await _serieRepository.GetAsync(input.SerieId);
            if (serie == null)
            {
                throw new ArgumentException("Serie not found.");
            }

            var existingRating = await _ratingRepository.GetRatingByUserAndSerieAsync(userId.Value, input.SerieId);
            if (existingRating != null)
            {
                existingRating.Score = input.Score;
                existingRating.Comment = input.Comment;
                await _ratingRepository.UpdateAsync(existingRating);
            }
            else
            {
                var newRating = new Rating
                {
                    SerieId = input.SerieId,
                    UserId = userId.Value,
                    Score = input.Score,
                    Comment = input.Comment
                };
                await _ratingRepository.InsertAsync(newRating);
            }
        }
    }
}

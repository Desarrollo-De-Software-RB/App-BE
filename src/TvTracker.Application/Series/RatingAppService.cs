using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Data;
using Microsoft.Extensions.Logging;

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
                Comment = r.Comment,
                ProfilePictureUrl = GetProfilePictureUrl(users.First(u => u.Id == r.UserId))
            }).ToList();
        }

        private string? GetProfilePictureUrl(IdentityUser user)
        {
            Logger.LogInformation($"[DEBUG] Checking profile picture for user: {user.UserName} ({user.Id})");

            var profilePicture = user.GetProperty<string>("ProfilePicture");
            if (!string.IsNullOrEmpty(profilePicture))
            {
                Logger.LogInformation($"[DEBUG] Found 'ProfilePicture': {profilePicture.Substring(0, Math.Min(20, profilePicture.Length))}...");
                return profilePicture;
            }

            // Fallback to "picture" if "ProfilePicture" is empty
            var picture = user.GetProperty<string>("picture");
            if (!string.IsNullOrEmpty(picture))
            {
                Logger.LogInformation($"[DEBUG] Found 'picture': {picture.Substring(0, Math.Min(20, picture.Length))}...");
                return picture;
            }

            Logger.LogInformation("[DEBUG] No profile picture found.");
            return null;
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

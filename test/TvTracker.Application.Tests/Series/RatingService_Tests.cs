using System;
using System.Threading.Tasks;
using NSubstitute;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using Xunit;

namespace TvTracker.Series
{
    public class RatingAppService_Tests
    {
        [Fact]
        public async Task Should_Create_New_Rating()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var serieId = 1;

            var ratingRepository = Substitute.For<IRatingRepository>();
            var serieRepository = Substitute.For<IRepository<Serie, int>>();
            var userRepository = Substitute.For<IRepository<IdentityUser, Guid>>();
            var currentUser = Substitute.For<ICurrentUser>();
            var weakServiceProvider = Substitute.For<IAbpLazyServiceProvider>();

            currentUser.Id.Returns(userId);
            weakServiceProvider.LazyGetRequiredService<ICurrentUser>().Returns(currentUser);

            var serie = new Serie { CreatorId = userId };
            serieRepository.GetAsync(serieId).Returns(serie);

            var appService = new RatingAppService(ratingRepository, serieRepository, userRepository);
            appService.LazyServiceProvider = weakServiceProvider;

            var input = new CreateUpdateRatingDto
            {
                SerieId = serieId,
                Score = 5
            };

            // Act
            await appService.RateSeriesAsync(input);

            // Assert
            await ratingRepository.Received(1).InsertAsync(Arg.Is<Rating>(r =>
                r.SerieId == serieId &&
                r.UserId == userId &&
                r.Score == 5));
        }

        [Fact]
        public async Task Should_Update_Existing_Rating()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var serieId = 1;

            var existingRating = new Rating
            {
                SerieId = serieId,
                UserId = userId,
                Score = 3,
                Comment = "Buena serie"
            };

            var ratingRepository = Substitute.For<IRatingRepository>();
            ratingRepository.GetRatingByUserAndSerieAsync(userId, serieId).Returns(existingRating);

            var serieRepository = Substitute.For<IRepository<Serie, int>>();
            var serie = new Serie { CreatorId = userId };
            serieRepository.GetAsync(serieId).Returns(serie);
            
            var userRepository = Substitute.For<IRepository<IdentityUser, Guid>>();
            var currentUser = Substitute.For<ICurrentUser>();
            var weakServiceProvider = Substitute.For<IAbpLazyServiceProvider>();

            currentUser.Id.Returns(userId);
            weakServiceProvider.LazyGetRequiredService<ICurrentUser>().Returns(currentUser);

            var appService = new RatingAppService(ratingRepository, serieRepository, userRepository);
            appService.LazyServiceProvider = weakServiceProvider;

            var input = new CreateUpdateRatingDto
            {
                SerieId = serieId,
                Score = 4,
                Comment = "Excelente serie!"
            };

            // Act
            await appService.RateSeriesAsync(input);

            // Assert
            await ratingRepository.Received(1).UpdateAsync(Arg.Is<Rating>(r =>
                r.SerieId == serieId &&
                r.UserId == userId &&
                r.Score == 4 &&
                r.Comment == "Excelente serie!"));
        }

        [Fact]
        public async Task Should_Not_Modify_Other_User_Rating_Logic_Changed()
        {
            // Logic change note: The service now uses CurrentUser.Id. 
            // So if we simulate a different user logged in (user2), they won't find the rating of user1 via GetRatingByUserAndSerieAsync(userId, ...)
            // unless the logic allows fetching others' ratings for update?
            // The service code: GetRatingByUserAndSerieAsync(userId.Value, input.SerieId);
            // So if userId is user2, it looks for user2's rating.
            // If user2 tries to rate, it will be a NEW rating for user2, it won't touch user1's rating.
            // So the test 'Should_Not_Modify_Other_User_Rating' is effectively 'Should_Create_Rating_If_Not_Exists_For_User'.

            // Arrange
            var user1Id = Guid.NewGuid();
            var user2Id = Guid.NewGuid();
            var serieId = 1;

            // Existing rating for user1
            var existingRatingUser1 = new Rating
            {
                SerieId = serieId,
                UserId = user1Id,
                Score = 5,
                Comment = "Excelente!!!"
            };

            var ratingRepository = Substitute.For<IRatingRepository>();
            // Repository will return null for user2
            ratingRepository.GetRatingByUserAndSerieAsync(user2Id, serieId).Returns((Rating?)null); 

            var serieRepository = Substitute.For<IRepository<Serie, int>>();
            var serie = new Serie { CreatorId = user1Id };
            serieRepository.GetAsync(serieId).Returns(serie);

            var userRepository = Substitute.For<IRepository<IdentityUser, Guid>>();
            var currentUser = Substitute.For<ICurrentUser>();
            var weakServiceProvider = Substitute.For<IAbpLazyServiceProvider>();

            // Simulate User2 logged in
            currentUser.Id.Returns(user2Id);
            weakServiceProvider.LazyGetRequiredService<ICurrentUser>().Returns(currentUser);

            var appService = new RatingAppService(ratingRepository, serieRepository, userRepository);
            appService.LazyServiceProvider = weakServiceProvider;

            var input = new CreateUpdateRatingDto
            {
                SerieId = serieId,
                Score = 3,
                Comment = "Buena serie"
            };

            // Act
            await appService.RateSeriesAsync(input);

            // Assert
            // It should insert a NEW rating for user2
            await ratingRepository.Received(1).InsertAsync(Arg.Is<Rating>(r =>
                r.SerieId == serieId &&
                r.UserId == user2Id &&
                r.Score == 3));

            // It should NOT update any rating
            await ratingRepository.DidNotReceive().UpdateAsync(Arg.Any<Rating>());
        }
    }
}

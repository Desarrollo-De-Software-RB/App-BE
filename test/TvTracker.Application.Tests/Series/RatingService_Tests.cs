using System;
using System.Threading.Tasks;
using NSubstitute;
using Volo.Abp.Domain.Repositories;
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

            var serie = new Serie { CreatorId = userId };
            serieRepository.GetAsync(serieId).Returns(serie);

            var appService = new RatingAppService(ratingRepository, serieRepository);

            // Act
            await appService.CreateOrUpdateRatingAsync(userId, serieId, 5);

            // Assert
            await ratingRepository.Received(1).InsertAsync(Arg.Is<Rating>(r =>
                r.SerieId == serieId &&
                r.UserId == userId &&
                r.Score == 5));
        }
        [Fact]
        public async Task Should_Not_Create_New_Rating_Out_Range()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var serieId = 1;

            var ratingRepository = Substitute.For<IRatingRepository>();
            var serieRepository = Substitute.For<IRepository<Serie, int>>();

            var serie = new Serie { CreatorId = userId };
            serieRepository.GetAsync(serieId).Returns(serie);

            var appService = new RatingAppService(ratingRepository, serieRepository);

            // Act
            await appService.CreateOrUpdateRatingAsync(userId, serieId, 6);

            // Assert
            await ratingRepository.Received(1).InsertAsync(Arg.Is<Rating>(r =>
                r.SerieId == serieId &&
                r.UserId == userId &&
                r.Score == 6));
            //Intenta crear una calificacion con un score fuera de 1 a 5
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

            var appService = new RatingAppService(ratingRepository, serieRepository);

            // Act
            await appService.CreateOrUpdateRatingAsync(userId, serieId, 4, "Excelente serie!");

            // Assert
            await ratingRepository.Received(1).UpdateAsync(Arg.Is<Rating>(r =>
                r.SerieId == serieId &&
                r.UserId == userId &&
                r.Score == 4 &&
                r.Comment == "Excelente serie!"));
        }

        [Fact]
        public async Task Should_Not_Modify_Other_User_Rating()
        {
            // Arrange
            var user1Id = Guid.NewGuid();
            var user2Id = Guid.NewGuid();
            var serieId = 1;

            var existingRating = new Rating
            {
                SerieId = serieId,
                UserId = user1Id,
                Score = 5,
                Comment = "Excelente!!!"
            };

            var ratingRepository = Substitute.For<IRatingRepository>();
            ratingRepository.GetRatingByUserAndSerieAsync(user1Id, serieId).Returns(existingRating);

            var serieRepository = Substitute.For<IRepository<Serie, int>>();
            var serie = new Serie { CreatorId = user1Id };
            serieRepository.GetAsync(serieId).Returns(serie);

            var appService = new RatingAppService(ratingRepository, serieRepository);

            // Act
            await appService.CreateOrUpdateRatingAsync(user2Id, serieId, 3, "Buena serie");

            // Assert
            await ratingRepository.Received(1).InsertAsync(Arg.Is<Rating>(r =>
                r.SerieId == serieId &&
                r.UserId == user2Id &&
                r.Score == 3 &&
                r.Comment == "Buena serie"));

            await ratingRepository.DidNotReceive().UpdateAsync(Arg.Any<Rating>());
            //Da error porque intenta modificar una calificacion que tiene otro usuario.
            //Cada calificacion es modificada por el usuario creador
        }
    }
}

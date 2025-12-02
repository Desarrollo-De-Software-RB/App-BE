using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using TvTracker.Series;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Xunit;

namespace TvTracker.Series;

public class SerieAppServiceTests
{
    private readonly Mock<IRepository<Serie, int>> _serieRepositoryMock;
    private readonly Mock<ISeriesApiService> _seriesApiServiceMock;
    private readonly Mock<IObjectMapper> _objectMapperMock;
    private readonly SerieAppService _serieAppService;

    public SerieAppServiceTests()
    {
        _serieRepositoryMock = new Mock<IRepository<Serie, int>>();
        _seriesApiServiceMock = new Mock<ISeriesApiService>();
        _objectMapperMock = new Mock<IObjectMapper>();

        _serieAppService = new SerieAppService(_serieRepositoryMock.Object, _seriesApiServiceMock.Object);
        _serieAppService.LazyServiceProvider = new FakeLazyServiceProvider(_objectMapperMock.Object);
    }

    public class FakeLazyServiceProvider : IAbpLazyServiceProvider, IKeyedServiceProvider, ICachedServiceProvider
    {
        private readonly IObjectMapper _objectMapper;
        public FakeLazyServiceProvider(IObjectMapper objectMapper) { _objectMapper = objectMapper; }
        
        public T LazyGetRequiredService<T>(Func<IServiceProvider, object> factory = null)
        {
            if (typeof(T) == typeof(IObjectMapper)) return (T)_objectMapper;
            return default;
        }

        public object LazyGetRequiredService(Type serviceType, Func<IServiceProvider, object> factory = null)
        {
            if (serviceType == typeof(IObjectMapper)) return _objectMapper;
            return null;
        }

        public T LazyGetRequiredService<T>()
        {
            if (typeof(T) == typeof(IObjectMapper)) return (T)_objectMapper;
            return default;
        }

        public object LazyGetRequiredService(Type serviceType)
        {
            if (serviceType == typeof(IObjectMapper)) return _objectMapper;
            return null;
        }
        
        public T LazyGetService<T>(T defaultValue = default, Func<IServiceProvider, object> factory = null) => defaultValue;
        public object LazyGetService(Type serviceType, object defaultValue = null, Func<IServiceProvider, object> factory = null) => defaultValue;
        
        public object GetService(Type serviceType)
        {
             if (serviceType == typeof(IObjectMapper)) return _objectMapper;
             return null;
        }

        public object GetKeyedService(Type serviceType, object? serviceKey) => null;
        public object GetRequiredKeyedService(Type serviceType, object? serviceKey) => null;

        public object GetService(Type serviceType, Func<IServiceProvider, object> factory)
        {
             if (serviceType == typeof(IObjectMapper)) return _objectMapper;
             return null;
        }

        public object GetService(Type serviceType, object defaultValue)
        {
             if (serviceType == typeof(IObjectMapper)) return _objectMapper;
             return defaultValue;
        }

        public T GetService<T>(T defaultValue)
        {
            if (typeof(T) == typeof(IObjectMapper)) return (T)_objectMapper;
            return defaultValue;
        }

        public T GetService<T>(Func<IServiceProvider, object> factory)
        {
            if (typeof(T) == typeof(IObjectMapper)) return (T)_objectMapper;
            return default;
        }

        public object LazyGetService(Type serviceType, Func<IServiceProvider, object> factory = null)
        {
            if (serviceType == typeof(IObjectMapper)) return _objectMapper;
            return null;
        }

        public object LazyGetService(Type serviceType, object defaultValue)
        {
            if (serviceType == typeof(IObjectMapper)) return _objectMapper;
            return defaultValue;
        }

        public T LazyGetService<T>(T defaultValue)
        {
            if (typeof(T) == typeof(IObjectMapper)) return (T)_objectMapper;
            return defaultValue;
        }

        public object LazyGetService(Type serviceType)
        {
            if (serviceType == typeof(IObjectMapper)) return _objectMapper;
            return null;
        }

        public T LazyGetService<T>()
        {
            if (typeof(T) == typeof(IObjectMapper)) return (T)_objectMapper;
            return default;
        }

        public T LazyGetService<T>(Func<IServiceProvider, object> factory)
        {
            if (typeof(T) == typeof(IObjectMapper)) return (T)_objectMapper;
            return default;
        }
    }

    [Fact]
    public async Task SearchAsync_Should_Fetch_From_Api_And_Save_To_Db_If_Not_Exists()
    {
        // Arrange
        var title = "Test Series";
        var imdbId = "tt1234567";
        
        var apiSearchResults = new List<Serie>
        {
            new Serie { Title = title, IMDBID = imdbId, Type = "series" }
        };

        var fullSerieDetails = new Serie
        {
            Title = title,
            IMDBID = imdbId,
            Type = "series",
            Genre = "Action"
        };

        _seriesApiServiceMock.Setup(x => x.SearchByTitleAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(apiSearchResults);

        _seriesApiServiceMock.Setup(x => x.GetSerieDetailsAsync(imdbId))
            .ReturnsAsync(fullSerieDetails);

        var dbData = new List<Serie>();
        
        _serieRepositoryMock.Setup(x => x.GetQueryableAsync())
            .ReturnsAsync(() => dbData.AsQueryable());


        _serieRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<Serie>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Callback<Serie, bool, CancellationToken>((s, b, c) => dbData.Add(s))
            .ReturnsAsync((Serie s, bool b, CancellationToken c) => s);

        // Mock Mapping
        var serieDtos = new List<SerieDto> { new SerieDto { Title = title, IMDBID = imdbId } };
        _objectMapperMock.Setup(x => x.Map<ICollection<Serie>, ICollection<SerieDto>>(It.IsAny<ICollection<Serie>>()))
            .Returns(serieDtos);

        // Act
        var result = await _serieAppService.SearchAsync(title, null);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result.First().Title.ShouldBe(title);

        // Verify it was saved to DB with autoSave: true
        _serieRepositoryMock.Verify(x => x.InsertAsync(It.Is<Serie>(s => s.IMDBID == imdbId), true, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_Should_Return_From_Db_If_Exists()
    {
        // Arrange
        var title = "Existing Series";
        var imdbId = "tt7654321";
        
        var existingSerie = new Serie
        {
            Title = title,
            IMDBID = imdbId,
            Type = "series",
            Genre = "Drama"
        };

        var dbData = new List<Serie> { existingSerie };

        _serieRepositoryMock.Setup(x => x.GetQueryableAsync())
            .ReturnsAsync(() => dbData.AsQueryable());


        var apiSearchResults = new List<Serie>
        {
            new Serie { Title = title, IMDBID = imdbId, Type = "series" }
        };

        _seriesApiServiceMock.Setup(x => x.SearchByTitleAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(apiSearchResults);

        // Mock Mapping
        var serieDtos = new List<SerieDto> { new SerieDto { Title = title, IMDBID = imdbId } };
        _objectMapperMock.Setup(x => x.Map<ICollection<Serie>, ICollection<SerieDto>>(It.IsAny<ICollection<Serie>>()))
            .Returns(serieDtos);

        // Act
        var result = await _serieAppService.SearchAsync(title, null);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);

        // Verify GetSerieDetailsAsync was NOT called
        _seriesApiServiceMock.Verify(x => x.GetSerieDetailsAsync(imdbId), Times.Never);
        
        // Verify Insert was NOT called
        _serieRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<Serie>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByImdbIdAsync_Should_Return_From_Db_If_Exists()
    {
        // Arrange
        var imdbId = "tt9999999";
        var title = "My Series";
        
        var existingSerie = new Serie
        {
            Title = title,
            IMDBID = imdbId
        };

        var dbData = new List<Serie> { existingSerie };
        _serieRepositoryMock.Setup(x => x.GetQueryableAsync())
            .ReturnsAsync(() => dbData.AsQueryable());


        // Mock Mapping
        var serieDto = new SerieDto { Title = title, IMDBID = imdbId };
        _objectMapperMock.Setup(x => x.Map<Serie, SerieDto>(existingSerie))
            .Returns(serieDto);

        // Act
        var result = await _serieAppService.GetByImdbIdAsync(imdbId);

        // Assert
        result.ShouldNotBeNull();
        result.Title.ShouldBe(title);
        
        // Verify API was not called
        _seriesApiServiceMock.Verify(x => x.GetSerieDetailsAsync(imdbId), Times.Never);
    }

    [Fact]
    public async Task GetByImdbIdAsync_Should_Fetch_From_Api_And_Save_If_Not_Exists()
    {
        // Arrange
        var imdbId = "tt8888888";
        var title = "New Series";
        
        var fullSerieDetails = new Serie
        {
            Title = title,
            IMDBID = imdbId
        };

        var dbData = new List<Serie>();
        _serieRepositoryMock.Setup(x => x.GetQueryableAsync())
            .ReturnsAsync(() => dbData.AsQueryable());


        _serieRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<Serie>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Callback<Serie, bool, CancellationToken>((s, b, c) => dbData.Add(s))
            .ReturnsAsync((Serie s, bool b, CancellationToken c) => s);

        _seriesApiServiceMock.Setup(x => x.GetSerieDetailsAsync(imdbId))
            .ReturnsAsync(fullSerieDetails);

        // Mock Mapping
        var serieDto = new SerieDto { Title = title, IMDBID = imdbId };
        _objectMapperMock.Setup(x => x.Map<Serie, SerieDto>(fullSerieDetails))
            .Returns(serieDto);

        // Act
        var result = await _serieAppService.GetByImdbIdAsync(imdbId);

        // Assert
        result.ShouldNotBeNull();
        result.Title.ShouldBe(title);

        // Verify it was saved to DB with autoSave: true
        _serieRepositoryMock.Verify(x => x.InsertAsync(It.Is<Serie>(s => s.IMDBID == imdbId), true, It.IsAny<CancellationToken>()), Times.Once);
    }
}

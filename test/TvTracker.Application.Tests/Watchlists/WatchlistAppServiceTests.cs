using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using TvTracker.Series;
using TvTracker.Watchlists;
using Volo.Abp.Application.Dtos;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using Xunit;

namespace TvTracker.Watchlists;

public class WatchlistAppServiceTests
{
    private readonly Mock<IRepository<WatchlistItem, Guid>> _watchlistRepositoryMock;
    private readonly Mock<IRepository<Serie, int>> _serieRepositoryMock;
    private readonly Mock<ISeriesApiService> _seriesApiServiceMock;
    private readonly Mock<IObjectMapper> _objectMapperMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly WatchlistAppServices _watchlistAppService;

    private List<WatchlistItem> _watchlistDb;
    private List<Serie> _serieDb;

    public WatchlistAppServiceTests()
    {
        _watchlistRepositoryMock = new Mock<IRepository<WatchlistItem, Guid>>();
        _serieRepositoryMock = new Mock<IRepository<Serie, int>>();
        _seriesApiServiceMock = new Mock<ISeriesApiService>();
        _objectMapperMock = new Mock<IObjectMapper>();
        _currentUserMock = new Mock<ICurrentUser>();

        _watchlistDb = new List<WatchlistItem>();
        _serieDb = new List<Serie>();

        // Mock GetQueryableAsync
        _watchlistRepositoryMock.Setup(x => x.GetQueryableAsync())
            .ReturnsAsync(() => _watchlistDb.AsQueryable());
        
        _serieRepositoryMock.Setup(x => x.GetQueryableAsync())
            .ReturnsAsync(() => _serieDb.AsQueryable());

        // Mock InsertAsync
        _watchlistRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<WatchlistItem>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Callback<WatchlistItem, bool, CancellationToken>((w, b, c) => 
            {
                if (w.Id == Guid.Empty) SetId(w, Guid.NewGuid());
                _watchlistDb.Add(w);
            })
            .ReturnsAsync((WatchlistItem w, bool b, CancellationToken c) => w);

        _serieRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<Serie>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Callback<Serie, bool, CancellationToken>((s, b, c) => _serieDb.Add(s))
            .ReturnsAsync((Serie s, bool b, CancellationToken c) => s);

        // Mock DeleteAsync
        _watchlistRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<WatchlistItem>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Callback<WatchlistItem, bool, CancellationToken>((w, b, c) => _watchlistDb.Remove(w))
            .Returns(Task.CompletedTask);

        // Mock UpdateAsync
        _watchlistRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<WatchlistItem>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Callback<WatchlistItem, bool, CancellationToken>((w, b, c) => 
            {
                var existing = _watchlistDb.FirstOrDefault(i => i.Id == w.Id);
                if (existing != null)
                {
                    _watchlistDb.Remove(existing);
                    _watchlistDb.Add(w);
                }
            })
            .ReturnsAsync((WatchlistItem w, bool b, CancellationToken c) => w);
            
         _serieRepositoryMock.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, bool b, CancellationToken c) => _serieDb.FirstOrDefault(s => s.Id == id));


        _watchlistAppService = new WatchlistAppServices(
            _watchlistRepositoryMock.Object,
            _serieRepositoryMock.Object,
            _seriesApiServiceMock.Object
        );
        
        _watchlistAppService.LazyServiceProvider = new FakeLazyServiceProvider(_objectMapperMock.Object, _currentUserMock.Object);
    }

    private void SetId(object entity, object id) 
    {
        entity.GetType().GetProperty("Id")?.SetValue(entity, id);
    }

    [Fact]
    public async Task GetListAsync_Should_Return_Empty_If_User_Not_LoggedIn()
    {
        // Arrange
        _currentUserMock.Setup(x => x.Id).Returns((Guid?)null);

        // Act
        var result = await _watchlistAppService.GetListAsync();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetListAsync_Should_Return_Items_For_CurrentUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.Id).Returns(userId);

        var serieId = 1;
        var watchlistItem = new WatchlistItem
        {
            UserId = userId,
            SerieId = serieId,
            Status = WatchlistStatus.Watching
        };
        // Use SetId/reflection if needed or just add to DB
        
        var serie = new Serie { Title = "Test Serie", IMDBID = "tt12345" };
        SetId(serie, serieId);

        _serieDb.Add(serie);
        _watchlistDb.Add(watchlistItem);

        var serieDto = new SerieDto { Title = "Test Serie", IMDBID = "tt12345" };
        _objectMapperMock.Setup(x => x.Map<Serie, SerieDto>(serie)).Returns(serieDto);

        // Act
        var result = await _watchlistAppService.GetListAsync();

        // Assert
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(1);
        result[0].UserId.ShouldBe(userId);
        result[0].Serie.Title.ShouldBe("Test Serie");
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_If_User_Not_LoggedIn()
    {
         // Arrange
        _currentUserMock.Setup(x => x.Id).Returns((Guid?)null);
        var input = new CreateUpdateWatchlistItemDto { ImdbId = "tt123", Status = WatchlistStatus.Pending };

        // Act & Assert
        await Should.ThrowAsync<UserFriendlyException>(async () => await _watchlistAppService.CreateAsync(input));
    }

    [Fact]
    public async Task CreateAsync_Should_Add_Item_If_Not_Exists_Locally()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.Id).Returns(userId);

        var imdbId = "tt12345";
        var input = new CreateUpdateWatchlistItemDto { ImdbId = imdbId, Status = WatchlistStatus.Watching };
        
        // Serie not in DB initially
        _serieDb.Clear();

        // Fetch from API
        var fullSerie = new Serie { Title = "New Serie", IMDBID = imdbId };
        SetId(fullSerie, 10);
        
        _seriesApiServiceMock.Setup(x => x.GetSerieDetailsAsync(imdbId)).ReturnsAsync(fullSerie);

        _objectMapperMock.Setup(x => x.Map<Serie, SerieDto>(fullSerie)).Returns(new SerieDto { Title = "New Serie" });

        // Act
        var result = await _watchlistAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.SerieId.ShouldBe(fullSerie.Id);
        result.UserId.ShouldBe(userId);
        
        _watchlistDb.Count.ShouldBe(1);
        _watchlistDb.First().UserId.ShouldBe(userId);
        _watchlistDb.First().SerieId.ShouldBe(fullSerie.Id);
        
        _serieDb.Count.ShouldBe(1); // Should have been inserted
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_If_Already_In_Watchlist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.Id).Returns(userId);

        var imdbId = "tt12345";
        var input = new CreateUpdateWatchlistItemDto { ImdbId = imdbId };
        var serie = new Serie { IMDBID = imdbId };
        SetId(serie, 1);
        _serieDb.Add(serie);

        var existingItem = new WatchlistItem { UserId = userId, SerieId = serie.Id };
        _watchlistDb.Add(existingItem);
        
        // Act & Assert
        var exception = await Should.ThrowAsync<UserFriendlyException>(async () => await _watchlistAppService.CreateAsync(input));
        exception.Message.ShouldBe("Serie already in watchlist");
    }

    [Fact]
    public async Task RemoveItemAsync_Should_Delete_Item_If_Exists()
    {
         // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.Id).Returns(userId);

        var imdbId = "tt12345";
        var serie = new Serie { IMDBID = imdbId };
        SetId(serie, 5);
        _serieDb.Add(serie);
        
        var item = new WatchlistItem { UserId = userId, SerieId = serie.Id };
        SetId(item, Guid.NewGuid());
        _watchlistDb.Add(item);

        // Act
        await _watchlistAppService.RemoveItemAsync(imdbId);

        // Assert
        _watchlistDb.ShouldBeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Status()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.Id).Returns(userId);

        var imdbId = "tt12345";
        var input = new CreateUpdateWatchlistItemDto { ImdbId = imdbId, Status = WatchlistStatus.Completed };
        var serie = new Serie { IMDBID = imdbId };
        SetId(serie, 5);
        _serieDb.Add(serie);
        
        var item = new WatchlistItem { UserId = userId, SerieId = serie.Id, Status = WatchlistStatus.Pending };
        SetId(item, Guid.NewGuid());
        _watchlistDb.Add(item);

        // Act
        await _watchlistAppService.UpdateAsync(input);

        // Assert
        var updatedItem = _watchlistDb.First();
        updatedItem.Status.ShouldBe(WatchlistStatus.Completed);
    }

    public class FakeLazyServiceProvider : IAbpLazyServiceProvider, IKeyedServiceProvider, ICachedServiceProvider
    {
        private readonly IObjectMapper _objectMapper;
        private readonly ICurrentUser _currentUser;
        
        public FakeLazyServiceProvider(IObjectMapper objectMapper, ICurrentUser currentUser) 
        { 
            _objectMapper = objectMapper; 
            _currentUser = currentUser;
        }
        
        public T LazyGetRequiredService<T>(Func<IServiceProvider, object>? factory = null)
        {
            if (typeof(T) == typeof(IObjectMapper)) return (T)_objectMapper;
            if (typeof(T) == typeof(ICurrentUser)) return (T)_currentUser;
            return default!;
        }

        public object LazyGetRequiredService(Type serviceType, Func<IServiceProvider, object>? factory = null)
        {
            if (serviceType == typeof(IObjectMapper)) return _objectMapper;
            if (serviceType == typeof(ICurrentUser)) return _currentUser;
            return default!;
        }

        public T LazyGetRequiredService<T>()
        {
             if (typeof(T) == typeof(IObjectMapper)) return (T)_objectMapper;
            if (typeof(T) == typeof(ICurrentUser)) return (T)_currentUser;
            return default!;
        }

        public object LazyGetRequiredService(Type serviceType)
        {
            if (serviceType == typeof(IObjectMapper)) return _objectMapper;
             if (serviceType == typeof(ICurrentUser)) return _currentUser;
            return default!;
        }
        
        public T LazyGetService<T>(T defaultValue = default!, Func<IServiceProvider, object>? factory = null) => defaultValue;
        public object LazyGetService(Type serviceType, object defaultValue = default!, Func<IServiceProvider, object>? factory = null) => defaultValue;
        
        public object? GetService(Type serviceType)
        {
             if (serviceType == typeof(IObjectMapper)) return _objectMapper;
             if (serviceType == typeof(ICurrentUser)) return _currentUser;
             return null;
        }

        public object? GetKeyedService(Type serviceType, object? serviceKey) => null;
        public object? GetRequiredKeyedService(Type serviceType, object? serviceKey) => null;

        public object? GetService(Type serviceType, Func<IServiceProvider, object> factory)
        {
             if (serviceType == typeof(IObjectMapper)) return _objectMapper;
             if (serviceType == typeof(ICurrentUser)) return _currentUser;
             return null;
        }

        public object GetService(Type serviceType, object defaultValue)
        {
             if (serviceType == typeof(IObjectMapper)) return _objectMapper;
             if (serviceType == typeof(ICurrentUser)) return _currentUser;
             return defaultValue;
        }

        public T GetService<T>(T defaultValue)
        {
           if (typeof(T) == typeof(IObjectMapper)) return (T)_objectMapper;
            if (typeof(T) == typeof(ICurrentUser)) return (T)_currentUser;
            return defaultValue;
        }

        public T GetService<T>(Func<IServiceProvider, object> factory)
        {
            if (typeof(T) == typeof(IObjectMapper)) return (T)_objectMapper;
            if (typeof(T) == typeof(ICurrentUser)) return (T)_currentUser;
            return default!;
        }

        public object LazyGetService(Type serviceType, Func<IServiceProvider, object>? factory = null)
        {
            if (serviceType == typeof(IObjectMapper)) return _objectMapper;
            if (serviceType == typeof(ICurrentUser)) return _currentUser;
            return default!;
        }

        public object LazyGetService(Type serviceType, object defaultValue)
        {
            if (serviceType == typeof(IObjectMapper)) return _objectMapper;
            if (serviceType == typeof(ICurrentUser)) return _currentUser;
            return defaultValue;
        }

        public T LazyGetService<T>(T defaultValue)
        {
             if (typeof(T) == typeof(IObjectMapper)) return (T)_objectMapper;
            if (typeof(T) == typeof(ICurrentUser)) return (T)_currentUser;
            return defaultValue;
        }

        public object LazyGetService(Type serviceType)
        {
             if (serviceType == typeof(IObjectMapper)) return _objectMapper;
             if (serviceType == typeof(ICurrentUser)) return _currentUser;
            return default!;
        }

        public T LazyGetService<T>()
        {
             if (typeof(T) == typeof(IObjectMapper)) return (T)_objectMapper;
            if (typeof(T) == typeof(ICurrentUser)) return (T)_currentUser;
            return default!;
        }

        public T LazyGetService<T>(Func<IServiceProvider, object> factory)
        {
            if (typeof(T) == typeof(IObjectMapper)) return (T)_objectMapper;
            if (typeof(T) == typeof(ICurrentUser)) return (T)_currentUser;
            return default!;
        }
    }
}

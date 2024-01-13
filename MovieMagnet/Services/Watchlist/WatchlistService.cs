using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMagnet.Authorization;
using MovieMagnet.Entities;
using MovieMagnet.Services.Dtos;
using MovieMagnet.Services.Dtos.Watchlist;
using System.Linq;
using MovieMagnet.Data;
using MovieMagnet.Services.Dtos.Movies;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Validation;

namespace MovieMagnet.Services.Watchlist;

public class WatchlistService : MovieMagnetAppService, IWatchlistService
{
    private readonly IRepository<UserWatchList, long> _userWatchListRepository;
    private readonly IRepository<Movie, long> _movieRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public WatchlistService(IRepository<UserWatchList, long> userWatchListRepository, IRepository<Movie, long> movieRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userWatchListRepository = userWatchListRepository;
        _movieRepository = movieRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    [Authorize]
    public async Task<string> AddAsync(long movieId)
    {
        UserDto? user = _httpContextAccessor.HttpContext?.Items["User"] as UserDto ?? throw new UserFriendlyException("User not found");

        var movie = await _movieRepository.FirstOrDefaultAsync(x => x.Id == movieId);

        if(movie == null) { throw new EntityNotFoundException("Movie not found"); }


        var watchlistMovie = await _userWatchListRepository.FirstOrDefaultAsync(_ => _.MovieId == movieId);

        if(watchlistMovie != null) {
            throw new AbpValidationException("You already add this to watchlist");
        }

        await _userWatchListRepository.InsertAsync(new UserWatchList() { MovieId = movieId, UserId = user.Id });
        return "Add successfully";
    }

    [Authorize]
    public async Task<string> RemoveAsync(long movieId)
    {
        UserDto? user = _httpContextAccessor.HttpContext?.Items["User"] as UserDto ?? throw new UserFriendlyException("User not found");

        var movie = await _userWatchListRepository.FirstOrDefaultAsync(x => x.MovieId ==  movieId);

        if (movie == null) { throw new EntityNotFoundException("You not add this movie to your watchlist yet."); }

        await _userWatchListRepository.DeleteAsync(e => e.MovieId == movieId);
        return "Remove successfully";
    }

    [Authorize]
    public async Task<PagedResultDto<MovieDto>> GetAsync(PagedAndSortedResultRequestDto input)
    {
        List<MovieDto> result = new() { };

        UserDto? user = _httpContextAccessor.HttpContext?.Items["User"] as UserDto ?? throw new UserFriendlyException("User not found");
        var queryable = await _userWatchListRepository.WithDetailsAsync();

        queryable = queryable.Where(x => x.UserId == user.Id)
            .Include(x => x.Movie)
            .ThenInclude(x => x.MovieGenres)
            .ThenInclude(x => x.Genre);

        var queryResult = await AsyncExecuter.ToListAsync(queryable.Skip(input.SkipCount).Take(input.MaxResultCount));

        queryResult.ForEach(x =>
        {
            decimal? movieRating =  x.Movie.Ratings is { Count: > 0 } ? x.Movie.Ratings.Average(r => r.Score) : null;
            result.Add(new MovieDto()
            {
                Id = x.Movie.Id,
                Budget = x.Movie.Budget,
                Title = x.Movie.Title,
                Language = x.Movie.Language,
                Overview = x.Movie.Overview,
                PosterPath = x.Movie.PosterPath,
                ReleaseDate = x.Movie.ReleaseDate,
                ImdbId = x.Movie.ImdbId,
                Popularity = x.Movie.Popularity,
                Revenue = x.Movie.Revenue,
                Runtime = x.Movie.Runtime,
                VoteAverage = movieRating,
                VoteCount = x.Movie.VoteCount,
                Genres = x.Movie.MovieGenres.Select(mg => mg.Genre.Name).ToArray()
            });
        });

        return new PagedResultDto<MovieDto>(
          queryable.Count(),
         result
      );
    }
}

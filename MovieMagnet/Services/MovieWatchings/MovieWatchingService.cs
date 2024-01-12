using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMagnet.Authorization;
using MovieMagnet.Entities;
using MovieMagnet.Services.Dtos;
using MovieMagnet.Services.Dtos.MovieWatchings;
using OneOf.Types;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace MovieMagnet.Services.MovieWatchings;

[Authorize]
public class MovieWatchingService : MovieMagnetAppService, IMovieWatchingService
{
    private readonly  IRepository<MovieWatching, long> _movieWatchingRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MovieWatchingService(IRepository<MovieWatching, long> movieWatchingRepository,IHttpContextAccessor httpContextAccessor)
    {
        _movieWatchingRepository = movieWatchingRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost("movies-watching")]
    [Authorize]
    public async Task<string> AddToWatchingList(long id,string lastViewMoment)
    {
        var user = _httpContextAccessor.HttpContext?.Items["User"] as UserDto;
        await _movieWatchingRepository.InsertAsync(new MovieWatching()
        {
            MovieId = id, UserId = user.Id, lastViewMoment = lastViewMoment
        });

        return "Add successfully";
    }
    
    [HttpDelete("movies-watching")]
    [Authorize]
    public async Task<string> RemoveFromWatchingList(long id)
    {
        var user = _httpContextAccessor.HttpContext?.Items["User"] as UserDto;
        var movie = await _movieWatchingRepository.FirstOrDefaultAsync(x => x.MovieId == id && x.UserId == user.Id);
        if (movie == null)
        {
            throw new EntityNotFoundException("You not add this movie to your watchlist yet.");
        }
        await _movieWatchingRepository.DeleteAsync(e => e.MovieId == id && e.UserId == user.Id);
        return "Remove successfully";
    }
    
    [HttpGet("movies-watching")]
    [Authorize]
    public async Task<PagedResultDto<MovieDto>> GetAsync(PagedAndSortedResultRequestDto input)
    {
        var user = _httpContextAccessor.HttpContext?.Items["User"] as UserDto;
        var queryable = await _movieWatchingRepository.WithDetailsAsync();
        queryable = queryable.Where(x => x.UserId == user.Id);
        queryable = queryable
            .OrderBy(input.Sorting ?? nameof(MovieWatching.MovieId))
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .Include(x => x.Movie)
            .ThenInclude(x => x.MovieGenres)
            .ThenInclude(x => x.Genre);
        var movies = await AsyncExecuter.ToListAsync(queryable);
        List<MovieDto> result = new() { };
        movies.ForEach(x =>
        {
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
                VoteAverage = x.Movie.VoteAverage,
                VoteCount = x.Movie.VoteCount,
                Genres = x.Movie.MovieGenres.Select(mg => mg.Genre.Name).ToArray(),
            });
        });
        return new PagedResultDto<MovieDto>(
            queryable.Count(),
            result
        );
    }
}
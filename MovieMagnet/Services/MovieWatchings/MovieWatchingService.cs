using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MovieMagnet.Authorization;
using MovieMagnet.Entities;
using MovieMagnet.Services.Dtos;
using MovieMagnet.Services.Dtos.MovieWatchings;
using OneOf.Types;
using Volo.Abp;
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
    public async Task<MovieWatchingDto> AddToWatchingList(Guid id, CreateMovieWatchingDto createMovieWatchingDto)
    {
        var user = _httpContextAccessor.HttpContext?.Items["User"] as UserDto;

        if (id == null)
        {
            throw new AbpException("Movie ID is missing");
        }

        // Ensure user is provided or can be determined
        if (user == null)
        {
            throw new AbpException("User information is missing");
        }

        // Check if the movie is already in the watching list
        var existingWatching = await _movieWatchingRepository.FirstOrDefaultAsync(w => w.MovieId == id && w.UserId == user.Id);
        if (existingWatching != null)
        {
            throw new AbpException("Movie is already in your watching list");
        }

        // Create a new MovieWatching entity
        var movieWatching = new MovieWatching
        {
            MovieId = id,
            UserId = user.Id,
            lastViewMoment = createMovieWatchingDto.lastViewMoment
        };

        // Persist the entity to the database
        await _movieWatchingRepository.InsertAsync(movieWatching);

        return movieWatching;
        // Return the created MovieWatchingDto as a successful response
    }
}
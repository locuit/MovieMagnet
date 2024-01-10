using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using MovieMagnet.Authorization;
using MovieMagnet.Entities;
using MovieMagnet.Services.Dtos;
using MovieMagnet.Services.Dtos.Movies;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using static Tensorflow.Binding;


namespace MovieMagnet.Services.Movies;

[Authorize]
public class MovieService : MovieMagnetAppService, IMovieService
{
    private readonly IRepository<Movie, long> _movieRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MovieService(IRepository<Movie, long> movieRepository,IHttpContextAccessor httpContextAccessor)
    {
        _movieRepository = movieRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet("movies")]
    [AllowAnonymous]
    public async Task<PagedResultDto<MovieDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _movieRepository.WithDetailsAsync();

        queryable = queryable
            .OrderBy(input.Sorting ?? nameof(Movie.Title))
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);
        var movies = await AsyncExecuter.ToListAsync(queryable);
        return new PagedResultDto<MovieDto>(
            queryable.Count(),
            ObjectMapper.Map<List<Movie>, List<MovieDto>>(movies)
        );
    }
    
    [HttpGet("movies/{id}")]
    [AllowAnonymous]
    public async Task<MovieDto> GetAsync(long id)
    {
        var movie = await _movieRepository.GetAsync(id);
        return ObjectMapper.Map<Movie, MovieDto>(movie);
    }
    
    public async Task<MovieDto> CreateAsync(MovieDto input)
    {
        var movie = new Movie
        {
            Budget = input.Budget,
            Title = input.Title,
            Language = input.Language,
            Overview = input.Overview,
            PosterPath = input.PosterPath,
            ReleaseDate = input.ReleaseDate,
            ImdbId = input.ImdbId,
            Popularity = input.Popularity,
            Revenue = input.Revenue,
            Runtime = input.Runtime,
            VoteAverage = input.VoteAverage,
            VoteCount = input.VoteCount,
            };
        movie = await _movieRepository.InsertAsync(movie, autoSave: true);
        var movieDto = new MovieDto
        {
            Budget = movie.Budget,
            Title = movie.Title,
            Language = movie.Language,
            Overview = movie.Overview,
            PosterPath = movie.PosterPath,
            ReleaseDate = movie.ReleaseDate,
            ImdbId = movie.ImdbId,
            Popularity = movie.Popularity,
            Revenue = movie.Revenue,
            Runtime = movie.Runtime,
            VoteAverage = movie.VoteAverage,
            VoteCount = movie.VoteCount,
        };
        return movieDto;
    }
}
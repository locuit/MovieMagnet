using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using MovieMagnet.Data;
using MovieMagnet.Entities;
using MovieMagnet.Services.Dtos;
using MovieMagnet.Services.Dtos.Movies;
using NUglify.Helpers;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using static Tensorflow.Binding;



namespace MovieMagnet.Services.Movies;

public class MovieService : MovieMagnetAppService, IMovieService
{
    private readonly IRepository<Movie, long> _movieRepository;
    private readonly MovieMagnetDbContext _dbContext;


    public MovieService(IRepository<Movie, long> movieRepository,MovieMagnetDbContext dbContext)
    {
        _movieRepository = movieRepository;
        _dbContext = dbContext;
    }

    public async Task<PagedResultDto<MovieDto>> GetListAsync(PagedAndSortedResultRequestDto input, string? search)
    {
        var movies = _dbContext.Movies
            .Where(m =>
                string.IsNullOrEmpty(search) ||
                m.Title.Contains(search))
            .OrderBy(input.Sorting ?? nameof(Movie.Title))
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre);
        var movieDtos = movies.Select(entry => new MovieDto
        {
            Id = entry.Id,
            Title = entry.Title,
            Budget = entry.Budget,
            Language = entry.Language,
            Overview = entry.Overview,
            PosterPath = entry.PosterPath,
            ReleaseDate = entry.ReleaseDate,
            ImdbId = entry.ImdbId,
            Popularity = entry.Popularity,
            Revenue = entry.Revenue,
            Runtime = entry.Runtime,
            VoteAverage = entry.VoteAverage,
            VoteCount = entry.VoteCount,
            Genres = entry.MovieGenres.Select(mg => mg.Genre.Name).ToArray()
        }).ToList();

        var result = new PagedResultDto<MovieDto>
        {
            TotalCount = movieDtos.Count,
            Items = movieDtos
        };

        return result;
    }

    
    public async Task<MovieDto> GetAsync(long id)
    {
        var movie = await _dbContext.Movies
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie == null)
        {
            return null;
        }

        var movieDto = new MovieDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Budget = movie.Budget,
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
            Genres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToArray()
        };

        return movieDto;
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

    public async Task<PagedResultDto<MovieDto>> GetMoviesByGenresAsync(long genresId, PagedAndSortedResultRequestDto input)
    {
        var movies = _dbContext.Movies
            .Where(m => m.MovieGenres.Any(mg => mg.GenreId == genresId))
            .OrderBy(input.Sorting ?? nameof(Movie.Title))
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre);

       var moviesDtos = movies.Select(entry => new MovieDto
        {
            Id = entry.Id,
            Title = entry.Title,
            Budget = entry.Budget,
            Language = entry.Language,
            Overview = entry.Overview,
            PosterPath = entry.PosterPath,
            ReleaseDate = entry.ReleaseDate,
            ImdbId = entry.ImdbId,
            Popularity = entry.Popularity,
            Revenue = entry.Revenue,
            Runtime = entry.Runtime,
            VoteAverage = entry.VoteAverage,
            VoteCount = entry.VoteCount,
            Genres = entry.MovieGenres.Select(mg => mg.Genre.Name).ToArray()
        }).ToList();

       var result = new PagedResultDto<MovieDto>
       {
           TotalCount = moviesDtos.Count,
           Items = moviesDtos
       };

       return result;
    }

    public async Task<PagedResultDto<MovieDto>> GetTopRated(PagedAndSortedResultRequestDto input)
    {

        decimal meanVote = await CalculateMeanVote();
        const decimal minVotesRequired = 7;

        var movies = _dbContext.Movies
            .Where(movie => movie.VoteAverage >= minVotesRequired)
            .OrderByDescending(movie =>
                ((movie.VoteCount / (movie.VoteCount + meanVote)) * movie.VoteAverage)
                + ((meanVote / (movie.VoteCount + meanVote)) * meanVote))
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre);
        

        var movieDtos = movies.Select(entry => new MovieDto()
        {
            Id = entry.Id,
            Title = entry.Title,
            Budget = entry.Budget,
            Language = entry.Language,
            Overview = entry.Overview,
            PosterPath = entry.PosterPath,
            ReleaseDate = entry.ReleaseDate,
            ImdbId = entry.ImdbId,
            Popularity = entry.Popularity,
            Revenue = entry.Revenue,
            Runtime = entry.Runtime,
            VoteAverage = entry.VoteAverage,
            VoteCount = entry.VoteCount,
            Genres = entry.MovieGenres.Select(mg => mg.Genre.Name).ToArray()
        }).ToList();
        
        var result = new PagedResultDto<MovieDto>
        {
            TotalCount = movieDtos.Count,
            Items = movieDtos
        };
        
        return result;
            
    }

    private async Task<decimal> CalculateMeanVote()
    {
        IQueryable queryable = await _movieRepository.WithDetailsAsync();

        decimal totalVotes = await _movieRepository.SumAsync(x => x.VoteAverage);
        int totalMovies = await _movieRepository.CountAsync();

        return totalVotes / totalMovies;
    }

    public async Task<PagedResultDto<MovieDto>> GetRandom(PagedAndSortedResultRequestDto input)
    {
        Random rnd = new Random();
    
        var movies = _dbContext.Movies
            .Where(x => true)
            .OrderBy(x => EF.Functions.Random())
            .ThenBy(input.Sorting ?? nameof(Movie.Title))
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre);
       
        var movieDtos = movies.Select(entry => new MovieDto()
        {
            Id = entry.Id,
            Title = entry.Title,
            Budget = entry.Budget,
            Language = entry.Language,
            Overview = entry.Overview,
            PosterPath = entry.PosterPath,
            ReleaseDate = entry.ReleaseDate,
            ImdbId = entry.ImdbId,
            Popularity = entry.Popularity,
            Revenue = entry.Revenue,
            Runtime = entry.Runtime,
            VoteAverage = entry.VoteAverage,
            VoteCount = entry.VoteCount,
            Genres = entry.MovieGenres.Select(mg => mg.Genre.Name).ToArray()
        }).ToList();
        
        var result = new PagedResultDto<MovieDto>
        {
            TotalCount = movieDtos.Count,
            Items = movieDtos
        };
        
        return result;
    }

}
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using MovieMagnet.Data;
using MovieMagnet.Entities;
using MovieMagnet.Services.Dtos;
using MovieMagnet.Services.Dtos.Movies;
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

    public async Task<PagedResultDto<MovieDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
  
        var pageSize = input.MaxResultCount * 2;
        var offset = input.SkipCount * pageSize + 1;
        var sqlQuery = $"SELECT m.*, g.Name AS GenreName " +
                       $"FROM Movies m " +
                       $"INNER JOIN MovieGenres mg ON m.Id = mg.MovieId " +
                       $"INNER JOIN Genres g ON mg.GenreId = g.Id " +
                       $"ORDER BY m.Id " +
                       $"LIMIT {pageSize} OFFSET {offset}";
        Console.WriteLine(sqlQuery);
        var movies = await _dbContext.Movies
            .FromSqlRaw(sqlQuery)
            .Select(movie => new
            {
                movie.Id,
                movie.Title,
                movie.Budget,
                movie.Language,
                movie.Overview,
                movie.PosterPath,
                movie.ReleaseDate,
                movie.ImdbId,
                movie.Popularity,
                movie.Revenue,
                movie.Runtime,
                movie.VoteAverage,
                movie.VoteCount,
                MovieGenres = movie.MovieGenres.AsEnumerable().Select(mg => mg.Genre.Name).ToList()
            })
            .ToListAsync();

        var uniqueMovies = movies
            .GroupBy(movie => movie.Id)
            .Select(group => group.First())  // Select the first movie from each group (assuming Id is unique)
            .ToList();

        var movieDtos = uniqueMovies.Select(entry => new MovieDto
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
            Genres = entry.MovieGenres.ToArray()
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

    public async Task<PagedResultDto<MovieDto>> GetMoviesByGenresAsync(long genresId, PagedAndSortedResultRequestDto input)
    {
        List<MovieDto> result = new() { };

        var queryable = (await _movieRepository.WithDetailsAsync()).Where(movie => movie.MovieGenres.Any(movieGenre => movieGenre.GenreId == genresId))
                                                                   .Skip(input.SkipCount)
                                                                   .Take(input.MaxResultCount);

        var queryResult = await AsyncExecuter.ToListAsync(queryable);

        queryResult.ForEach(movie =>
        {
            result.add(new MovieDto()
            {
                Id = movie.Id,
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
            });
        });

        return new PagedResultDto<MovieDto>(
            queryable.Count(),
           result
        );
    }

    public async Task<PagedResultDto<MovieDto>> GetTopRated(PagedAndSortedResultRequestDto input)
    {
        List<MovieDto> result = new() { };

        decimal meanVote = await CalculateMeanVote();
        const decimal minVotesRequired = 7;

        var queryable = (await _movieRepository.WithDetailsAsync())
            .Where(movie => movie.VoteAverage >= minVotesRequired)
            .OrderByDescending(movie =>
                ((movie.VoteCount / (movie.VoteCount + meanVote)) * movie.VoteAverage)
                + ((meanVote / (movie.VoteCount + meanVote)) * meanVote))
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var queryResult = await AsyncExecuter.ToListAsync(queryable);

        queryResult.ForEach(movie =>
        {
            result.add(new MovieDto()
            {
                Id = movie.Id,
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
            });
        });

        return new PagedResultDto<MovieDto>(
            queryable.Count(),
           result
        );
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
        List<MovieDto> result = new() { };

        var queryable = await _movieRepository.WithDetailsAsync();

        Random rnd = new Random();

        queryable = queryable.Where(x => true).OrderBy(x => EF.Functions.Random()).Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var queryResult = await AsyncExecuter.ToListAsync(queryable);

        queryResult.ForEach(movie =>
        {
            result.add(new MovieDto()
            {
                Id = movie.Id,
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
            });
        });

        return new PagedResultDto<MovieDto>(
            queryable.Count(),
           result
        );
    }
}
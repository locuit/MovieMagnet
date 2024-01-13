using System.Linq.Dynamic.Core;
using System.Net;
using Microsoft.EntityFrameworkCore;
using MovieMagnet.Data;
using MovieMagnet.Entities;
using MovieMagnet.Services.Dtos.Movies;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;



namespace MovieMagnet.Services.Movies;

public class MovieService : MovieMagnetAppService, IMovieService
{
    private readonly IRepository<Movie, long> _movieRepository;
    private readonly MovieMagnetDbContext _dbContext;


    public MovieService(IRepository<Movie, long> movieRepository, MovieMagnetDbContext dbContext)
    {
        _movieRepository = movieRepository;
        _dbContext = dbContext;
    }

    public async Task<PagedResultDto<MovieDto>> GetListAsync(PagedAndSortedResultRequestDto input, string? search, decimal? minRating, decimal? maxRating, string[]? genres)
{
    var moviesQuery = _dbContext.Movies
        .Where(m =>
            (string.IsNullOrEmpty(search) || m.Title.Contains(search) || m.Overview.Contains(search)) &&
            (genres == null || genres.Length == 0 ||
             m.MovieGenres.Any(mg => genres.Contains(mg.GenreId.ToString()))));

    moviesQuery = ApplyRatingFilter(moviesQuery, minRating, maxRating);

    moviesQuery = moviesQuery.OrderBy(input.Sorting ?? nameof(Movie.Title));

    var movies = await moviesQuery
        .Skip(input.SkipCount)
        .Take(input.MaxResultCount)
        .AsSplitQuery()
        .Include(m => m.MovieGenres)
        .ThenInclude(mg => mg.Genre)
        .Include(m => m.Ratings)
        .Select(entry => new MovieDto
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
            VoteAverage = entry.Ratings.Average(mr => mr.Score),
            VoteCount = entry.VoteCount,
            Genres = entry.MovieGenres.Select(mg => mg.Genre.Name).ToArray()
        }).ToListAsync();

    var result = new PagedResultDto<MovieDto>
    {
        TotalCount = movies.Count(),
        Items = movies
    };

    return result;
}

    private IQueryable<Movie> ApplyRatingFilter(IQueryable<Movie> query, decimal? minRating, decimal? maxRating)
    {
        if (minRating.HasValue || maxRating.HasValue)
        {
            query = query.Where(m => m.Ratings.Any());
        
            if (minRating.HasValue)
            {
                query = query.Where(m => m.Ratings.Average(r => r.Score) >= minRating.Value);
            }

            if (maxRating.HasValue)
            {
                query = query.Where(m => m.Ratings.Average(r => r.Score) <= maxRating.Value);
            }
        }

        return query;
    }



    private static MovieDto MapToMovieDto(Movie entry)
    {
        return new MovieDto
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
            VoteAverage = entry.Ratings.Any() ? entry.Ratings.Average(mr => mr.Score) : 0,
            VoteCount = entry.VoteCount,
            Genres = entry.MovieGenres.Select(mg => mg.Genre.Name).ToArray()
        };
    }




    public async Task<MovieDto> GetAsync(long id)
    {
        var movie = await _dbContext.Movies
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
            .Include(m => m.Ratings)
            .Select(entry => new MovieDto
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
                VoteAverage = entry.Ratings.Average(mr => mr.Score),
                VoteCount = entry.VoteCount,
                Genres = entry.MovieGenres.Select(mg => mg.Genre.Name).ToArray()
            })
            .AsSplitQuery()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie == null)
        {
            throw new EntityNotFoundException("Movie not found");
        }

        return movie;
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
            // VoteAverage = input.VoteAverage,
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
            .Where(m => m.MovieGenres.Any(mg => mg.GenreId == genresId));

        var paginateMovies = await movies
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre).Select(entry => new MovieDto
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
                VoteAverage = entry.Ratings.Average(mr => mr.Score),
                VoteCount = entry.VoteCount,
                Genres = entry.MovieGenres.Select(mg => mg.Genre.Name).ToArray()
            }).ToListAsync();

        var result = new PagedResultDto<MovieDto>
        {
            TotalCount = await movies.CountAsync(),
            Items = paginateMovies
        };

        return result;
    }

    public async Task<PagedResultDto<MovieDto>> GetTopRated(PagedAndSortedResultRequestDto input)
    {

        var meanVote = await CalculateMeanVote();
        const decimal minVotesRequired = 4;

        // var movies = _dbContext.Movies
        //     .Where(movie => movie.VoteAverage >= minVotesRequired)
        //     .OrderByDescending(movie =>
        //         ((movie.VoteCount / (movie.VoteCount + meanVote)) * movie.VoteAverage)
        //         + ((meanVote / (movie.VoteCount + meanVote)) * meanVote));
        
        var movies = _dbContext.Movies
            .Where(movie => movie.Ratings != null && movie.Ratings.Average(r => r.Score) >= minVotesRequired)
            .OrderByDescending(movie =>
                movie.VoteCount / (movie.VoteCount + meanVote) * movie.Ratings!.Average(r => r.Score)
                + ((meanVote / (movie.VoteCount + meanVote)) * meanVote));

        var paginateMovies = await movies
        .Skip(input.SkipCount)
        .Take(input.MaxResultCount)
        .Include(m => m.MovieGenres)
        .ThenInclude(mg => mg.Genre)
        .Include(m => m.Ratings)
        .Select(entry => new MovieDto()
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
            VoteAverage = entry.Ratings.Average(mr => mr.Score),
            VoteCount = entry.VoteCount,
            Genres = entry.MovieGenres.Select(mg => mg.Genre.Name).ToArray()
        }).ToListAsync();


        var result = new PagedResultDto<MovieDto>
        {
            TotalCount = await movies.CountAsync(),
            Items = paginateMovies
        };

        return result;

    }

    private async Task<decimal> CalculateMeanVote()
    {
        var totalVotes = await _movieRepository.SumAsync(x => x.Ratings!.Average(r => r.Score));
        var totalMovies = await _movieRepository.CountAsync();

        return totalVotes / totalMovies;
    }

    public async Task<PagedResultDto<MovieDto>> GetRandom(PagedAndSortedResultRequestDto input)
    {
        var movies = _dbContext.Movies
            .OrderBy(x => EF.Functions.Random())
            .ThenBy(input.Sorting ?? nameof(Movie.Title));

        var paginateMovies = await movies
        .Skip(input.SkipCount)
        .Take(input.MaxResultCount)
        .Include(m => m.MovieGenres)
        .ThenInclude(mg => mg.Genre)
        .Select(entry => new MovieDto()
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
        }).ToListAsync();

        var result = new PagedResultDto<MovieDto>
        {
            TotalCount = await movies.CountAsync(),
            Items = paginateMovies
        };

        return result;
    }

    public async Task<bool> PostValidateVidSrcUrl(VidsrcRequestDto input)
    {
        var httpClient = new HttpClient();

        var res = await httpClient.GetAsync(input.VidSrcUrl);

        if (res.StatusCode == HttpStatusCode.OK)
        {
            return true;
        }

        return false;
    }
}
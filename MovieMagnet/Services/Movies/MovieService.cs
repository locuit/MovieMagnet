using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.EntityFrameworkCore;
using MovieMagnet.Authorization;
using MovieMagnet.Data;
using MovieMagnet.Entities;
using MovieMagnet.Services.Dtos;
using MovieMagnet.Services.Dtos.Movies;
using MovieMagnet.Services.Utils;
using Newtonsoft.Json;
using NUglify.Helpers;
using Tensorflow;
using Volo.Abp;
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
    
    public async Task<PagedResultDto<MovieDto>> GetListAsync(PagedAndSortedResultRequestDto input, string? searchByTitle, string? searchByOverview, decimal? minRating, decimal? maxRating, string[]? genres)
    {
        var moviesQuery = _dbContext.Movies.Where(m => (string.IsNullOrEmpty(searchByTitle) ? true : m.Title.ToLower().Contains(searchByTitle.ToLower()))
            || string.IsNullOrEmpty(searchByOverview) ? true : m.Overview.ToLower().Contains(searchByOverview.ToLower()));

        if (genres != null && genres.Length != 0)
        {
            moviesQuery = moviesQuery.Where(m => m.MovieGenres.Any(mg => genres.Contains(mg.GenreId.ToString())));
        };

        moviesQuery = ApplyRatingFilter(moviesQuery, minRating, maxRating);

        moviesQuery = moviesQuery.OrderByDescending(x => x.Ratings.Average(m => m.Score) * 0.8m + x.Ratings.Count() * 0.2m);

        var movies = await moviesQuery
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .AsSplitQuery()
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
            .Include(m => m.Ratings)
            .ToListAsync();

        var utils = new UtilsService();

        movies = await utils.SavePosterPathToDb(movies);

        await _dbContext.SaveChangesAsync();

        var moviesDto = utils.MapToMovieDto(movies);

        var result = new PagedResultDto<MovieDto>
        {
            TotalCount = moviesQuery.Count(),
            Items = moviesDto
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
            VoteCount = entry.Ratings.Any() ? entry.Ratings.Count() : 0,
            Genres = entry.MovieGenres.Select(mg => mg.Genre.Name).ToArray()
        };
    }

    public async Task<MovieDto> GetAsync(long id)
    {
        var movie = await _dbContext.Movies
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
            .Include(m => m.Ratings)
            .AsSplitQuery()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie == null)
        {
            throw new EntityNotFoundException("Movie not found");
        }
        var utils = new UtilsService();

        movie = await utils.SavePosterPathToDb(movie);

        await _dbContext.SaveChangesAsync();

        return utils.MapToMovieDto(movie);
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
        var preMovies = _dbContext.Movies.Where(m => m.MovieGenres.Any(mg => genresId.ToString() == mg.GenreId.ToString()));

        var movies = await preMovies
            .OrderByDescending(x => x.Ratings.Average(m => m.Score) * 0.8m + x.Ratings.Count() * 0.2m)
            .AsSplitQuery()
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .Include(m => m.Ratings)
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre).ToListAsync();

        var utils = new UtilsService();

        movies = await utils.SavePosterPathToDb(movies);

        await _dbContext.SaveChangesAsync();

        var moviesDto = utils.MapToMovieDto(movies);

        var result = new PagedResultDto<MovieDto>
        {
            TotalCount = preMovies.Count(),
            Items = moviesDto
        };

        return result;
    }

    public async Task<PagedResultDto<MovieDto>> GetTopRated(PagedAndSortedResultRequestDto input)
    {

        var meanVote = await CalculateMeanVote();
        const decimal minVotesRequired = 4;

        var preMovies = _dbContext.Movies
            .Where(movie => movie.Ratings != null && movie.Ratings.Average(r => r.Score) >= minVotesRequired)
            .OrderByDescending(movie =>
                movie.VoteCount / (movie.VoteCount + meanVote) * movie.Ratings!.Average(r => r.Score)
                + ((meanVote / (movie.VoteCount + meanVote)) * meanVote));

        var movies = await preMovies
        .Skip(input.SkipCount)
        .AsSplitQuery()
        .Take(input.MaxResultCount)
        .Include(m => m.MovieGenres)
        .ThenInclude(mg => mg.Genre)
        .Include(m => m.Ratings)
        .ToListAsync();

        var utils = new UtilsService();

        movies = await utils.SavePosterPathToDb(movies);

        await _dbContext.SaveChangesAsync();

        var moviesDto = utils.MapToMovieDto(movies);

        var result = new PagedResultDto<MovieDto>
        {
            TotalCount = preMovies.Count(),
            Items = moviesDto
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
        var preMovies = _dbContext.Movies.OrderBy(x => EF.Functions.Random()).Include(m => m.Ratings);

        var movies = await preMovies
        .Skip(input.SkipCount)
        .Take(input.MaxResultCount)
        .AsSplitQuery()
        .Include(m => m.MovieGenres)
        .ThenInclude(mg => mg.Genre).ToListAsync();

        var utils = new UtilsService();

        movies = await utils.SavePosterPathToDb(movies);

        await _dbContext.SaveChangesAsync();

        var moviesDto = utils.MapToMovieDto(movies);

        var result = new PagedResultDto<MovieDto>
        {
            TotalCount = preMovies.Count(),
            Items = moviesDto
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
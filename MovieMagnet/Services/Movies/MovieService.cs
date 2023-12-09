using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AutoMapper.Internal.Mappers;
using MovieMagnet.Movies;
using MovieMagnet.Services.Dtos;
using MovieMagnet.Services.Dtos.Movies;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace MovieMagnet.Services.Movies;

public class MovieService : MovieMagnetAppService, IMovieService
{
    private readonly IRepository<Movie, long> _movieRepository;
    
    public MovieService(IRepository<Movie, long> movieRepository)
    {
        _movieRepository = movieRepository;
    }

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
    
    public async Task<MovieDto> GetAsync(long id)
    {
        var movie = await _movieRepository.GetAsync(id);
        return ObjectMapper.Map<Movie, MovieDto>(movie);
    }
    
    public async Task<MovieDto> CreateAsync(MovieDto input)
    {
        var movie = ObjectMapper.Map<MovieDto, Movie>(input);
        movie = await _movieRepository.InsertAsync(movie, autoSave: true);
        return ObjectMapper.Map<Movie, MovieDto>(movie);
    }
}
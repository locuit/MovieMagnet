using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using MovieMagnet.Entities;
using MovieMagnet.Services.Dtos;
using MovieMagnet.Services.Dtos.Genres;
using MovieMagnet.Services.Dtos.Movies;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using static Tensorflow.Binding;

namespace MovieMagnet.Services.Genres;

public class GenresService : MovieMagnetAppService, IGenresService
{
    private readonly IRepository<Genre, long> _genreRepository;

    public GenresService(IRepository<Genre, long> genreRepository)
    {
        _genreRepository = genreRepository;
    }

    public async Task<List<GenresDto>> GetAllAsync()
    {
        var movie = await _genreRepository.ToListAsync();

        List<GenresDto> res = new List<GenresDto>();

        movie.ForEach(eachMovie => res.Add(new GenresDto() { Id = eachMovie.Id, Name = eachMovie.Name }));
        return res;
    }
}
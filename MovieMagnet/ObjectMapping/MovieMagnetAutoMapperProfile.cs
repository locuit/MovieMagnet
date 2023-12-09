using AutoMapper;
using MovieMagnet.Movies;
using MovieMagnet.Services.Dtos;

namespace MovieMagnet.ObjectMapping;

public class MovieMagnetAutoMapperProfile : Profile
{
    public MovieMagnetAutoMapperProfile()
    {
        /* Create your AutoMapper object mappings here */
        CreateMap<Movie, MovieDto>();
    }
}

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MovieMagnet.Services.Dtos.Genres;

public interface IGenresService : IApplicationService
{
    Task<List<GenresDto>> GetAllAsync();
    Task<GenresDto> GetAsync(long genreId);
}
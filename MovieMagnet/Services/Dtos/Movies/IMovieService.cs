using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MovieMagnet.Services.Dtos.Movies;

public interface IMovieService : IApplicationService
{
    Task<PagedResultDto<MovieDto>> GetListAsync(PagedAndSortedResultRequestDto input);
    Task<MovieDto> GetAsync(long id);
    Task<MovieDto> CreateAsync(MovieDto input);
    Task<PagedResultDto<MovieDto>> GetMoviesByGenresAsync(long genresId, PagedAndSortedResultRequestDto input);
    Task<PagedResultDto<MovieDto>> GetTopRated(PagedAndSortedResultRequestDto input);
    Task<PagedResultDto<MovieDto>> GetRandom(PagedAndSortedResultRequestDto input);
}
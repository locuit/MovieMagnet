using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MovieMagnet.Services.Dtos.Movies;

public interface IMovieService : IApplicationService
{
    Task<PagedResultDto<MovieDto>> GetListAsync(PagedAndSortedResultRequestDto input);
    Task<MovieDto> GetAsync(long id);
    Task<MovieDto> CreateAsync(MovieDto input);
    
}
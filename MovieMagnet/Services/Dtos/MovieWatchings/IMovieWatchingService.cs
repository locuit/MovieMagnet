using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MovieMagnet.Services.Dtos.MovieWatchings;

public interface IMovieWatchingService : IApplicationService
{
    Task<string> AddToWatchingList(long id, string lastViewMoment);
    Task<string> RemoveFromWatchingList(long id);
    Task<PagedResultDto<MovieDto>> GetAsync(PagedAndSortedResultRequestDto input);
}
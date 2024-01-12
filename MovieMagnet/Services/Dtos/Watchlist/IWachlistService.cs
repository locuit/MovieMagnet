using MovieMagnet.Services.Dtos.Movies;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MovieMagnet.Services.Dtos.Watchlist;

public interface IWatchlistService : IApplicationService
{
    Task<string> AddAsync(long MovieId);
    Task<string> RemoveAsync(long MovieId);
    Task<PagedResultDto<MovieDto>> GetAsync(PagedAndSortedResultRequestDto input);
}
using Volo.Abp.Application.Services;

namespace MovieMagnet.Services.Dtos.MovieWatchings;

public interface IMovieWatchingService : IApplicationService
{
    Task<MovieWatchingDto> AddToWatchingList(Guid id, CreateMovieWatchingDto createMovieWatchingDto);
}
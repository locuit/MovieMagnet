using MovieMagnet.Services.Dtos.Movies;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MovieMagnet.Services.Dtos.Recommendations;

public interface IRecommendationService : IApplicationService
{
    Task<PagedResultDto<MovieDto>> RecommendMovies(int n);
}
using Volo.Abp.Application.Services;

namespace MovieMagnet.Services.Dtos.Recommendations;

public interface IRecommendationService : IApplicationService
{
    void CreateModel();
}
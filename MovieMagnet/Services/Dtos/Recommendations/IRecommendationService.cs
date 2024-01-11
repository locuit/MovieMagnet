using Volo.Abp.Application.Services;

namespace MovieMagnet.Services.Dtos.Recommendations;

public interface IRecommendationService : IApplicationService
{
    void PrepareData();
    // void TrainModel(int numUsers, int numMovies, int embeddingDimension, int epochs);
    // List<int> RecommendMovies(int userId, int topN);
    //
    // void Run();
}
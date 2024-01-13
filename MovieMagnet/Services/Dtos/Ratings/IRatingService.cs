using Volo.Abp.Application.Services;

namespace MovieMagnet.Services.Dtos.Ratings;

public interface IRatingService : IApplicationService
{
    Task<string> CreateAsync(CreateRatingDto input);
    
    Task<string> UpdateAsync(UpdateRatingDto input);

    Task<RatingDto> GetAsync(long movieId);
}
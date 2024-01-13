using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using MovieMagnet.Authorization;
using MovieMagnet.Data;
using MovieMagnet.Entities;
using MovieMagnet.Services.Dtos;
using MovieMagnet.Services.Dtos.Ratings;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace MovieMagnet.Services.Ratings;

[Authorize]
public class RatingService : MovieMagnetAppService, IRatingService
{
    private readonly IRepository<Rating, long> _ratingRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RatingService(IRepository<Rating, long> ratingRepository, IHttpContextAccessor httpContextAccessor)
    {
        _ratingRepository = ratingRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet("ratings")]
    [Authorize]
    public async Task<RatingDto> GetAsync(long movieId)
    {
        var user = _httpContextAccessor.HttpContext?.Items["User"] as UserDto;
        var rating = await _ratingRepository.FirstOrDefaultAsync(x => x.MovieId == movieId && x.UserId == user.Id);

        if (rating == null)
        {
            throw new EntityNotFoundException("You not rate this movie yet.");
        }

        return new RatingDto()
        {
            Score = rating.Score,
            MovieId = rating.MovieId,
            UserId = rating.UserId,
            Timestamp = rating.Timestamp
        };
    }

    [HttpPost("ratings")]
    [Authorize]
    public async Task<string> CreateAsync(CreateRatingDto input)
    {
        var user = _httpContextAccessor.HttpContext?.Items["User"] as UserDto;
        var rating = await _ratingRepository.FirstOrDefaultAsync(x => x.MovieId == input.MovieId && x.UserId == user.Id);

        if (rating != null)
        {
            throw new EntityNotFoundException("You already rate this movie.");
        }

        await _ratingRepository.InsertAsync(new Rating()
        {
            MovieId = input.MovieId,
            UserId = user.Id,
            Score = input.Score,
            Timestamp = input.Timestamp
        });
        return "Rating created successfully";
    }

    [HttpPut("ratings")]
    [Authorize]
    public async Task<string> UpdateAsync(UpdateRatingDto input)
    {
        var user = _httpContextAccessor.HttpContext?.Items["User"] as UserDto;
        var rating = await _ratingRepository.FirstOrDefaultAsync(x => x.MovieId == input.MovieId && x.UserId == user.Id);

        if (rating == null)
        {
            throw new EntityNotFoundException("You not rate this movie yet.");
        }

        rating.Score = input.Score;
        rating.Timestamp = input.Timestamp;
        await _ratingRepository.UpdateAsync(rating);
        return "Rating updated successfully";
    }
}
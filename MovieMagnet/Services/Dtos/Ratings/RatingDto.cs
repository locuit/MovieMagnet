using Volo.Abp.Application.Dtos;

namespace MovieMagnet.Services.Dtos;

public class RatingDto : EntityDto<long>
{
    public int Rating { get; set; }
    public long MovieId { get; set; }
    public long? UserId { get; set; }
    public decimal Score { get; set; }
    public string Timestamp { get; set; }
}
    
public class CreateRatingDto
{
    public long MovieId { get; set; }
    public decimal Score { get; set; }
    public string Timestamp { get; set; }
}
    
public class UpdateRatingDto
{
    public long MovieId { get; set; }
    public decimal Score { get; set; }
    public string Timestamp { get; set; }
}
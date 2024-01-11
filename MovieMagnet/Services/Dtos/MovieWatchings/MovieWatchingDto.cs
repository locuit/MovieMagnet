namespace MovieMagnet.Services.Dtos.MovieWatchings;

public class MovieWatchingDto
{
    public long Movie { get; set; }
    public long User { get; set; }
    public string lastViewMoment { get; set; }
}

public class CreateMovieWatchingDto
{
    public long movieId;
    public long? userId;
    public string lastViewMoment;
}
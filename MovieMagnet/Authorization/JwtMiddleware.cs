using MovieMagnet.Services.Dtos;
using MovieMagnet.Services.Dtos.Users;

namespace MovieMagnet.Authorization;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var userId = jwtUtils.ValidateJwtToken(token);
        if (userId != null)
        {
            context.Items["User"] = await userService.GetById(userId.Value);
        }

        await _next(context);
    }
}
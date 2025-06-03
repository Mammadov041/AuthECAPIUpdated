using AuthECAPI.Dtos;
using AuthECAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthECAPI.Controllers
{
    public static class IdentityUserEndpoints
    {
        public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app,IConfiguration configuration)
        {
            app.MapPost("/signup",CreateUser);
            app.MapPost("/signin", Login);
            return app;
        }

        private static async Task<IResult> CreateUser(UserManager<AppUser> userManager, [FromBody] RegisterDto registerDto) 
        {
            AppUser user = new AppUser()
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FullName = registerDto.FullName
            };
            var result = await userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
                return Results.Ok(result);
            return Results.BadRequest(result);
        }

        private static async Task<IResult> Login(UserManager<AppUser> userManager, [FromBody] LoginDto loginDto,IOptions<AppSettings> appSettings)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Value.JWTSecret));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                new Claim("UserID",user.Id)
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Results.Ok(new { Token = token, ExpiresIn = tokenDescriptor.Expires });
            }
            return Results.BadRequest("Provided Email or password is incorrect");
        }
    }
}

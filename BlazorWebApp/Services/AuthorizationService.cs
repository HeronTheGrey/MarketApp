using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MauiMarket.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BlazorWebApp.Services;

public class AuthorizationService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;
    
    public AuthorizationService(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }
    
    public async Task<RegisterResponse> Register(string email, string password)
    {
        var user = new User
        {
            UserName = email,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);
    
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            Console.WriteLine("Błąd rejestracji: " + errors);
            return new RegisterResponse{Errors = errors, Success = false};
        }

        if (!await _userManager.IsInRoleAsync(user, "User"))
        {
            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                Console.WriteLine("Błąd dodania roli: " + roleErrors);
                return new RegisterResponse{Errors = roleErrors, Success = false};
            }
        }

        return new RegisterResponse{Success = true};
    }
    
    public async Task<string?> Login(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded) return null;

        var token = GenerateJwtToken(user);
        return token;
    }
    
    public async void Logout()
    {
        await _signInManager.SignOutAsync();
    }
    
    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var userRoles = _userManager.GetRolesAsync(user).Result;
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
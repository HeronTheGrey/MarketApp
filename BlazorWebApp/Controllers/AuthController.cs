using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MauiMarket.Shared.Models;
using BlazorWebApp.Data;
using BlazorWebApp.Services;

namespace BlazorWebApp.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly AuthorizationService _authorizationService;

    public AuthController(UserManager<User> userManager, AuthorizationService authorizationService)
    {
        _userManager = userManager;
        _authorizationService = authorizationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var result = await _authorizationService.Register(model.Email, model.Password);
        if(result.Success) return Ok(new { message = "Rejestracja zakończona sukcesem" });
        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var token = await _authorizationService.Login(model.Username, model.Password);
        if (token == null) return Unauthorized("Invalid credentials");
        return Ok(new { token });
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        _authorizationService.Logout();
        return Ok(new { message = "Logged out successfully" });
    }
}

public class RegisterDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}
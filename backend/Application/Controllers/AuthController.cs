using Microsoft.AspNetCore.Mvc;
using TaskManager.Models;
using Microsoft.AspNetCore.Identity;
using TaskManager.Dto;
using TaskManager.Repositories;
using TaskManager.Services;

namespace TaskManager.API;

[Route("auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthController(IUserRepository users, ITokenService tokenService)
    {
        _users = users;
        _tokenService = tokenService;
    }

    public record RegisterRequest(string Email, string Password);
    public record LoginRequest(string Email, string Password);
    public record AuthResponse(string Token);

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var exists = await _users.AnyByEmailAsync(request.Email);

        if (exists)
        {
            return StatusCode(409, ApiResponse<AuthResponse>.Fail(
                message: "Email already in use",
                code: 409
            ));
        }

        var user = new User
        {
            Email = request.Email
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _users.AddAsync(user);
        var token = _tokenService.GenerateToken(user);
        return Ok(ApiResponse<AuthResponse>.Ok(message: "Registered", data: new AuthResponse(token)));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email);
        if (user == null)
        {
            return StatusCode(401, ApiResponse<AuthResponse>.Fail(
                message: "Invalid credentials",
                code: 401
            ));
        }

        var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verify == PasswordVerificationResult.Failed)
        {
            return StatusCode(401, ApiResponse<AuthResponse>.Fail(
                message: "Invalid credentials",
                code: 401
            ));
        }

        var token = _tokenService.GenerateToken(user);
        return Ok(ApiResponse<AuthResponse>.Ok(message: "Logged in", data: new AuthResponse(token)));
    }
}
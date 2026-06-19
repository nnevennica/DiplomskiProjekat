using DiplomskiProjekat.Models;
using DiplomskiProjekat.Models.Auth;
using DiplomskiProjekat.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace MeteoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserStore _users;
    private readonly PasswordService _passwords;
    private readonly IJwtTokenService _jwt;

    public AuthController(IUserStore users, PasswordService passwords, IJwtTokenService jwt)
    {
        _users = users;
        _passwords = passwords;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest dto)
    {
        var email = (dto.Email ?? "").Trim().ToLowerInvariant();
        var password = (dto.Password ?? "").Trim();
        var city = (dto.City ?? "").Trim();

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email je obavezan.");

        if (!email.Contains("@"))
            return BadRequest("Unesi validan email.");

        if (string.IsNullOrWhiteSpace(password))
            return BadRequest("Šifra je obavezna.");

        if (string.IsNullOrWhiteSpace(city))
            return BadRequest("Grad je obavezan.");

        var existing = await _users.FindByEmailAsync(email);
        if (existing != null)
            return BadRequest("Email je već registrovan.");

        var user = new User
        {
            FirstName = (dto.FirstName ?? "").Trim(),
            LastName = (dto.LastName ?? "").Trim(),
            Email = email,
            City = city,
            PasswordHash = _passwords.Hash(password)
        };

        await _users.CreateAsync(user);

        var token = _jwt.CreateToken(user);

        return Ok(new AuthResponse
        {
            User = new { user.Id, user.FirstName, user.LastName, user.Email, user.City },
            AccessToken = token
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest dto)
    {
        var email = (dto.Email ?? "").Trim().ToLowerInvariant();
        var password = (dto.Password ?? "").Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return Unauthorized("Pogrešan email ili šifra.");

        var user = await _users.FindByEmailAsync(email);
        if (user == null) return Unauthorized("Pogrešan email ili šifra.");

        var ok = _passwords.Verify(user.PasswordHash, password);
        if (!ok) return Unauthorized("Pogrešan email ili šifra.");

        var token = _jwt.CreateToken(user);

        return Ok(new AuthResponse
        {
            User = new { user.Id, user.FirstName, user.LastName, user.Email, user.City },
            AccessToken = token
        });
    }
}

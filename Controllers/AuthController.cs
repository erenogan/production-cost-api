using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UretimMaliyet.Data;
using UretimMaliyet.Models;
using UretimMaliyet.Services;

namespace UretimMaliyet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _token;

    public AuthController(AppDbContext db, TokenService token)
    {
        _db = db;
        _token = token;
    }

    [HttpPost("kayit")]
    public async Task<IActionResult> Kayit(KayitDto dto)
    {
        if (await _db.Kullanicilar.AnyAsync(k => k.Email == dto.Email))
            return BadRequest(new { hata = "Bu email zaten kayıtlı" });

        var kullanici = new Kullanici
        {
            Email = dto.Email,
            ParolaHash = BCrypt.Net.BCrypt.HashPassword(dto.Parola),
            Rol = dto.Rol
        };

        _db.Kullanicilar.Add(kullanici);
        await _db.SaveChangesAsync();

        return Ok(new { kullanici.Id, kullanici.Email, kullanici.Rol });
    }

    [HttpPost("giris")]
    public async Task<IActionResult> Giris(GirisDto dto)
    {
        var kullanici = await _db.Kullanicilar
            .FirstOrDefaultAsync(k => k.Email == dto.Email);

        if (kullanici is null ||
            !BCrypt.Net.BCrypt.Verify(dto.Parola, kullanici.ParolaHash))
            return Unauthorized(new { hata = "Email veya parola hatalı" });

        var token = _token.Uret(kullanici);
        return Ok(new { token });
    }
}
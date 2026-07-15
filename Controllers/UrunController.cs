using Microsoft.AspNetCore.Mvc;
using UretimMaliyet.Models;
using UretimMaliyet.Services;
using Microsoft.AspNetCore.Authorization;

namespace UretimMaliyet.Controllers;

[Authorize]  
[ApiController]
[Route("api/[controller]")]
public class UretimController : ControllerBase
{
    private readonly MaliyetService _maliyet;

    public UretimController(MaliyetService maliyet) => _maliyet = maliyet;

    [HttpPost]
    public async Task<IActionResult> Uret(UretimEmriDto dto)
    {
        var (basarili, mesaj, emir) = await _maliyet.UretimYapAsync(dto.UrunId, dto.Miktar);

        if (!basarili)
            return BadRequest(new { hata = mesaj });

        return Ok(new
        {
            mesaj,
            emir!.Id,
            emir.UrunId,
            emir.Miktar,
            emir.ToplamMaliyet,
            emir.Tarih
        });
    }
}
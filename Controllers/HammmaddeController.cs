
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UretimMaliyet.Data;
using UretimMaliyet.Models;
using Microsoft.AspNetCore.Authorization;

namespace UretimMaliyet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HammaddeController : ControllerBase
{
    private readonly AppDbContext _db;

    public HammaddeController(AppDbContext db)   // ← Dependency Injection
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var liste = await _db.Hammaddeler.ToListAsync();
        return Ok(liste);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var hammadde = await _db.Hammaddeler.FindAsync(id);
        if (hammadde is null) return NotFound();
        return Ok(hammadde);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Hammadde hammadde)
    {
        _db.Hammaddeler.Add(hammadde);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = hammadde.Id }, hammadde);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Hammadde girdi)
    {
        var hammadde = await _db.Hammaddeler.FindAsync(id);
        if (hammadde is null) return NotFound();

        hammadde.Ad = girdi.Ad;
        hammadde.Birim = girdi.Birim;
        hammadde.BirimFiyat = girdi.BirimFiyat;
        hammadde.StokMiktari = girdi.StokMiktari;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]  
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var hammadde = await _db.Hammaddeler.FindAsync(id);
        if (hammadde is null) return NotFound();

        _db.Hammaddeler.Remove(hammadde);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
using Microsoft.AspNetCore.Mvc;
using UretimMaliyet.Services;

namespace UretimMaliyet.Controllers;

public class ChatIstekDto
{
    public string Soru { get; set; } = "";
}

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatService _chat;

    public ChatController(ChatService chat) => _chat = chat;

    [HttpPost]
    public async Task<IActionResult> Sor(ChatIstekDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Soru))
            return BadRequest(new { hata = "Soru boş olamaz" });

        var cevap = await _chat.SoruSorAsync(dto.Soru);
        return Ok(new { soru = dto.Soru, cevap });
    }
}
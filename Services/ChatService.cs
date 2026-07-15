using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using UretimMaliyet.Data;

namespace UretimMaliyet.Services;

public class ChatService
{
    private readonly AppDbContext _db;
    private readonly MaliyetService _maliyet;
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _config;

    public ChatService(AppDbContext db, MaliyetService maliyet,
                       IHttpClientFactory httpFactory, IConfiguration config)
    {
        _db = db;
        _maliyet = maliyet;
        _httpFactory = httpFactory;
        _config = config;
    }

    public async Task<string> SoruSorAsync(string soru)
    {
        // 1) Firmanın gerçek verisini topla (hesabı BİZİM kodumuz yapar)
        var veri = await VeriOzetiHazirlaAsync();

        // 2) LLM'e "işte gerçek veri, işte soru" diyerek gönder
       var sistemMesaji =
            "Sen bir baklava/tatlı üretim firmasının yapay zeka asistanısın. " +
            "Sana firmanın güncel üretim ve maliyet verileri, her ürünün reçete kırılımıyla veriliyor. " +
            "Bir hammadde fiyatı değişirse, o hammaddenin ürün içindeki payına (yukarıdaki reçete kırılımı) " +
            "göre yeni maliyeti hesapla; tüm maliyetin değiştiğini varsayma. " +
            "SADECE verilen verilere dayan, Türkçe kısa ve net cevap ver. " +
            "Sayıları uydurma; hesabı reçete kırılımına dayandır.\n\n" +
            "=== FİRMA VERİLERİ ===\n" + veri;

        var istekGovdesi = new
        {
            model = _config["Groq:Model"],
            messages = new[]
            {
                new { role = "system", content = sistemMesaji },
                new { role = "user", content = soru }
            },
            temperature = 0.3
        };

        var client = _httpFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization",
            $"Bearer {_config["Groq:ApiKey"]}");

        var json = JsonSerializer.Serialize(istekGovdesi);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(
            "https://api.groq.com/openai/v1/chat/completions", content);

        if (!response.IsSuccessStatusCode)
        {
            var hata = await response.Content.ReadAsStringAsync();
            return $"AI servisine ulaşılamadı: {response.StatusCode} - {hata}";
        }

        var cevapJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(cevapJson);
        var mesaj = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return mesaj ?? "Cevap alınamadı.";
    }

    // Firmanın tüm verisini LLM'in anlayacağı metne çevir
    private async Task<string> VeriOzetiHazirlaAsync()
    {
        var sb = new StringBuilder();

        // Ürünler, maliyetleri VE reçete dağılımı
        var urunler = await _db.Urunler.ToListAsync();
        sb.AppendLine("ÜRÜNLER, MALİYETLER VE REÇETELER:");
        foreach (var u in urunler)
        {
            var m = await _maliyet.HesaplaAsync(u.Id);
            if (m is null) continue;
            sb.AppendLine(
                $"- {m.UrunAd}: birim maliyet {m.BirimMaliyet} TL, " +
                $"satış {m.SatisFiyati} TL, kâr {m.BirimKar} TL, kâr marjı %{m.KarMarjiYuzde}");

            // Reçete kırılımı — hangi hammadde maliyetin ne kadarı
            foreach (var k in m.Kalemler)
            {
                sb.AppendLine(
                    $"    * {k.Hammadde}: {k.Miktar} birim x {k.BirimFiyat} TL = {k.Tutar} TL");
            }
        }

        // Hammadde stokları
        sb.AppendLine("\nHAMMADDE STOKLARI:");
        var hammaddeler = await _db.Hammaddeler.ToListAsync();
        foreach (var h in hammaddeler)
        {
            sb.AppendLine(
                $"- {h.Ad}: stok {h.StokMiktari}, birim fiyat {h.BirimFiyat} TL");
        }

        return sb.ToString();
    }
}
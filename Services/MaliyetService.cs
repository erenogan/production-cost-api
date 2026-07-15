using Microsoft.EntityFrameworkCore;
using UretimMaliyet.Data;
using UretimMaliyet.Models;

namespace UretimMaliyet.Services;

public class MaliyetService
{
    private readonly AppDbContext _db;

    public MaliyetService(AppDbContext db) => _db = db;

    public async Task<MaliyetSonucuDto?> HesaplaAsync(int urunId)
    {
        var urun = await _db.Urunler
            .Include(u => u.ReceteKalemleri)
                .ThenInclude(k => k.Hammadde)
            .FirstOrDefaultAsync(u => u.Id == urunId);

        if (urun is null) return null;

        var kalemler = urun.ReceteKalemleri
            .Select(k => new MaliyetKalemiDto
            {
                Hammadde   = k.Hammadde!.Ad,
                Miktar     = k.Miktar,
                BirimFiyat = k.Hammadde.BirimFiyat,
                Tutar      = k.Miktar * k.Hammadde.BirimFiyat
            })
            .ToList();

        var birimMaliyet = kalemler.Sum(k => k.Tutar);
        var birimKar     = urun.SatisFiyati - birimMaliyet;

        var karMarji = urun.SatisFiyati == 0
            ? 0
            : Math.Round(birimKar / urun.SatisFiyati * 100, 2);

        return new MaliyetSonucuDto
        {
            UrunId        = urun.Id,
            UrunAd        = urun.Ad,
            BirimMaliyet  = Math.Round(birimMaliyet, 2),
            SatisFiyati   = urun.SatisFiyati,
            BirimKar      = Math.Round(birimKar, 2),
            KarMarjiYuzde = karMarji,
            Kalemler      = kalemler
        };
    }

    public async Task<(bool basarili, string mesaj, UretimEmri? emir)> UretimYapAsync(int urunId, decimal miktar)
    {
        if (miktar <= 0)
            return (false, "Üretim miktarı pozitif olmalı", null);

        var urun = await _db.Urunler
            .Include(u => u.ReceteKalemleri)
                .ThenInclude(k => k.Hammadde)
            .FirstOrDefaultAsync(u => u.Id == urunId);

        if (urun is null)
            return (false, "Ürün bulunamadı", null);

        if (urun.ReceteKalemleri.Count == 0)
            return (false, "Ürünün reçetesi yok, üretilemez", null);

        // 1) Stok yeterli mi? Önce KONTROL et, sonra düş.
        foreach (var kalem in urun.ReceteKalemleri)
        {
            var gerekli = kalem.Miktar * miktar;
            if (kalem.Hammadde!.StokMiktari < gerekli)
                return (false,
                    $"Yetersiz stok: {kalem.Hammadde.Ad} " +
                    $"(gerekli: {gerekli}, mevcut: {kalem.Hammadde.StokMiktari})",
                    null);
        }

        // 2) Tüm stoklar yeterli. Şimdi düş.
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            foreach (var kalem in urun.ReceteKalemleri)
            {
                var gerekli = kalem.Miktar * miktar;
                kalem.Hammadde!.StokMiktari -= gerekli;
            }

            var birimMaliyet = urun.ReceteKalemleri
                .Sum(k => k.Miktar * k.Hammadde!.BirimFiyat);

            var emir = new UretimEmri
            {
                UrunId = urunId,
                Miktar = miktar,
                Tarih = DateTime.UtcNow,
                ToplamMaliyet = birimMaliyet * miktar
            };

            _db.UretimEmirleri.Add(emir);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, "Üretim başarılı", emir);
        }
        catch
        {
            await transaction.RollbackAsync();
            return (false, "Üretim sırasında hata oluştu", null);
        }
    }   
}   

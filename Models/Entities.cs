namespace UretimMaliyet.Models;

public enum Birim { Kg = 0, Litre = 1, Adet = 2 }

public class Hammadde
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public Birim Birim { get; set; }
    public decimal BirimFiyat { get; set; }
    public decimal StokMiktari { get; set; }
}

public class Urun
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public decimal SatisFiyati { get; set; }

    public List<ReceteKalemi> ReceteKalemleri { get; set; } = new();
}

public class ReceteKalemi
{
    public int Id { get; set; }

    public int UrunId { get; set; }
    public Urun? Urun { get; set; }

    public int HammaddeId { get; set; }
    public Hammadde? Hammadde { get; set; }

    public decimal Miktar { get; set; }   // 1 birim ürün için gereken hammadde miktarı
}

public class UretimEmri
{
    public int Id { get; set; }
    public int UrunId { get; set; }
    public Urun? Urun { get; set; }
    public decimal Miktar { get; set; }          // kaç birim üretilecek
    public DateTime Tarih { get; set; }
    public decimal ToplamMaliyet { get; set; }   // üretim anındaki maliyet (dondurulur)
}

public class Kullanici
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string ParolaHash { get; set; } = "";   // asla düz parola!
    public string Rol { get; set; } = "Personel";  // "Admin" veya "Personel"
}

// #Hammadde veri tabanındaki tablo bu sınıfa bakıp tabloyu kendisi yaratacak
// #id ismi özel ben söylemesemde ef core bunu primary key yapar
// #para double değil decimal para her zaman decimal olur çünkü double ondalık sayılarla yuvarlama hatası yapar
// #string empty ile başlamalı yoksa derleyici uyarıar
// #enum birim  birim serbestbir metin olsaydı biri k biri Kg biri de kilogram yazardı enum bunu engeller

namespace UretimMaliyet.Models;

public class ReceteKalemiDto
{
    public int HammaddeId { get; set; }
    public decimal Miktar { get; set; }
}

public class MaliyetSonucuDto
{
    public int UrunId { get; set; }
    public string UrunAd { get; set; } = string.Empty;
    public decimal BirimMaliyet { get; set; }
    public decimal SatisFiyati { get; set; }
    public decimal BirimKar { get; set; }
    public decimal KarMarjiYuzde { get; set; }
    public List<MaliyetKalemiDto> Kalemler { get; set; } = new();
}

public class MaliyetKalemiDto
{
    public string Hammadde { get; set; } = string.Empty;
    public decimal Miktar { get; set; }
    public decimal BirimFiyat { get; set; }
    public decimal Tutar { get; set; }
}

public class UretimEmriDto
{
    public int UrunId { get; set; }
    public decimal Miktar { get; set; }
}   
public class KayitDto
{
    public string Email { get; set; } = "";
    public string Parola { get; set; } = "";
    public string Rol { get; set; } = "Personel";
}

public class GirisDto
{
    public string Email { get; set; } = "";
    public string Parola { get; set; } = "";
}
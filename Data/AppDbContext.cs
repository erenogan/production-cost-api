using Microsoft.EntityFrameworkCore;
using UretimMaliyet.Models;

namespace UretimMaliyet.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Hammadde> Hammaddeler => Set<Hammadde>();
public DbSet<Urun> Urunler => Set<Urun>();
public DbSet<ReceteKalemi> ReceteKalemleri => Set<ReceteKalemi>();
public DbSet<UretimEmri> UretimEmirleri => Set<UretimEmri>();
public DbSet<Kullanici> Kullanicilar => Set<Kullanici>();
    
}




// #dbcontext veri tabanıyla konuşan tek şeydir kodun her yerinde veri tabanına ulaşamazsın bu yüzden 
// dbset ham madde satırı şunu söyler veritabanında hammaddeler diye bir tablo olacak içinde hammadde nesneleri duracka

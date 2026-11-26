using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Concrete;

public class KontrolServisi(AppDbContext db) : IKontrolServisi
{
    public async Task<NumaraKontrolDto> NumaraAsync(string tur, string deger, int? haricId = null, CancellationToken ct = default)
    {
        deger = (deger ?? "").Trim();
        if (Numara.Sade(deger).Length == 0)
            return new NumaraKontrolDto(false, "");
        var haric = haricId ?? 0;
        string? nerede = tur switch
        {
            "lot" => await db.StokLotlari.Where(Numara.Ayni<StokLotu>(l => l.LotNo, deger)).Where(l => l.Id != haric)
                .Select(l => l.Parca!.Ad + " lotu").FirstOrDefaultAsync(ct),
            "seri" => await db.Uretimler.Where(Numara.Ayni<Uretim>(u => u.SeriNo, deger)).Where(u => u.Id != haric)
                .Select(u => u.Urun!.Ad + " ürününde").FirstOrDefaultAsync(ct),
            "siparis" => await db.Siparisler.Where(Numara.Ayni<Siparis>(s => s.SiparisNo, deger)).Where(s => s.Id != haric)
                .Select(s => s.Musteri!.Ad + " talebinde").FirstOrDefaultAsync(ct),
            "sevkiyat" => await db.Satislar.Where(Numara.Ayni<Satis>(s => s.No, deger)).Where(s => s.Id != haric)
                .Select(s => s.Musteri!.Ad + " sevkiyatında").FirstOrDefaultAsync(ct),
            _ => throw new IsKuraliHatasi("Tür lot, seri, siparis ya da sevkiyat olmalı."),
        };
        return nerede is null ? new NumaraKontrolDto(false, "") : new NumaraKontrolDto(true, $"{deger} daha önce kullanıldı ({nerede}).");
    }
}

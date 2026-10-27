# Stok Lot Sistemi

Elektronik ürün üreten bir firma için stok, üretim ve satış takibi; lot ve seri numarası
bazında izlenebilirlik.

- **Parçadan bütüne:** bir lottaki parça ne zaman, nereden alındı; hangi ürünlerde (seri no)
  kullanıldı; o ürünler kime satıldı?
- **Bütünden parçaya:** bir seri numaralı ürünün içinde hangi parçalar, hangi lotlardan, kaçar adet var?

## Yapı

| Klasör | İçerik |
|---|---|
| `backend/` | .NET 10 Web API (katmanlar: EntityLayer, DataAccessLayer, BusinessLayer, SharedLayer, PresentationLayer, Tests) |
| `frontend/` | Next.js web arayüzü |

Veritabanı SQL Server; şema EF Core migration'larıyla kurulur.

## Gerekenler

- .NET SDK 10 (sürüm `global.json`'da)
- SQL Server (Express yeterli)
- Node.js LTS

## Kurulum

1. Ayar dosyasını oluşturun:

   ```
   copy backend\PresentationLayer\appsettings.Development.example.json backend\PresentationLayer\appsettings.Development.json
   ```

   İçinde bağlantı cümlesini, `OrnekVeri:AdminSifre`'yi ve `Jwt:Anahtar`'ı (en az 32 karakter,
   rastgele) doldurun. Bu dosya repoya girmez.

2. Veritabanını kurup örnek veriyi yükleyin (migration'lar da uygulanır):

   ```
   dotnet run --project backend/PresentationLayer -- ornek-veri
   ```

   `--sifirla` eklenirse mevcut veri silinip örnek veri baştan yüklenir.
   Giriş: kullanıcı adı `admin`, şifre ayar dosyasındaki `OrnekVeri:AdminSifre`.

## Çalıştırma

API (http://localhost:5291, geliştirme ortamında Swagger: http://localhost:5291/swagger):

```
dotnet run --project backend/PresentationLayer
```

Web arayüzü (http://localhost:3000):

```
cd frontend
npm install
npm run dev
```

## Testler

```
dotnet test StokLotSistemi.sln
```

Testler gerçek SQL Server'da ayrı bir veritabanında (`LotTakip_Test`) çalışır; her çalıştırmada
yeniden kurulur. Başka bir ad için `LOTTAKIP_TEST_DB` ortam değişkeni kullanılır.

Web arayüzü için: `npm run lint` ve `npm run build` (`frontend` klasöründe).

## Temel kurallar

- Her mal girişine sistem bir stok giriş numarası verir (`SG-0001`); tedarikçinin paketteki lot
  numarası ayrıca tutulur ve aramada bulunur.
- Stok düşümü FIFO: sipariş tarihi en eski giriş önce kullanılır; gerekirse birden çok girişten düşülür.
- Aynı anda üretilen ürünler ortak bir ürün lot numarası (`U1-L001`), her ürün kendi seri
  numarasını (`U1-0001`) alır. Seri no öneki ürün kodundan oluşur.
- Stok yetmezse üretim kaydedilmez, eksik parçalar ve adetleri gösterilir.
- Hatalı işaretlenen stok girişi istenirse üretime kapatılır. Ürünler kendiliğinden geri çağrılmaz;
  geri çağrılacaklar seçilir. Hatalı lotlu ürün satışta uyarı verir ve ancak onayla satılır.
- Geri çağrılan ürün müşteriden iade alınır ya da sorun yoksa müşteriye geri verilir.
- Bir seri numarası yalnız bir kez satılır. Üretim ve satış kayıtları silinmez.
- Kendi kaydolan kullanıcı yönetici onaylayınca giriş yapabilir. Şifremi unuttum bağlantısı
  e-postayla gider (`Posta` ayarları); SMTP ayarlı değilse e-posta API klasöründe `posta/` altına yazılır.
- Kullanıcı yönetimi, hatalı işaretleme, geri çağırma ve stok düzeltme yalnız yönetici rolündedir.

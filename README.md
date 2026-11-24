# VetClinic: Veteriner Kliniği Randevu ve Hasta Takip Sistemi

| | |
|---|---|
| **Öğrenci** | Batuhan Bektaş |
| **Öğrenci No** | _B2XXXXXXXX_ |
| **Ders / Grup** | Web Programlama – _X. Öğretim, Grup X_ |
| **GitHub** | https://github.com/batuhanbektas/VeterinerKlinik |

## Proje Hakkında

Sakarya Üniversitesi Web Programlama dersi proje ödevi. Veteriner kliniği için geliştirilen bir ASP.NET Core MVC uygulamasıdır:

- Hayvan sahipleri kayıt olur, evcil hayvanlarını ve aşı kayıtlarını yönetir, randevu alır.
- Randevular hekimin müsaitliğine, klinik çalışma saatlerine ve hizmet süresine göre çakışma kontrolünden geçer.
- Veterinerler randevularını onaylar/reddeder ve muayene notu girer.
- Admin klinik, hizmet, uzmanlık ve hekimleri yönetir, raporları görür.
- REST API ve MCP sunucusu aynı servis katmanını kullanır.

## Teknolojiler

- ASP.NET Core MVC (.NET 10), C#
- Entity Framework Core (Code First + Migration), SQL Server LocalDB
- ASP.NET Core Identity (rol bazlı yetkilendirme)
- ModelContextProtocol.AspNetCore (MCP sunucusu)
- Bootstrap 5, jQuery, jQuery Validation Unobtrusive

## Kurulum

Gereksinimler: .NET 10 SDK, SQL Server LocalDB (Visual Studio ile birlikte gelir).

```bash
# 1) Bağlantı dizesini User Secrets'a kaydet (appsettings.json'a yazılmaz)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\mssqllocaldb;Database=VetClinicDb;Trusted_Connection=True;MultipleActiveResultSets=true"

# 2) Veritabanını migration'lardan oluştur
dotnet tool install --global dotnet-ef --version "10.*"   # sadece ilk sefer
dotnet ef database update

# 3) Çalıştır
dotnet run
```

## Proje Yapısı

```
VeterinerKlinik/
├── Models/Entities/   Veritabanı varlıkları (Pet, Appointment, Veterinarian...)
├── Data/              ApplicationDbContext (ilişkiler, index'ler, silme kuralları)
├── Migrations/        EF Core migration dosyaları
├── Controllers/       MVC controller'ları
└── Views/             Razor görünümleri
```

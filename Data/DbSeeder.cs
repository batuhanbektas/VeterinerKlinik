using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VetClinic.Models;
using VetClinic.Models.Entities;

namespace VetClinic.Data;

// uygulama açılırken rolleri, admini ve örnek verileri ekler
// her açılışta çalışıyor ama zaten varsa tekrar eklemiyor
public static class DbSeeder
{
    // ödevde istenen admin hesabı (öğrenci no kısmını kendi numaramla değiştireceğim)
    private const string AdminEmail = "G231210379@sakarya.edu.tr";
    private const string AdminPassword = "sau";

    // örnek veteriner ve üyelerin ortak şifresi, test ederken kolaylık olsun
    private const string SamplePassword = "sau123";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // bekleyen migration varsa uygula, elle database update yapmaya gerek kalmasın
        await context.Database.MigrateAsync();

        // 1) roller
        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // 2) admin
        await CreateUserAsync(userManager, AdminEmail, AdminPassword, "Sistem", "Yöneticisi", Roles.Admin);

        // 3) örnek veriler, sadece veritabanı boşsa (hizmet yoksa boş sayıyorum)
        if (await context.Services.AnyAsync())
            return;

        await SeedSampleDataAsync(context, userManager);
    }

    // kullanıcı yoksa oluşturup rolünü verir, varsa var olanı döndürür
    private static async Task<ApplicationUser> CreateUserAsync(UserManager<ApplicationUser> userManager,
        string email, string password, string firstName, string lastName, string role)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email, // giriş e-postayla yapılsın diye kullanıcı adı = e-posta
                Email = email,
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new Exception($"{email} oluşturulamadı: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        if (!await userManager.IsInRoleAsync(user, role))
            await userManager.AddToRoleAsync(user, role);

        return user;
    }

    private static async Task SeedSampleDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        // önce kullanıcılar. usermanager kendi içinde SaveChanges çağırdığı için
        // diğer kayıtları bunlardan sonra ekliyorum, karışmasın
        var ayseUser = await CreateUserAsync(userManager, "ayse.yilmaz@example.com", SamplePassword, "Ayşe", "Yılmaz", Roles.Veteriner);
        var mehmetUser = await CreateUserAsync(userManager, "mehmet.kaya@example.com", SamplePassword, "Mehmet", "Kaya", Roles.Veteriner);
        var ali = await CreateUserAsync(userManager, "ali.demir@example.com", SamplePassword, "Ali", "Demir", Roles.Uye);
        var zeynep = await CreateUserAsync(userManager, "zeynep.kara@example.com", SamplePassword, "Zeynep", "Kara", Roles.Uye);

        // klinik
        context.Clinics.Add(new Clinic
        {
            Name = "Patiler Veteriner Kliniği",
            Address = "Arifiye, Sakarya",
            OpeningTime = new TimeOnly(9, 0),
            ClosingTime = new TimeOnly(18, 0)
        });

        // uzmanlıklar
        var kucukHayvan = new Specialty { Name = "Küçük Hayvan" };
        var egzotik = new Specialty { Name = "Egzotik Hayvan" };
        var cerrahi = new Specialty { Name = "Cerrahi" };
        var dermatoloji = new Specialty { Name = "Dermatoloji" };
        context.Specialties.AddRange(kucukHayvan, egzotik, cerrahi, dermatoloji);

        // hizmetler
        var muayene = new Service { Name = "Genel Muayene", DurationMinutes = 30, Price = 500 };
        var asi = new Service { Name = "Aşı", DurationMinutes = 15, Price = 350 };
        var kisirlastirma = new Service { Name = "Kısırlaştırma", DurationMinutes = 90, Price = 3000 };
        var disTemizligi = new Service { Name = "Diş Temizliği", DurationMinutes = 60, Price = 1500 };
        var tiras = new Service { Name = "Tıraş / Bakım", DurationMinutes = 45, Price = 600 };
        context.Services.AddRange(muayene, asi, kisirlastirma, disTemizligi, tiras);

        // veterinerler
        var ayse = new Veterinarian
        {
            UserId = ayseUser.Id,
            Specialties = new List<Specialty> { kucukHayvan, dermatoloji },
            Services = new List<Service> { muayene, asi, tiras }
        };

        var mehmet = new Veterinarian
        {
            UserId = mehmetUser.Id,
            Specialties = new List<Specialty> { cerrahi, egzotik },
            Services = new List<Service> { muayene, kisirlastirma, disTemizligi }
        };

        // ayşe hafta içi her gün 09-17
        var haftaIci = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
        foreach (var gun in haftaIci)
        {
            ayse.Availabilities.Add(new VetAvailability { DayOfWeek = gun, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(17, 0) });
        }

        // mehmet pazartesi, çarşamba, cuma 10-18 ve cumartesi yarım gün
        foreach (var gun in new[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday })
        {
            mehmet.Availabilities.Add(new VetAvailability { DayOfWeek = gun, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(18, 0) });
        }
        mehmet.Availabilities.Add(new VetAvailability { DayOfWeek = DayOfWeek.Saturday, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(14, 0) });

        context.Veterinarians.AddRange(ayse, mehmet);

        // evcil hayvanlar ve aşıları. tarihleri bugüne göre veriyorum ki
        // "geçmiş / yaklaşan aşı" ekranında her zaman ikisi de görünsün
        var bugun = DateOnly.FromDateTime(DateTime.Today);

        var pamuk = new Pet
        {
            OwnerId = ali.Id, Name = "Pamuk", Species = "Kedi", Breed = "Tekir",
            BirthDate = new DateOnly(2022, 5, 10), WeightKg = 4.2m
        };
        pamuk.Vaccinations.Add(new VaccinationRecord { VaccineName = "Karma Aşı", AdministeredDate = bugun.AddMonths(-11), NextDueDate = bugun.AddMonths(1) });
        pamuk.Vaccinations.Add(new VaccinationRecord { VaccineName = "Kuduz", AdministeredDate = bugun.AddMonths(-2), NextDueDate = bugun.AddMonths(10) });

        var karabas = new Pet
        {
            OwnerId = ali.Id, Name = "Karabaş", Species = "Köpek", Breed = "Golden Retriever",
            BirthDate = new DateOnly(2020, 3, 15), WeightKg = 28.5m
        };
        // bu aşının sonraki dozu geçmiş, gecikmiş aşı örneği olsun
        karabas.Vaccinations.Add(new VaccinationRecord { VaccineName = "Kuduz", AdministeredDate = bugun.AddYears(-1).AddDays(-10), NextDueDate = bugun.AddDays(-10) });
        karabas.Vaccinations.Add(new VaccinationRecord { VaccineName = "Karma Aşı", AdministeredDate = bugun.AddMonths(-3), NextDueDate = bugun.AddMonths(9) });

        // başka bir üyenin hayvanı, ileride IDOR testi için (ali bunu görememeli)
        var boncuk = new Pet
        {
            OwnerId = zeynep.Id, Name = "Boncuk", Species = "Tavşan", Breed = "Hollanda Lop",
            BirthDate = new DateOnly(2024, 1, 20), WeightKg = 1.8m
        };
        boncuk.Vaccinations.Add(new VaccinationRecord { VaccineName = "RHD (Tavşan Hemorajik)", AdministeredDate = bugun.AddMonths(-6), NextDueDate = bugun.AddMonths(6) });

        context.Pets.AddRange(pamuk, karabas, boncuk);

        // örnek randevular. süre ve ücret hizmetten kopyalanıyor (randevu anındaki değer)
        context.Appointments.AddRange(
            // geçen haftaki tamamlanmış muayene + muayene notu
            new Appointment
            {
                Pet = pamuk, Service = muayene, Veterinarian = ayse,
                StartTime = NextDay(DayOfWeek.Monday, 10).AddDays(-7),
                DurationMinutes = muayene.DurationMinutes, Price = muayene.Price,
                Status = AppointmentStatus.Tamamlandi,
                ExaminationNote = new ExaminationNote { Note = "Genel durumu iyi, kilosu normal. Karma aşının zamanı yaklaşıyor, hatırlatıldı." }
            },
            // onay bekleyen randevu
            new Appointment
            {
                Pet = karabas, Service = disTemizligi, Veterinarian = mehmet,
                StartTime = NextDay(DayOfWeek.Wednesday, 11),
                DurationMinutes = disTemizligi.DurationMinutes, Price = disTemizligi.Price,
                Status = AppointmentStatus.Bekliyor
            },
            // onaylanmış randevu
            new Appointment
            {
                Pet = boncuk, Service = muayene, Veterinarian = mehmet,
                StartTime = NextDay(DayOfWeek.Friday, 14),
                DurationMinutes = muayene.DurationMinutes, Price = muayene.Price,
                Status = AppointmentStatus.Onaylandi
            }
        );

        await context.SaveChangesAsync();
    }

    // bugünden sonraki ilk "day" gününü verilen saatle döndürür
    // örnek: NextDay(DayOfWeek.Monday, 10) -> önümüzdeki pazartesi 10:00
    private static DateTime NextDay(DayOfWeek day, int hour)
    {
        var date = DateTime.Today.AddDays(1);
        while (date.DayOfWeek != day)
            date = date.AddDays(1);

        return date.AddHours(hour);
    }
}

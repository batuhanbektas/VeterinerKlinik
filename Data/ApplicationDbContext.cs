using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VetClinic.Models.Entities;

namespace VetClinic.Data;

// IdentityDbContext'ten türettim, identity tabloları da aynı veritabanında olsun diye
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Clinic> Clinics { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Specialty> Specialties { get; set; }
    public DbSet<Veterinarian> Veterinarians { get; set; }
    public DbSet<VetAvailability> VetAvailabilities { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<VaccinationRecord> VaccinationRecords { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<ExaminationNote> ExaminationNotes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // bu olmazsa identity tabloları bozuluyor

        // decimal alanlar için migration uyarı vermesin diye
        builder.Entity<Service>().Property(s => s.Price).HasPrecision(10, 2);
        builder.Entity<Appointment>().Property(a => a.Price).HasPrecision(10, 2);
        builder.Entity<Pet>().Property(p => p.WeightKg).HasPrecision(6, 2);

        // hizmet adı tekrar etmesin
        builder.Entity<Service>().HasIndex(s => s.Name).IsUnique();

        // aynı kullanıcının aynı isimde 2 hayvanı olmasın, mcp'de isimle arayacağız
        builder.Entity<Pet>().HasIndex(p => new { p.OwnerId, p.Name }).IsUnique();

        // kullanıcı silinince hayvanları da gitmesin (cascade yerine restrict)
        builder.Entity<Pet>()
            .HasOne(p => p.Owner).WithMany(u => u.Pets)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Veterinarian>()
            .HasOne(v => v.User).WithOne(u => u.Veterinarian)
            .HasForeignKey<Veterinarian>(v => v.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // randevusu olan hayvan, hekim veya hizmet silinemesin
        builder.Entity<Appointment>()
            .HasOne(a => a.Pet).WithMany(p => p.Appointments)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Appointment>()
            .HasOne(a => a.Veterinarian).WithMany(v => v.Appointments)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Appointment>()
            .HasOne(a => a.Service).WithMany(s => s.Appointments)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

namespace VetClinic.Models.Entities;

// giriş bilgileri ApplicationUser'da, burası hekime özel kısım
public class Veterinarian
{
    public int Id { get; set; }

    public string UserId { get; set; } = "";
    public ApplicationUser? User { get; set; }

    // çoka çok ilişkiler
    public List<Specialty> Specialties { get; set; } = new();
    public List<Service> Services { get; set; } = new();

    public List<VetAvailability> Availabilities { get; set; } = new();
    public List<Appointment> Appointments { get; set; } = new();
}

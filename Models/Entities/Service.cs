using System.ComponentModel.DataAnnotations;

namespace VetClinic.Models.Entities;

// muayene, aşı, kısırlaştırma gibi hizmetler
public class Service
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Hizmet adı zorunludur.")]
    [StringLength(100)]
    [Display(Name = "Hizmet Adı")]
    public string Name { get; set; } = "";

    // çakışma kontrolünde lazım
    [Range(5, 480, ErrorMessage = "Süre 5 ile 480 dakika arasında olmalı.")]
    [Display(Name = "Süre (dk)")]
    public int DurationMinutes { get; set; }

    [Range(0, 100000, ErrorMessage = "Geçerli bir ücret girin.")]
    [Display(Name = "Ücret")]
    public decimal Price { get; set; }

    public List<Veterinarian> Veterinarians { get; set; } = new();
    public List<Appointment> Appointments { get; set; } = new();
}

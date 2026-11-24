using System.ComponentModel.DataAnnotations;

namespace VetClinic.Models.Entities;

// hekimin hangi gün hangi saatler arası çalıştığı
public class VetAvailability : IValidatableObject
{
    public int Id { get; set; }

    public int VeterinarianId { get; set; }
    public Veterinarian? Veterinarian { get; set; }

    [Display(Name = "Gün")]
    public DayOfWeek DayOfWeek { get; set; }

    [Display(Name = "Başlangıç")]
    public TimeOnly StartTime { get; set; }

    [Display(Name = "Bitiş")]
    public TimeOnly EndTime { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (EndTime <= StartTime)
            yield return new ValidationResult("Bitiş saati başlangıçtan sonra olmalı.", new[] { nameof(EndTime) });
    }
}

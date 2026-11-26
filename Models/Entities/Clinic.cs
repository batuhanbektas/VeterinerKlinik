using System.ComponentModel.DataAnnotations;

namespace VetClinic.Models.Entities;

public class Clinic : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Klinik adı zorunludur.")]
    [StringLength(100)]
    [Display(Name = "Klinik Adı")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Adres zorunludur.")]
    [StringLength(250)]
    [Display(Name = "Adres")]
    public string Address { get; set; } = "";

    [Required(ErrorMessage = "Açılış saati zorunludur.")]
    [Display(Name = "Açılış Saati")]
    public TimeOnly OpeningTime { get; set; }

    [Required(ErrorMessage = "Kapanış saati zorunludur.")]
    [Display(Name = "Kapanış Saati")]
    public TimeOnly ClosingTime { get; set; }

    // kapanış açılıştan önce olamaz
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (ClosingTime <= OpeningTime)
            yield return new ValidationResult("Kapanış saati açılış saatinden sonra olmalı.", new[] { nameof(ClosingTime) });
    }
}

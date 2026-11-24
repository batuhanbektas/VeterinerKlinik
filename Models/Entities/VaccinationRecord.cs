using System.ComponentModel.DataAnnotations;

namespace VetClinic.Models.Entities;

public class VaccinationRecord : IValidatableObject
{
    public int Id { get; set; }

    public int PetId { get; set; }
    public Pet? Pet { get; set; }

    [Required(ErrorMessage = "Aşı adı zorunludur.")]
    [StringLength(100)]
    [Display(Name = "Aşı Adı")]
    public string VaccineName { get; set; } = "";

    [Display(Name = "Yapıldığı Tarih")]
    public DateOnly AdministeredDate { get; set; }

    // boşsa sonraki doz yok demek
    [Display(Name = "Sonraki Doz")]
    public DateOnly? NextDueDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (NextDueDate.HasValue && NextDueDate <= AdministeredDate)
            yield return new ValidationResult("Sonraki doz tarihi yapıldığı tarihten sonra olmalı.", new[] { nameof(NextDueDate) });
    }
}

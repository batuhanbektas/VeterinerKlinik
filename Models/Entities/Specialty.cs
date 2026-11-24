using System.ComponentModel.DataAnnotations;

namespace VetClinic.Models.Entities;

// küçük hayvan, egzotik, cerrahi vs.
public class Specialty
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Uzmanlık adı zorunludur.")]
    [StringLength(100)]
    [Display(Name = "Uzmanlık")]
    public string Name { get; set; } = "";

    public List<Veterinarian> Veterinarians { get; set; } = new();
}

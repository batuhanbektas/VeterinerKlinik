using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace VetClinic.Models.Entities;

// identity'nin user sınıfına ad soyad ekledim
// admin, veteriner, üye hepsi burada, farkları rolleri
public class ApplicationUser : IdentityUser
{
    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(50)]
    [Display(Name = "Ad")]
    public string FirstName { get; set; } = "";

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(50)]
    [Display(Name = "Soyad")]
    public string LastName { get; set; } = "";

    public List<Pet> Pets { get; set; } = new();

    // veteriner değilse null
    public Veterinarian? Veterinarian { get; set; }
}

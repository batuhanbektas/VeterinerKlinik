using System.ComponentModel.DataAnnotations;

namespace VetClinic.Models.Entities;

public class Pet
{
    public int Id { get; set; }

    // sahibini formdan almıyorum, giriş yapan kullanıcıdan alacağım
    public string OwnerId { get; set; } = "";
    public ApplicationUser? Owner { get; set; }

    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(50)]
    [Display(Name = "Adı")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Tür zorunludur.")]
    [StringLength(30)]
    [Display(Name = "Tür")]
    public string Species { get; set; } = ""; // kedi, köpek vs.

    [StringLength(50)]
    [Display(Name = "Irk")]
    public string? Breed { get; set; }

    [Display(Name = "Doğum Tarihi")]
    public DateOnly? BirthDate { get; set; }

    [Range(0.1, 500, ErrorMessage = "Kilo 0.1 ile 500 arasında olmalı.")]
    [Display(Name = "Kilo (kg)")]
    public decimal? WeightKg { get; set; }

    [Display(Name = "Fotoğraf")]
    public string? PhotoPath { get; set; }

    public List<VaccinationRecord> Vaccinations { get; set; } = new();
    public List<Appointment> Appointments { get; set; } = new();
}

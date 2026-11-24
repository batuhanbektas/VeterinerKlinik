using System.ComponentModel.DataAnnotations;

namespace VetClinic.Models.Entities;

// hekimin randevu sonrası yazdığı not
public class ExaminationNote
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    [Required(ErrorMessage = "Not boş olamaz.")]
    [StringLength(1000)]
    [Display(Name = "Muayene Notu")]
    public string Note { get; set; } = "";
}

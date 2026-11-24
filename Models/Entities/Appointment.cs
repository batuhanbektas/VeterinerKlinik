using System.ComponentModel.DataAnnotations;

namespace VetClinic.Models.Entities;

public enum AppointmentStatus
{
    [Display(Name = "Bekliyor")] Bekliyor,
    [Display(Name = "Onaylandı")] Onaylandi,
    [Display(Name = "Reddedildi")] Reddedildi,
    [Display(Name = "Tamamlandı")] Tamamlandi,
    [Display(Name = "İptal")] Iptal
}

public class Appointment
{
    public int Id { get; set; }

    public int PetId { get; set; }
    public Pet? Pet { get; set; }

    public int ServiceId { get; set; }
    public Service? Service { get; set; }

    public int VeterinarianId { get; set; }
    public Veterinarian? Veterinarian { get; set; }

    [Display(Name = "Tarih / Saat")]
    public DateTime StartTime { get; set; }

    // hizmetin fiyatı sonradan değişirse eski randevu etkilenmesin diye burada da tutuyorum
    [Display(Name = "Süre (dk)")]
    public int DurationMinutes { get; set; }

    [Display(Name = "Ücret")]
    public decimal Price { get; set; }

    [Display(Name = "Durum")]
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Bekliyor;

    public ExaminationNote? ExaminationNote { get; set; }
}

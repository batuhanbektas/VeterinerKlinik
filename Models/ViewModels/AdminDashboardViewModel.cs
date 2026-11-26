namespace VetClinic.Models.ViewModels;

// dashboard'daki özet sayılar
public class AdminDashboardViewModel
{
    public int ServiceCount { get; set; }
    public int SpecialtyCount { get; set; }
    public int VeterinarianCount { get; set; }
    public int MemberCount { get; set; }
    public int PetCount { get; set; }
    public int PendingAppointmentCount { get; set; }
    public int TodayAppointmentCount { get; set; }
}

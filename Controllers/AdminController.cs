using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetClinic.Data;
using VetClinic.Models;
using VetClinic.Models.Entities;
using VetClinic.Models.ViewModels;

namespace VetClinic.Controllers;

// admin paneli ana sayfası (dashboard)
[Authorize(Roles = Roles.Admin)]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var model = new AdminDashboardViewModel
        {
            ServiceCount = await _context.Services.CountAsync(),
            SpecialtyCount = await _context.Specialties.CountAsync(),
            VeterinarianCount = await _context.Veterinarians.CountAsync(),
            MemberCount = (await _userManager.GetUsersInRoleAsync(Roles.Uye)).Count,
            PetCount = await _context.Pets.CountAsync(),
            PendingAppointmentCount = await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Bekliyor),
            // .Date kullanmak yerine aralık verdim, sql'e daha düzgün çevriliyor
            TodayAppointmentCount = await _context.Appointments.CountAsync(a => a.StartTime >= today && a.StartTime < tomorrow)
        };

        return View(model);
    }
}

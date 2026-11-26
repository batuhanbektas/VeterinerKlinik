using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetClinic.Data;
using VetClinic.Models;
using VetClinic.Models.Entities;

namespace VetClinic.Controllers;

// hizmet ekleme/düzenleme/silme, sadece admin
[Authorize(Roles = Roles.Admin)]
public class ServicesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServicesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var services = await _context.Services.OrderBy(s => s.Name).ToListAsync();
        return View(services);
    }

    public async Task<IActionResult> Details(int id)
    {
        // bu hizmeti veren hekimleri de göstermek için include ettim
        var service = await _context.Services
            .Include(s => s.Veterinarians).ThenInclude(v => v.User)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (service == null)
            return NotFound();

        ViewBag.AppointmentCount = await _context.Appointments.CountAsync(a => a.ServiceId == id);
        return View(service);
    }

    public IActionResult Create()
    {
        return View();
    }

    // Bind ile sadece formdaki alanları alıyorum (overposting olmasın)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,DurationMinutes,Price")] Service service)
    {
        // veritabanında unique index var ama hata sayfası yerine düzgün mesaj çıksın
        if (await _context.Services.AnyAsync(s => s.Name == service.Name))
            ModelState.AddModelError("Name", "Bu isimde bir hizmet zaten var.");

        if (!ModelState.IsValid)
            return View(service);

        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Hizmet eklendi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
            return NotFound();

        return View(service);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DurationMinutes,Price")] Service service)
    {
        if (id != service.Id)
            return NotFound();

        if (!await _context.Services.AnyAsync(s => s.Id == id))
            return NotFound();

        if (await _context.Services.AnyAsync(s => s.Name == service.Name && s.Id != id))
            ModelState.AddModelError("Name", "Bu isimde bir hizmet zaten var.");

        if (!ModelState.IsValid)
            return View(service);

        // ücreti değiştirmek eski randevuları etkilemez, randevuda ücret ayrıca saklanıyor
        _context.Update(service);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Hizmet güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
            return NotFound();

        ViewBag.AppointmentCount = await _context.Appointments.CountAsync(a => a.ServiceId == id);
        return View(service);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
            return NotFound();

        // randevusu olan hizmet silinemez (veritabanında da restrict ayarlı)
        if (await _context.Appointments.AnyAsync(a => a.ServiceId == id))
        {
            TempData["Error"] = "Bu hizmete ait randevular olduğu için silinemez.";
            return RedirectToAction(nameof(Index));
        }

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Hizmet silindi.";
        return RedirectToAction(nameof(Index));
    }
}

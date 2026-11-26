using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetClinic.Data;
using VetClinic.Models;
using VetClinic.Models.Entities;

namespace VetClinic.Controllers;

// klinik bilgileri (ad, adres, çalışma saatleri), sadece admin
// çalışma saatleri randevu alırken kontrol edilecek
[Authorize(Roles = Roles.Admin)]
public class ClinicsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ClinicsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var clinics = await _context.Clinics.OrderBy(c => c.Name).ToListAsync();
        return View(clinics);
    }

    public async Task<IActionResult> Details(int id)
    {
        var clinic = await _context.Clinics.FindAsync(id);
        if (clinic == null)
            return NotFound();

        return View(clinic);
    }

    public IActionResult Create()
    {
        // formda boş gelmesin diye varsayılan saatler
        var clinic = new Clinic
        {
            OpeningTime = new TimeOnly(9, 0),
            ClosingTime = new TimeOnly(18, 0)
        };
        return View(clinic);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Address,OpeningTime,ClosingTime")] Clinic clinic)
    {
        // kapanış > açılış kontrolü Clinic.Validate içinde, ModelState'e oradan düşüyor
        if (!ModelState.IsValid)
            return View(clinic);

        _context.Clinics.Add(clinic);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Klinik eklendi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var clinic = await _context.Clinics.FindAsync(id);
        if (clinic == null)
            return NotFound();

        return View(clinic);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,OpeningTime,ClosingTime")] Clinic clinic)
    {
        if (id != clinic.Id)
            return NotFound();

        if (!await _context.Clinics.AnyAsync(c => c.Id == id))
            return NotFound();

        if (!ModelState.IsValid)
            return View(clinic);

        _context.Update(clinic);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Klinik bilgileri güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var clinic = await _context.Clinics.FindAsync(id);
        if (clinic == null)
            return NotFound();

        return View(clinic);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var clinic = await _context.Clinics.FindAsync(id);
        if (clinic == null)
            return NotFound();

        _context.Clinics.Remove(clinic);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Klinik silindi.";
        return RedirectToAction(nameof(Index));
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetClinic.Data;
using VetClinic.Models;
using VetClinic.Models.Entities;

namespace VetClinic.Controllers;

// uzmanlık CRUD, sadece admin
[Authorize(Roles = Roles.Admin)]
public class SpecialtiesController : Controller
{
    private readonly ApplicationDbContext _context;

    public SpecialtiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // listede kaç hekimin bu uzmanlığa sahip olduğunu da gösteriyorum
        var specialties = await _context.Specialties
            .Include(s => s.Veterinarians)
            .OrderBy(s => s.Name)
            .ToListAsync();

        return View(specialties);
    }

    public async Task<IActionResult> Details(int id)
    {
        var specialty = await _context.Specialties
            .Include(s => s.Veterinarians).ThenInclude(v => v.User)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (specialty == null)
            return NotFound();

        return View(specialty);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name")] Specialty specialty)
    {
        if (await _context.Specialties.AnyAsync(s => s.Name == specialty.Name))
            ModelState.AddModelError("Name", "Bu uzmanlık zaten var.");

        if (!ModelState.IsValid)
            return View(specialty);

        _context.Specialties.Add(specialty);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Uzmanlık eklendi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var specialty = await _context.Specialties.FindAsync(id);
        if (specialty == null)
            return NotFound();

        return View(specialty);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Specialty specialty)
    {
        if (id != specialty.Id)
            return NotFound();

        if (!await _context.Specialties.AnyAsync(s => s.Id == id))
            return NotFound();

        if (await _context.Specialties.AnyAsync(s => s.Name == specialty.Name && s.Id != id))
            ModelState.AddModelError("Name", "Bu uzmanlık zaten var.");

        if (!ModelState.IsValid)
            return View(specialty);

        _context.Update(specialty);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Uzmanlık güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var specialty = await _context.Specialties
            .Include(s => s.Veterinarians)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (specialty == null)
            return NotFound();

        return View(specialty);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var specialty = await _context.Specialties.FindAsync(id);
        if (specialty == null)
            return NotFound();

        // hekimlerle olan bağlantısı (ara tablo) kendiliğinden silinir, hekimler silinmez
        _context.Specialties.Remove(specialty);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Uzmanlık silindi.";
        return RedirectToAction(nameof(Index));
    }
}

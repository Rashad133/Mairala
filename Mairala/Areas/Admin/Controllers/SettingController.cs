using Mairala.Areas.Admin.ViewModels;
using Mairala.DAL;
using Mairala.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mairala.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingController : Controller
    {
        private readonly AppDbContext _db;
        public SettingController(AppDbContext db)
        {
            _db=db;
        }
        public async Task<IActionResult> Index()
        {
            List<Setting> settings = await _db.Settings.ToListAsync();
            return View(settings);
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Setting setting = await _db.Settings.FirstOrDefaultAsync(x => x.Id == id);
            if (setting == null) return NotFound();

            UpdateSettingVM update = new UpdateSettingVM
            {
                Key = setting.Key,
                Value = setting.Value,
            };
            return View(update);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateSettingVM update)
        {
            if(!ModelState.IsValid) return View(update);
            Setting setting= await _db.Settings.FirstOrDefaultAsync(x=>x.Id == id);
            if (setting == null) return NotFound();

            bool result = await _db.Settings.AnyAsync(x => x.Key.Trim().ToLower() == update.Key.Trim().ToLower() && x.Id != id);
            if (result)
            {
                ModelState.AddModelError("Key", "is exists");
                return View(setting);
            }

            setting.Key = update.Value;
            setting.Value = update.Value;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}

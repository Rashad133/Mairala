using Mairala.Areas.Admin.ViewModels;
using Mairala.DAL;
using Mairala.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace Mairala.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PositonController : Controller
    {
        private readonly AppDbContext _db;
        public PositonController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            List<Position> positions = await _db.Positions.Include(x => x.Members).ToListAsync();
            return View(positions);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreatePositionVM create)
        {
            if (!ModelState.IsValid) return View(create);

            bool result = await _db.Positions.AnyAsync(x => x.Name.Trim().ToLower() == create.Name.Trim().ToLower());

            if (result)
            {
                ModelState.AddModelError("Name", "is exists");
                return View(create);
            }

            Position position = new Position
            {
                Name = create.Name,
            };

            await _db.Positions.AddAsync(position);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if(id<=0) return BadRequest();
            Position position = await _db.Positions.FirstOrDefaultAsync(x => x.Id == id);
            if (position == null) return NotFound();

            UpdatePositionVM update = new UpdatePositionVM
            {
                Name = position.Name,
            };
            return View(update);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdatePositionVM update)
        {
            if(!ModelState.IsValid) return View(update);
            Position position = await _db.Positions.FirstOrDefaultAsync(x => x.Id == id);
            if (position == null) return NotFound();

            bool result = await _db.Positions.AnyAsync(x=>x.Name.Trim().ToLower()==update.Name.Trim().ToLower() && x.Id!=id);
            if (result)
            {
                ModelState.AddModelError("Name","is exists");
                return View(update);
            }

            position.Name=update.Name;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Position position= await _db.Positions.FirstOrDefaultAsync(x=>x.Id==id);
            if (position == null) return NotFound();
            _db.Positions.Remove(position);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
         
    }
}

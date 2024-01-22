using Mairala.Areas.Admin.ViewModels;
using Mairala.DAL;
using Mairala.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mairala.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;
        public CategoryController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _db.Categories.Include(x=>x.Projects).ToListAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVM create)
        {
            if (!ModelState.IsValid) return View(create);

            bool result = await _db.Categories.AnyAsync(x => x.Name.Trim().ToLower() == create.Name.Trim().ToLower());

            if (result)
            {
                ModelState.AddModelError("Name", "is exists");
                return View(create);
            }

            Category category = new Category
            {
                Name = create.Name,
            };

            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Category category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null) return NotFound();

            UpdateCategoryVM update = new UpdateCategoryVM
            {
                Name = category.Name,
            };
            return View(update);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateCategoryVM update)
        {
            if (!ModelState.IsValid) return View(update);
            Category category= await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if ( category== null) return NotFound();

            bool result = await _db.Positions.AnyAsync(x => x.Name.Trim().ToLower() == update.Name.Trim().ToLower() && x.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "is exists");
                return View(update);
            }

            category.Name = update.Name;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Category category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null) return NotFound();
            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

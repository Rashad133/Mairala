using Mairala.Areas.Admin.ViewModels;
using Mairala.DAL;
using Mairala.Models;
using Mairala.Utilities.Extentions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mairala.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProjectController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public ProjectController(AppDbContext db,IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Project> projects = await _db.Projects.Include(x=>x.Category).ToListAsync();
            return View(projects);
        }

        public async Task<IActionResult> Create()
        {
            CreateProjectVM create = new CreateProjectVM
            {
                Categories= await _db.Categories.ToListAsync(),
            };
            return View(create);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectVM create)
        {
            if(!ModelState.IsValid)
            {
                create.Categories = await _db.Categories.ToListAsync();
                return View(create);
            }

            bool result = await _db.Projects.AnyAsync(x=>x.Name.Trim().ToLower()==create.Name.Trim().ToLower());
            if (result)
            {
                create.Categories = await _db.Categories.ToListAsync();
                ModelState.AddModelError("Name","is exists");
                return View(create);
            }
            if (!create.Photo.ValidateType())
            {
                create.Categories = await _db.Categories.ToListAsync();
                ModelState.AddModelError("Photo","not valid");
                return View(create);
            }
            if (!create.Photo.ValidateSize(10))
            {
                create.Categories = await _db.Categories.ToListAsync();
                ModelState.AddModelError("Photo", "max 10mb");
                return View(create);
            }

            Project project = new Project
            {
                Name = create.Name,
                CategoryId = create.CategoryId,
                Image= await create.Photo.CreateFile(_env.WebRootPath,"assets","images"),
            };

            await _db.Projects.AddAsync(project);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Project project = await _db.Projects.FirstOrDefaultAsync(x => x.Id == id);
            if (project == null) return NotFound();

            UpdateProjectVM update = new UpdateProjectVM
            {
                Name = project.Name,
                Categories= await _db.Categories.ToListAsync(),
                CategoryId=project.Id,
            };
            return View(update);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id ,UpdateProjectVM update)
        {
            if (!ModelState.IsValid)
            {
                update.Categories= await _db.Categories.ToListAsync();
                return View(update);

            }
            Project project = await _db.Projects.FirstOrDefaultAsync(x=>x.Id == id);
            if (project == null) return NotFound();

            bool result = await _db.Projects.AnyAsync(x=>x.Name.Trim().ToLower()==update.Name.Trim().ToLower() && x.Id!=id);

            if (result)
            {
                update.Categories = await _db.Categories.ToListAsync();
                ModelState.AddModelError("Name","is exists");
                return View(update);
            }

            if (update.Photo is not null)
            {
                if (!update.Photo.ValidateType())
                {
                    update.Categories = await _db.Categories.ToListAsync();
                    ModelState.AddModelError("Name", "not valid");
                    return View(update);
                }
                if (!update.Photo.ValidateSize(10))
                {
                    update.Categories = await _db.Categories.ToListAsync();
                    ModelState.AddModelError("Name", "max 10mb");
                    return View(update);
                }
                project.Image.DeleteFile(_env.WebRootPath, "assets", "images");
                project.Image=await update.Photo.CreateFile(_env.WebRootPath, "assets", "images");
            }

            project.Name= update.Name;
            project.CategoryId= update.CategoryId;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Project project = await _db.Projects.FirstOrDefaultAsync(x => x.Id == id);
            if (project == null) return NotFound();
            _db.Remove(project);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    } 
}

using Mairala.Areas.Admin.ViewModels;
using Mairala.DAL;
using Mairala.Models;
using Mairala.Utilities.Extentions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace Mairala.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MemberController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public MemberController(AppDbContext db,IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Member> members = await _db.Members.Include(x=>x.Position).ToListAsync();
            return View(members);
        }

        public async Task<IActionResult> Create()
        {
            CreateMemberVM create = new CreateMemberVM
            {
                Positions=await _db.Positions.ToListAsync()
            };
            return View(create);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateMemberVM create)
        {
            if (!ModelState.IsValid)
            {
                create.Positions = await _db.Positions.ToListAsync();
                return View(create);
            }

            bool result = await _db.Members.AnyAsync(x=>x.Name.Trim().ToLower()==create.Name.Trim().ToLower());

            if (result)
            {
                ModelState.AddModelError("Name","is exists");
                return View(create);
            }

            if (!create.Photo.ValidateType())
            {
                ModelState.AddModelError("Photo","not valid");
                return View(create);
            }
            if (!create.Photo.ValidateSize(10))
            {
                ModelState.AddModelError("Photo", "max 10mb");
                return View(create);
            }

            Member member = new Member
            {
                Name = create.Name,
                FaceLink = create.FaceLink,
                TwitLink = create.TwitLink,
                LinkedLink = create.LinkedLink,
                PositionId = create.PositionId,
                Image = await create.Photo.CreateFile(_env.WebRootPath, "assets", "images")
            };

            await _db.Members.AddAsync(member);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
          
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Member member = await _db.Members.FirstOrDefaultAsync(x => x.Id == id);
            if(member == null)  return NotFound();

            UpdateMemberVM update = new UpdateMemberVM
            {
                Name= member.Name,
                FaceLink = member.FaceLink,
                TwitLink = member.TwitLink,
                LinkedLink = member.LinkedLink,
                PositionId = member.PositionId,
                Positions=await _db.Positions.ToListAsync(),
            };
            return View(update);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateMemberVM update)
        {
            if (!ModelState.IsValid)
            {
                update.Positions = await _db.Positions.ToListAsync();
                return View(update);
            }

            Member member = await _db.Members.FirstOrDefaultAsync(x => x.Id == id);
            if (member == null) return NotFound();

            bool result = await _db.Members.AnyAsync(x => x.Name.Trim().ToLower() == update.Name.Trim().ToLower() && x.Id != id);
            if (result)
            {
                update.Positions= await _db.Positions.ToListAsync();
                ModelState.AddModelError("Name","is exists");
                return View(update);
            }
             
            if(update.Photo is not null)
            {
                if (!update.Photo.ValidateType())
                {
                    update.Positions=await _db.Positions.ToListAsync();
                    ModelState.AddModelError("Photo","not valid");
                    return View(update);
                }
                if (!update.Photo.ValidateSize(10))
                {
                    update.Positions=await _db.Positions.ToListAsync();
                    ModelState.AddModelError("Photo","max 10mb");
                    return View(update);
                }
                member.Image.DeleteFile(_env.WebRootPath,"assets","images");
                member.Image = await update.Photo.CreateFile(_env.WebRootPath,"assets","images");
            }
            member.Name= update.Name;
            member.LinkedLink= update.LinkedLink;
            member.FaceLink= update.FaceLink;
            member.TwitLink= update.TwitLink;
            member.PositionId= update.PositionId;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Member member = await _db.Members.FirstOrDefaultAsync(x => x.Id == id);
            if(member==null) return NotFound();

            _db.Members.Remove(member);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

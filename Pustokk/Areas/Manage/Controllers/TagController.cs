using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Pustokk.DAL;
using Pustokk.Models;

namespace Pustokk.Areas.Manage.Controllers
{

    [Area("Manage")]
    public class TagController : Controller
    {

        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Tag> Tags = _context.Tags.ToList();

            return View(Tags);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Tag tag)
        {
            if (!ModelState.IsValid) return View();

            if (_context.Tags.Any(t => t.Name.ToLower() == tag.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "tag already created!");
                return View();
            }

            _context.Tags.Add(tag);
            _context.SaveChanges();

            return RedirectToAction("index");

        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            if (id == null) return NotFound();

            Tag existtag = _context.Tags.FirstOrDefault(t => t.Id == id);

            if (existtag == null) return NotFound();

            return View(existtag);

        }

        [HttpPost]
        public IActionResult Update(Tag tag)
        {
            if (!ModelState.IsValid) return View();

            Tag existTag = _context.Tags.FirstOrDefault(t => t.Id == tag.Id);
            if (existTag == null) return NotFound();

            if (_context.Tags.Any(t => t.Id != tag.Id && t.Name.ToLower() == tag.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "tag has already created!");
                return View();
            }

            existTag.Name = tag.Name;

            _context.SaveChanges();
            return RedirectToAction("index");


        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == null) return NotFound();

            Tag tag = _context.Tags.FirstOrDefault(t => t.Id == id);
            return View(tag);
        }

        [HttpPost]
        public IActionResult Delete(Tag tag)
        {

            Tag existTag = _context.Tags.FirstOrDefault(t => t.Id == tag.Id);

            if (existTag == null)
            {
                return NotFound();
            }

            _context.Tags.Remove(existTag);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }   
}

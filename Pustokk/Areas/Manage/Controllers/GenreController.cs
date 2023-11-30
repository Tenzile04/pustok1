using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustokk.DAL;
using Pustokk.Models;

namespace Pustokk.Areas.Manage.Controllers
{
    [Area("manage")]
    public class GenreController : Controller
    {
        private readonly AppDbContext _context;

        public GenreController(AppDbContext context)
        {
           _context = context;
        }
        public IActionResult Index()
        {
            List<Genre> Genres = _context.Genres.ToList();
            return View(Genres);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Genre genre)
        {
            if (!ModelState.IsValid) return View();

            if (_context.Genres.Any(x => x.Name.ToLower() == genre.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Genre alredy exist!");
                return View();
            }

            _context.Genres.Add(genre);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Update(int id)
        {
            if (id == null) return NotFound();
            Genre existGenre = _context.Genres.FirstOrDefault(g => g.Id == id);
            if (existGenre == null) return NotFound();

            return View(existGenre);
        }
        [HttpPost]
        public IActionResult Update(Genre genre)
        {
            if (!ModelState.IsValid) return View();
            Genre existGenre = _context.Genres.FirstOrDefault(g => g.Id == genre.Id);
            if (existGenre == null) return NotFound();

            if (_context.Genres.Any(x =>x.Id!=genre.Id && x.Name.ToLower() == genre.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Genre alredy exist!");
                return View();
            }
            existGenre.Name = genre.Name;

            _context.SaveChanges();
            return RedirectToAction("index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            //if (id == null) return NotFound();

            Genre genre = _context.Genres.FirstOrDefault(g => g.Id == id);
            if (genre == null)
            {
                return NotFound();
            }
            _context.Genres.Remove(genre);
            _context.SaveChanges();
            return Ok();
        }

        //[HttpPost]
        //public IActionResult Delete(Genre genre)
        //{

        //    Genre existGenre = _context.Genres.FirstOrDefault(g => g.Id == genre.Id);

        //    if (existGenre == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Genres.Remove(existGenre);
        //    _context.SaveChanges();

        //    return RedirectToAction("Index");
        //}
    }
}

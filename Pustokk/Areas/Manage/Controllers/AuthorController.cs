using Microsoft.AspNetCore.Mvc;
using Pustokk.DAL;
using Pustokk.Models;

namespace Pustokk.Areas.Manage.Controllers
{
        [Area("Manage")]
        public class AuthorController : Controller
        {
            private readonly AppDbContext _context;
            public AuthorController(AppDbContext context)
            {
            _context = context;
            }
            public IActionResult Index()
            {
                List<Author> Authors = _context.Authors.ToList();
                return View(Authors);
            }

            [HttpGet]
            public IActionResult Create()
            {
                return View();
            }
            [HttpPost]
            public IActionResult Create(Author author)
            {
                if (!ModelState.IsValid) return View();

                if (_context.Authors.Any(a => a.FullName.ToLower() == author.FullName.ToLower()))
                {
                    ModelState.AddModelError("FullName", "Author already created!");
                    return View();
                }

            _context.Authors.Add(author);
            _context.SaveChanges();

                return RedirectToAction("Index");

            }

            [HttpGet]
            public IActionResult Update(int id)
            {
                if (id == null) return NotFound();

                Author author = _context.Authors.FirstOrDefault(a => a.Id == id);

                if (author == null) return NotFound();

                return View(author);
            }

            [HttpPost]
            public IActionResult Update(Author author)
            {
                if (!ModelState.IsValid) return View();

                Author existAuthor = _context.Authors.FirstOrDefault(a => a.Id == author.Id);

                if (existAuthor == null) return NotFound();

                if (_context.Authors.Any(a => a.Id != author.Id && a.FullName.ToLower() == author.FullName.ToLower()))
                {
                    ModelState.AddModelError("FullName", "Author has already created!");
                    return View();
                }

                existAuthor.FullName = author.FullName;

            _context.SaveChanges();
                return RedirectToAction("Index");
            }

            [HttpGet]
            public IActionResult Delete(int id)
            {
                if (id == null) return NotFound();

                Author author = _context.Authors.FirstOrDefault(a => a.Id == id);
                return View(author);
            }

            [HttpPost]
            public IActionResult Delete(Author author)
            {

                Author existAuthor = _context.Authors.FirstOrDefault(a => a.Id == author.Id);

                if (existAuthor == null)
                {
                    return NotFound();
                }

            _context.Authors.Remove(existAuthor);
            _context.SaveChanges();

                return RedirectToAction("Index");
            }
        }
}

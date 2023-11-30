using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustokk.DAL;
using Pustokk.Extencions;
using Pustokk.Models;
using System.Data;
using System.Linq;

namespace Pustokk.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BookController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public BookController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Book> books = _context.Books.ToList();
          
            return View(books);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }
        public IActionResult Create(Book book)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            if (!ModelState.IsValid) return View(book);

            if (!_context.Genres.Any(x => x.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId","Genre Not found");
                return View();
            }
            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Author Not found");
                return View();
            }
            
            bool check = false;
            if (book.TagIds != null)
            {
                foreach (var tagId in book.TagIds)
                {
                    if (!_context.Tags.Any(x => x.Id == tagId))
                    {
                        check = true;
                        break;
                    }
                }
            }
            if (check)
            {
                ModelState.AddModelError("TagId", "Tag not found");
                return View();
            }
            else
            {
                if (book.TagIds != null)
                {
                    foreach (var tagId in book.TagIds)
                    {
                        BookTag booktag = new BookTag
                        {
                            Book=book,
                            TagId = tagId,
                            
                        };
                        _context.BookTags.Add(booktag);
                    }
                }
            }

            if (book.BookPosterImageFile != null)
            {
                if (book.BookPosterImageFile.ContentType != "image/jpeg" && book.BookPosterImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("BookPosterImageFile", "can only upload .jpeg or .png");
                    return View();
                }

                if (book.BookPosterImageFile.Length > 1048576)
                {
                    ModelState.AddModelError("BookPosterImageFile", "File size must be lower than 1mb");
                    return View();
                }
                BookImage bookImage = new BookImage
                {
                    Book = book,
                    ImageUrl = Helper.SaveFile(_env.WebRootPath, "uploads/books", book.BookPosterImageFile),
                    IsPoster=true
                };
                _context.BookImages.Add(bookImage);
            }

            if (book.BookHoverImageFile != null)
            {
                if (book.BookHoverImageFile.ContentType != "image/jpeg" && book.BookHoverImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("BookHoverImageFile", "can only upload .jpeg or .png");
                    return View();
                }

                if (book.BookHoverImageFile.Length > 1048576)
                {
                    ModelState.AddModelError("BookHoverImageFile", "File size must be lower than 1mb");
                    return View();
                }
                BookImage bookImage = new BookImage
                {
                    Book = book,
                    ImageUrl = Helper.SaveFile(_env.WebRootPath, "uploads/books", book.BookHoverImageFile),
                    IsPoster = false
                };
                _context.BookImages.Add(bookImage);
            }
            if (book.ImageFiles != null)
            {
                foreach ( var imageFile in book.ImageFiles)
                {
                    if (imageFile.ContentType != "image/jpeg" && imageFile.ContentType != "image/png")
                    {
                        ModelState.AddModelError("ImageFiles", "can only upload .jpeg or .png");
                        return View();
                    }

                    if (imageFile.Length > 1048576)
                    {
                        ModelState.AddModelError("ImageFiles", "File size must be lower than 1mb");
                        return View();
                    }
                    BookImage bookImage = new BookImage
                    {
                        Book = book,
                        ImageUrl = Helper.SaveFile(_env.WebRootPath, "uploads/books", imageFile),
                        IsPoster = null
                    };
                    _context.BookImages.Add(bookImage);
                }
            }


            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
        [HttpGet]
        public IActionResult Update(int id)
        {

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            Book existbook = _context.Books.Include(x => x.BookTags).Include(x=>x.BookImages).FirstOrDefault(x => x.Id == id);
            if (existbook == null)
            {
                return NotFound();
            };
            existbook.TagIds = existbook.BookTags.Select(bt => bt.TagId).ToList();

            return View(existbook);
        }
        [HttpPost]
        public IActionResult Update(Book book)
        {
            //return Ok(book.BookImageIds);
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            if (!ModelState.IsValid) return View();

            Book existbook = _context.Books.Include(x => x.BookTags).FirstOrDefault(x => x.Id == book.Id);
            if (existbook == null) return NotFound();
            if (!_context.Genres.Any(g => g.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "Genre not found!");
                return View();
            }
            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Author not found!");
                return View();
            }
            var existBook = _context.Books
                .Include(x => x.BookImages)
                .FirstOrDefault(x => x.Id == book.Id);

            if (existBook == null)
            {
                return NotFound();
            }
            

            existBook.BookTags.RemoveAll(bt => !book.TagIds.Contains(bt.TagId));

            foreach (var tagId in book.TagIds.Where(t => !existBook.BookTags.Any(bt => bt.TagId == t)))
            {
                BookTag bookTag = new BookTag
                {
                    TagId = tagId
                };
                existBook.BookTags.Add(bookTag);
            }


            if (book.BookPosterImageFile != null)
            {
                if (book.BookPosterImageFile.ContentType != "image/jpeg" && book.BookPosterImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("BookPosterImageFile", "can only upload .jpeg or .png");
                    return View();
                }

                if (book.BookPosterImageFile.Length > 1048576)
                {
                    ModelState.AddModelError("BookPosterImageFile", "File size must be lower than 1mb");
                    return View();
                }
                existBook.BookImages.Remove(existBook.BookImages.FirstOrDefault(x => x.IsPoster == true));

                BookImage bookImage = new BookImage
                {
                    Book = book,
                    ImageUrl = Helper.SaveFile(_env.WebRootPath, "uploads/books", book.BookPosterImageFile),
                    IsPoster = true
                };
                existbook.BookImages.Add(bookImage);
            }

            if (book.BookHoverImageFile != null)
            {
                if (book.BookHoverImageFile.ContentType != "image/jpeg" && book.BookHoverImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("BookHoverImageFile", "can only upload .jpeg or .png");
                    return View();
                }

                if (book.BookHoverImageFile.Length > 1048576)
                {
                    ModelState.AddModelError("BookHoverImageFile", "File size must be lower than 1mb");
                    return View();
                }
                existBook.BookImages.Remove(existBook.BookImages.FirstOrDefault(x => x.IsPoster == false));

                BookImage bookImage = new BookImage
                {
                    Book = book,
                    ImageUrl = Helper.SaveFile(_env.WebRootPath, "uploads/books", book.BookHoverImageFile),
                    IsPoster = false
                };
                existbook.BookImages.Add(bookImage);
            }

            //existbook.BookImages.RemoveAll(bi => !book.BookImageIds.Contains(bi.Id) && bi.IsPoster ==null);
            if (book.ImageFiles != null)
            {
                if (existBook.BookImages is not null)
                {
                    existBook.BookImages.RemoveAll(bi => !book.BookImageIds.Contains(bi.Id) && bi.IsPoster == null);
                }
                foreach (var imageFile in book.ImageFiles)
                {
                    if (imageFile.ContentType != "image/jpeg" && imageFile.ContentType != "image/png")
                    {
                        ModelState.AddModelError("ImageFiles", "can only upload .jpeg or .png");
                        return View();
                    }

                    if (imageFile.Length > 1048576)
                    {
                        ModelState.AddModelError("ImageFiles", "File size must be lower than 1mb");
                        return View();
                    }
                    BookImage bookImage = new BookImage
                    {
                        Book = book,
                        ImageUrl = Helper.SaveFile(_env.WebRootPath, "uploads/books", imageFile),
                        IsPoster = null
                    };
                    existbook.BookImages.Add(bookImage);
                }
            }

            existbook.Name = book.Name;
            existbook.Description = book.Description;
            existbook.SalePrice = book.SalePrice;
            existbook.CostPrice = book.CostPrice;
            existbook.DiscountPercent = book.DiscountPercent;
            existbook.IsAvailable = book.IsAvailable;
            existbook.Tax = book.Tax;
            existbook.Code = book.Code;
            existbook.AuthorId = book.AuthorId;
            existbook.GenreId = book.GenreId;
            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            if (id == null) return NotFound();

            Book book = _context.Books.FirstOrDefault(b => b.Id == id);

            if (book == null) return NotFound();


            return View(book);
        }

        [HttpPost]
        public IActionResult Delete(Book book)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            Book wantedBook = _context.Books.FirstOrDefault(b => b.Id == book.Id);

            if (wantedBook == null)
            {
                return NotFound();
            }

            _context.Books.Remove(wantedBook);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}

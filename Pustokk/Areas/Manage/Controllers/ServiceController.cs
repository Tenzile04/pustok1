using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustokk.DAL;
using Pustokk.Models;
using System.Runtime.CompilerServices;

namespace Pustokk.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;
        public ServiceController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Service> services = new List<Service>();
            return View(services);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Service service)
        {
            if (!ModelState.IsValid) return View();

            _context.Services.Add(service);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Service service = _context.Services.FirstOrDefault(x => x.Id == id);

            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        [HttpPost]
        public IActionResult Update(Service service)
        {
            if (!ModelState.IsValid) return View();
            Service existService = _context.Services.FirstOrDefault(x => x.Id == service.Id);

            if (existService == null)
            {
                return NotFound();
            }

            existService.Title = service.Title;
            existService.Description = service.Description;
            existService.Icon = service.Icon;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            Service service = _context.Services.FirstOrDefault(x => x.Id == id);
            return View(service);
        }

        [HttpPost]
        public IActionResult Delete(Service service)
        {

            Service existService = _context.Services.FirstOrDefault(x => x.Id == service.Id);

            if (existService == null)
            {
                return NotFound();
            }
            _context.Services.Remove(existService);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}


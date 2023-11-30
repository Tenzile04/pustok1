using Microsoft.AspNetCore.Mvc;
using Pustokk.DAL;
using Pustokk.ViewModels;
using System.Diagnostics;

namespace PustokTask1.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            HomeViewModel homeViewModel = new HomeViewModel()
            {
                Sliders = _context.Sliders.ToList(),
                Services = _context.Services.ToList()
            };

            return View(homeViewModel);
        
        }
    }
}
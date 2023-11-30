using Microsoft.AspNetCore.Mvc;
using Pustokk.Models;
using Pustokk.DAL;
using Pustokk.Models;
using System.Runtime.CompilerServices;
using Pustokk.Extencions;

namespace PustokTask1.Areas.Manage.Controllers
{
    [Area("manage")]
    public class SlideController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Slider> sliders = _context.Sliders.ToList();
            return View(sliders);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Slider slider)
        {
            if (!ModelState.IsValid)
            {
                return View(slider);
            }
            if (slider.ImageFile != null)
            {

                string fileName = slider.ImageFile.FileName;
                if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "can only upload .jpeg or .png");
                    return View();
                }

                if (slider.ImageFile.Length > 1048576)
                {
                    ModelState.AddModelError("ImageFile", "File size must be lower than 1mb");
                    return View();
                }

                //fileName=fileName.Length > 64 ? fileName = fileName.Substring(fileName.Length - 64, 64) : fileName;

                //string newFileName = Guid.NewGuid().ToString() + fileName;



                //string test = _env.WebRootPath+"uploads/sliders"+fileName;
                //string path = "C:\\Users\\II novbe\\Desktop\\Pustokk\\Pustokk\\wwwroot\\uploads\\sliders\\" + newFileName;

                //using (FileStream fileStream = new FileStream(path, FileMode.Create))
                //{
                //    slider.ImageFile.CopyTo(fileStream);
                //}


                slider.ImageUrl = Helper.SaveFile(_env.WebRootPath, "uploads/sliders", slider.ImageFile);
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Required!");
                return View();
            }
            _context.Sliders.Add(slider);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Update(int id)
        {

            Slider wantedSlider = _context.Sliders.FirstOrDefault(s => s.Id == id);

            if (wantedSlider == null) return NotFound();

            return View(wantedSlider);
        }

        [HttpPost]
        public IActionResult Update(Slider slider)
        {

            Slider existSlider = _context.Sliders.FirstOrDefault(x => x.Id == slider.Id);

            if (existSlider == null) return NotFound();
            if (!ModelState.IsValid) return View(slider);


            if (slider.ImageFile != null)
            {

                if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "can only upload .jpeg or .png");
                    return View(slider);
                }

                if (slider.ImageFile.Length > 1048576)
                {
                    ModelState.AddModelError("ImageFile", "File size must be lower than 1mb");
                    return View(slider);
                }


                string deletepath = Path.Combine(_env.WebRootPath, "uploads/sliders", existSlider.ImageUrl);
                if (System.IO.File.Exists(deletepath))
                {
                    System.IO.File.Delete(deletepath);
                }

                existSlider.ImageUrl = Helper.SaveFile(_env.WebRootPath, "uploads/sliders", slider.ImageFile);
            }
            existSlider.Title = slider.Title;
            existSlider.Description = slider.Description;
            existSlider.RedirectUrl = slider.RedirectUrl;
            existSlider.RedirecText = slider.RedirecText;

            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Slider slider = _context.Sliders.Find(id);
            if (slider == null) return NotFound();
            return View(slider);

            _context.Sliders.Remove(slider);
            _context.SaveChanges();
            return Ok();
        }

        //[HttpPost]
        //public IActionResult Delete(Slider slider)
        //{
        //    Slider existSlider = _context.Sliders.FirstOrDefault(x => x.Id == slider.Id);
        //    string path = Path.Combine(_env.WebRootPath, "uploads/sliders", existSlider.ImageUrl);

        //   if(existSlider.ImageFile!=null)
        //    {
        //        if(System.IO.File.Exists(path))

        //        {
        //            System.IO.File.Delete(path);
        //        }
        //    }


        //    _context.Sliders.Remove(existSlider);
        //    _context.SaveChanges();

        //    return RedirectToAction("Index");
        //}
    }
}

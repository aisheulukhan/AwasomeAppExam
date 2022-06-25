using AWESOMEAPPBack.DAL;
using AWESOMEAPPBack.Models;
using AWESOMEAPPBack.Utilities.Extention;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AWESOMEAPPBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;

        private readonly IWebHostEnvironment _env;

        public CustomerController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Customers.ToList());
        }
        public ActionResult Create()
        {
           return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Customer customer)
        {

            if (customer.Photo.CheckSize(800))
            {
                ModelState.AddModelError("Photo", "File size must be less than 800kb");
                return View();
            }
            if (!customer.Photo.CheckType("image/"))
            {
                ModelState.AddModelError("Photo", "File must be image");
                return View();
            }
            customer.Image = await customer.Photo.SaveFileAsync(Path.Combine(_env.WebRootPath, "images"));
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            Customer customer = _context.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return NotFound();
            return View(customer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (ModelState.IsValid)
            {
                var s = await _context.Customers.FindAsync(customer.Id);
                s.Name = customer.Name;
                s.Desc = customer.Desc;
                s.Founder = customer.Founder;
                if (customer.Photo != null)
                {
                    if (customer.Image != null)
                    {
                        string filePath = Path.Combine(_env.WebRootPath, "images", customer.Image);
                        System.IO.File.Delete(filePath);
                    }
                    s.Image = ProcessUploadedFile(customer);
                }
                _context.Update(s);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        private string ProcessUploadedFile(Customer customer)
        {
            string uniqueFileName = null;

            if (customer.Photo != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + customer.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    customer.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            Customer customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();
            if (System.IO.File.Exists(Path.Combine(_env.WebRootPath, "images", customer.Image)))
            {
                System.IO.File.Delete(Path.Combine(_env.WebRootPath, "images", customer.Image));
            }
            _context.Customers.Remove(customer);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}

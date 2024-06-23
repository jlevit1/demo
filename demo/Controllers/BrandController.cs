using demo.Data;
using demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;

namespace demo.Controllers
{
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext context;

        public BrandController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var brands = context.Brands.ToList();

            return View(brands);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var brand = context.Brands.FirstOrDefault(b => b.Id == id);
            context.Brands.Remove(brand);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Brand brand)
        {
            if (ModelState.IsValid)
            {
                context.Brands.Add(brand);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var brand = context.Brands.FirstOrDefault(p => p.Id == id);
            return View(brand);
        }

        [HttpPost]
        public IActionResult Edit(Brand brand)
        {
            if (ModelState.IsValid)
            {
                context.Brands.Update(brand);
                context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(brand);
        }
    }
}

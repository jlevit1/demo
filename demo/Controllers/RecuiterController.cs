using demo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using demo.Models;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Xml.Linq;
using System.Collections.Generic;

namespace demo.Controllers
{
    public class RecuiterController : Controller
    {
        //declare application db context
        //use context to manage database
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> _userManager;

        //declare constructor
        public RecuiterController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Index()
        {
            var Recuiters = context.Recuiters.ToList();
            return View(Recuiters);
        }

        [Authorize]
        public IActionResult Detail(int? id)
        {
            //if id is null return error
            if (id == null)
            {
                return NotFound();
            }
            //find Recuiter whose id is similar to id in url
            var Recuiter = context.Recuiters.FirstOrDefault(p => p.Id == id);
            return View(Recuiter);
        }

        //DELETE function
        //SQL: DELETE FROM Recuiters WHERE Id = "id"
        public IActionResult Delete(int? id)
        {
            //if id is null return error
            if (id == null)
            {
                return NotFound();
            }
            //else remove Recuiter by id
            var Recuiter = context.Recuiters.FirstOrDefault(p => p.Id == id);
            context.Recuiters.Remove(Recuiter);
            //save change to database
            context.SaveChanges();
            //redirect to Recuiter list page
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var Recuiter = context.Recuiters.FirstOrDefault(p => p.Id == id);
            ViewBag.Brands = context.Brands.ToList();
            return View(Recuiter);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public IActionResult Edit(Recuiter Recuiter, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                var existingRecuiter = context.Recuiters.AsNoTracking().FirstOrDefault(p => p.Id == Recuiter.Id);
                if (existingRecuiter != null)
                {
                    // Update Recuiter in the database
                    context.Recuiters.Update(Recuiter);
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            return View(Recuiter);
        }


        [HttpPost]
        public IActionResult Search(string keyword)
        {
            var Recuiters = context.Recuiters.Where(p => p.FullName.Contains(keyword) || p.Address.Contains(keyword)).ToList();
            ViewBag.Message = keyword;
            return View("Index", Recuiters);
        }

        public IActionResult SortAsc()
        {
            var Recuiters = context.Recuiters.OrderBy(p => p.FullName).ToList();
            return View("Index", Recuiters);
        }

        public IActionResult SortDesc()
        {
            var Recuiters = context.Recuiters.OrderByDescending(p => p.FullName).ToList();
            return View("Index", Recuiters);
        }
    }
}

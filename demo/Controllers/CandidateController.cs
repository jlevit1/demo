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
    public class CandidateController : Controller
    {
        //declare application db context
        //use context to manage database
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> _userManager;

        //declare constructor
        public CandidateController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Index()
        {
            var Candidates = context.Candidates.ToList();
            return View(Candidates);
        }

        [Authorize]
        public IActionResult Detail(int? id)
        {
            //if id is null return error
            if (id == null)
            {
                return NotFound();
            }
            //find Candidate whose id is similar to id in url
            var Candidate = context.Candidates.FirstOrDefault(p => p.Id == id);
            return View(Candidate);
        }

        //DELETE function
        //SQL: DELETE FROM Candidates WHERE Id = "id"
        public IActionResult Delete(int? id)
        {
            //if id is null return error
            if (id == null)
            {
                return NotFound();
            }
            //else remove Candidate by id
            var Candidate = context.Candidates.FirstOrDefault(p => p.Id == id);
            context.Candidates.Remove(Candidate);
            //save change to database
            context.SaveChanges();
            //redirect to Candidate list page
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var Candidate = context.Candidates.FirstOrDefault(p => p.Id == id);
            ViewBag.Brands = context.Brands.ToList();
            return View(Candidate);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public IActionResult Edit(Candidate Candidate, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                var existingCandidate = context.Candidates.AsNoTracking().FirstOrDefault(p => p.Id == Candidate.Id);
                if (existingCandidate != null)
                {
                    // Update Candidate in the database
                    context.Candidates.Update(Candidate);
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            return View(Candidate);
        }

        public FileResult Download(string CV)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\cv", CV);
            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "application/octet-stream", CV);
        }

        [HttpPost]
        public IActionResult Search(string keyword)
        {
            var Candidates = context.Candidates.Where(p => p.FullName.Contains(keyword) || p.Skills.Contains(keyword) || p.Address.Contains(keyword)).ToList();
            return View("Index", Candidates);
        }

        public IActionResult SortAsc()
        {
            var Candidates = context.Candidates.OrderBy(p => p.FullName).ToList();
            return View("Index", Candidates);
        }

        public IActionResult SortDesc()
        {
            var Candidates = context.Candidates.OrderByDescending(p => p.FullName).ToList();
            return View("Index", Candidates);
        }
    }
}

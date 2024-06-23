using demo.Data;
using demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var jobs = _context.Jobs.Include(x => x.Recuiter).ToList();
            return View(jobs);
        }


        [HttpPost]
        public IActionResult Search(string keyword)
        {
            var Jobs = _context.Jobs.Include(x => x.Recuiter).Where(p => p.Name.Contains(keyword) || p.Skills.Contains(keyword) || p.Requiment.Contains(keyword)).ToList();
            ViewBag.Message = keyword;
            return View("Index", Jobs);
        }


        [Authorize(Roles = "Candidate")]
        [HttpGet]
        public IActionResult Apply(int? id)
        {
            //if id is null return error
            if (id == null)
            {
                return NotFound();
            }
            //find Job whose id is similar to id in url
            var Job = _context.Jobs.FirstOrDefault(p => p.Id == id);
            return View(Job);
        }


        [Authorize(Roles = "Candidate")]
        [HttpPost]
        public IActionResult Apply(Job Job, IFormFile file)
        {
            string userId = _userManager.GetUserId(User);
            var candidate = _context.Candidates.FirstOrDefault(x => x.UserId.Equals(userId));
            if (candidate != null)
            {
                var item = new JobCandidate() { JobId = Job.Id, CandidateId = candidate.Id };
                item.CV = candidate.CV;

                if (file != null && file.Length > 0)
                {
                    var fileName = candidate.Id + "_" + Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\cv", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        // Copy (upload) image file from original folder to project folder
                        file.CopyTo(stream);
                    }

                    // Update product image path
                    item.CV = fileName;
                }
                _context.Add(item);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Job);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

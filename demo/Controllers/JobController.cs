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
    public class JobController : Controller
    {
        //declare application db context
        //use context to manage database
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> _userManager;

        //declare constructor
        public JobController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Recuiter")]
        public IActionResult Index()
        {
            //get Job data from "Jobs" table
            //and save to an array (list)
            //note: must include Brand column to display Brand information
            var Jobs = new List<Job>();
            string userId = _userManager.GetUserId(User);
            var recuiter = context.Recuiters.FirstOrDefault(x => x.UserId.Equals(userId));
            if (recuiter != null)
            {
                Jobs = context.Jobs.Where(x=>x.RecuiterId.Equals(recuiter.Id)).ToList();
            }
            //render view along with data
            return View(Jobs);
        }

        [Authorize]
        public IActionResult Detail(int? id)
        {
            //if id is null return error
            if (id == null)
            {
                return NotFound();
            }
            //find Job whose id is similar to id in url
            var Job = context.Jobs.Include(x => x.JobCandidates).ThenInclude(x => x.Candidate).FirstOrDefault(p => p.Id == id);
            return View(Job);
        }

        //DELETE function
        //SQL: DELETE FROM Jobs WHERE Id = "id"
        public IActionResult Delete(int? id)
        {
            //if id is null return error
            if (id == null)
            {
                return NotFound();
            }
            //else remove Job by id
            var Job = context.Jobs.FirstOrDefault(p => p.Id == id);
            context.Jobs.Remove(Job);
            //save change to database
            context.SaveChanges();
            //redirect to Job list page
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Recuiter")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize(Roles = "Recuiter")]
        [HttpPost]
        public IActionResult Add(Job Job, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                string userId = _userManager.GetUserId(User);
                var recuiter = context.Recuiters.FirstOrDefault(x => x.UserId.Equals(userId));
                if (recuiter != null)
                {
                    //validate image is valid or not
                    if (Image != null && Image.Length > 0)
                    {
                        //set image file name
                        //Note: should add a prefix such as "Id" in the image name to make unique image file name
                        var fileName = Job.Id + "_" + Path.GetFileName(Image.FileName);
                        //set image file location
                        //Note: should create a folder named "images" or "photos" in "wwwroot" to store upload files 
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            //copy (upload) image file from original folder to project folder
                            Image.CopyTo(stream);
                        }
                    }
                    Job.RecuiterId = recuiter.Id;
                    context.Jobs.Add(Job);
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.Message = "qưdqwdqwdqwd";
            }
            return View(Job);
        }

        [Authorize(Roles = "Recuiter")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var Job = context.Jobs.FirstOrDefault(p => p.Id == id);
            ViewBag.Brands = context.Brands.ToList();
            return View(Job);
        }

        [Authorize(Roles = "Recuiter")]
        [HttpPost]
        public IActionResult Edit(Job Job, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                var existingJob = context.Jobs.AsNoTracking().FirstOrDefault(p => p.Id == Job.Id);
                if (existingJob != null)
                {
                    // Update Job in the database
                    Job.RecuiterId = existingJob.RecuiterId;
                    context.Jobs.Update(Job);
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            return View(Job);
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
            var Jobs = context.Jobs.Where(p => p.Name.Contains(keyword) || p.Skills.Contains(keyword) || p.Requiment.Contains(keyword)).ToList();
            ViewBag.Message = keyword;
            return View("Index", Jobs);
        }

        public IActionResult SortAsc()
        {
            var Jobs = context.Jobs.OrderBy(p => p.Name).ToList();
            return View("Index", Jobs);
        }

        public IActionResult SortDesc()
        {
            var Jobs = context.Jobs.OrderByDescending(p => p.Name).ToList();
            return View("Index", Jobs);
        }

        [HttpPost]
        public IActionResult SearchCandidate(string keyword)
        {
            var Jobs = context.Jobs.Where(p => p.Name.Contains(keyword) || p.Skills.Contains(keyword) || p.Requiment.Contains(keyword)).ToList();
            return View("Detail", Jobs);
        }
        public IActionResult SortCandidateAsc(int? id)
        {
            var rs = context.Jobs.Include(x => x.JobCandidates).ThenInclude(x => x.Candidate).FirstOrDefault(p => p.Id == id);
            rs.JobCandidates.OrderBy(x => x.Candidate.FullName);
            return View("Detail", rs);
        }

        public IActionResult SortCandidateDesc(int? id)
        {
            var rs = context.Jobs.Include(x => x.JobCandidates).ThenInclude(x => x.Candidate).FirstOrDefault(p => p.Id == id);
            rs.JobCandidates.OrderByDescending(x => x.Candidate.FullName);
            return View("Detail", rs);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using demo.Data;
using demo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.Mime.MediaTypeNames;

namespace demo.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Email { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            [Display(Name = "FullName")]
            public string FullName { get; set; }
            [Display(Name = "Skills")]
            public string Skills { get; set; }
            [Display(Name = "Address")]
            public string Address { get; set; }
            [Display(Name = "Scale")]
            public decimal Scale { get; set; }
            [Display(Name = "Description")]
            public string Description { get; set; }
            [Display(Name = "File")]
            public IFormFile SelectedFile { get; set; }
            [Display(Name = "CV")]
            public string CV { get; set; }
            [Display(Name = "Image")]
            public string Image { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            Email = email;
            if (User.IsInRole("Candidate"))
            {
                Candidate candidate = _context.Candidates.FirstOrDefault(x => x.UserId.Equals(user.Id));
                Input = new InputModel
                {
                    PhoneNumber = phoneNumber,
                    Address = candidate?.Address ?? "",
                    FullName = candidate?.FullName ?? "",
                    Skills = candidate?.Skills ?? "",
                    CV = candidate?.CV ?? "",
                };
            }
            else if (User.IsInRole("Recuiter"))
            {
                Recuiter recuiter = _context.Recuiters.FirstOrDefault(x => x.UserId.Equals(user.Id));
                Input = new InputModel
                {
                    PhoneNumber = phoneNumber,
                    Address= recuiter?.Address ?? "",
                    Scale = recuiter?.Scale ?? 0,
                    Description = recuiter?.Description ?? "",
                    FullName = recuiter?.FullName ?? "",
                    Image = recuiter?.Image ?? "",
                };
            } else
            {
                Input = new InputModel
                {
                    PhoneNumber = phoneNumber
                };
            }

        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            var file = Input.SelectedFile;

            if (User.IsInRole("Candidate"))
            {
                Candidate candidate = _context.Candidates.FirstOrDefault(x => x.UserId.Equals(user.Id));
                if (candidate is null)
                {
                    candidate = new Candidate()
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        FullName = Input.FullName,
                        Address = Input.Address,
                        Phone = phoneNumber,
                        Skills = Input.Skills,
                    };
                    if (file != null && file.Length > 0)
                    {
                        //set image file name
                        //Note: should add a prefix such as "Id" in the image name to make unique image file name
                        var fileName = user.Id + "_" + Path.GetFileName(file.FileName);
                        //set image file location
                        //Note: should create a folder named "images" or "photos" in "wwwroot" to store upload files 
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\cv", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            //copy (upload) image file from original folder to project folder
                            file.CopyTo(stream);
                        }
                        candidate.CV = "/cv/" + fileName;
                    }
                    await _context.Candidates.AddAsync(candidate);
                }
                else
                {
                    if (file != null && file.Length > 0)
                    {
                        //set image file name
                        //Note: should add a prefix such as "Id" in the image name to make unique image file name
                        var fileName = user.Id + "_" + Path.GetFileName(file.FileName);
                        //set image file location
                        //Note: should create a folder named "images" or "photos" in "wwwroot" to store upload files 
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\cv", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            //copy (upload) image file from original folder to project folder
                            file.CopyTo(stream);
                        }
                        candidate.CV = fileName;
                    }
                    candidate.FullName = Input.FullName;
                    candidate.Address = Input.Address;
                    candidate.Phone = phoneNumber;
                    candidate.Skills = Input.Skills;
                    _context.Candidates.Update(candidate);
                }
            _context.SaveChanges();
            }
            else if (User.IsInRole("Recuiter"))
            {
                Recuiter recuiter = _context.Recuiters.FirstOrDefault(x => x.UserId.Equals(user.Id));
                if (recuiter is null)
                {
                    recuiter =new Recuiter()
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        FullName = Input.FullName,
                        Address = Input.Address,
                        Phone = phoneNumber,
                        Description = Input.Description,
                        Scale = Input.Scale,
                    };

                    if (file != null && file.Length > 0)
                    {
                        //set image file name
                        //Note: should add a prefix such as "Id" in the image name to make unique image file name
                        var fileName = user.Id + "_" + Path.GetFileName(file.FileName);
                        //set image file location
                        //Note: should create a folder named "images" or "photos" in "wwwroot" to store upload files 
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            //copy (upload) image file from original folder to project folder
                            file.CopyTo(stream);
                        }
                        recuiter.Image = "/images/" + fileName;
                    }
                    await _context.Recuiters.AddAsync(recuiter);
                }
                else
                {
                    if (file != null && file.Length > 0)
                    {
                        //set image file name
                        //Note: should add a prefix such as "Id" in the image name to make unique image file name
                        var fileName = user.Id + "_" + Path.GetFileName(file.FileName);
                        //set image file location
                        //Note: should create a folder named "images" or "photos" in "wwwroot" to store upload files 
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            //copy (upload) image file from original folder to project folder
                            file.CopyTo(stream);
                        }
                        recuiter.Image = "/images/" + fileName;
                    }
                    recuiter.FullName = Input.FullName;
                    recuiter.Address = Input.Address;
                    recuiter.Phone = phoneNumber;
                    recuiter.Description = Input.Description;
                    recuiter.Scale = Input.Scale;

                    _context.Recuiters.Update(recuiter);
                }
            _context.SaveChanges();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}

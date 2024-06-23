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

namespace demo.Controllers
{
    public class ProductController : Controller
    {
        //declare application db context
        //use context to manage database
        private readonly ApplicationDbContext context;

        //declare constructor
        public ProductController(ApplicationDbContext context)
        {
            this.context = context;
        }

        //READ function (1A)
        //View all products : Product list
        //SQL: SELECT * FROM Products
        public IActionResult Index()
        {
            //get product data from "Products" table
            //and save to an array (list)
            //note: must include Brand column to display Brand information
            var products = context.Products.Include(p => p.Brand).ToList();

            //render view along with data
            return View(products);
        }

        //READ function (1B) - with async & await
        //View all products : Product list
        //SQL: SELECT * FROM Products
        //public async Task<IActionResult> Index()
        //{
        //    //get product data from "Products" table
        //    //and save to an array (list)
        //    var products = await context.Products.ToListAsync();

        //    //render view along with data
        //    return View(products);
        //}

        //READ function (2)
        //View product by ID : Product Detail
        //SQL: SELECT * FROM Products WHERE Id = "id"
        //int? id : id is nullable

        [Authorize]
        public IActionResult Detail(int? id)
        {
            //if id is null return error
            if (id == null)
            {
                return NotFound();
            }
            //find product whose id is similar to id in url
            var product = context.Products.Include(p => p.Brand).FirstOrDefault(p => p.Id == id);
            return View(product);
        }

        //DELETE function
        //SQL: DELETE FROM Products WHERE Id = "id"
        public IActionResult Delete(int? id)
        {
            //if id is null return error
            if (id == null)
            {
                return NotFound();
            }
            //else remove product by id
            var product = context.Products.FirstOrDefault(p => p.Id == id);
            context.Products.Remove(product);
            //save change to database
            context.SaveChanges();
            //redirect to product list page
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Brands = context.Brands.ToList();
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public IActionResult Add(Product product, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                //validate image is valid or not
                if (Image != null && Image.Length > 0)
                {
                    //set image file name
                    //Note: should add a prefix such as "Id" in the image name to make unique image file name
                    var fileName = product.Id + "_" + Path.GetFileName(Image.FileName);    
                    //set image file location
                    //Note: should create a folder named "images" or "photos" in "wwwroot" to store upload files 
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        //copy (upload) image file from original folder to project folder
                        Image.CopyTo(stream);
                    }
                    product.Image = "/images/" + fileName;
                }
                context.Products.Add(product);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        [Authorize(Roles = "Administrator, User")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == id);
            ViewBag.Brands = context.Brands.ToList();
            return View(product);
        }

        [Authorize(Roles = "Administrator, User")]
        [HttpPost]
        public IActionResult Edit(Product product, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                // Check if a new image file is uploaded
                if (Image != null && Image.Length > 0)
                {
                    var fileName = product.Id + "_" + Path.GetFileName(Image.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        // Copy (upload) image file from original folder to project folder
                        Image.CopyTo(stream);
                    }

                    // Update product image path
                    product.Image = "/images/" + fileName;
                }
                // Use the existing image file if no new image file is uploaded
                else
                {
                    var existingProduct = context.Products.AsNoTracking().FirstOrDefault(p => p.Id == product.Id);
                    if (existingProduct != null)
                    {
                        product.Image = existingProduct.Image;
                    }
                }

                // Update product in the database
                context.Products.Update(product);
                context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        [HttpPost]
        public IActionResult Search (string keyword)
        {
            var products = context.Products.Include(p => p.Brand).Where(p => p.Name.Contains(keyword)).ToList();
            return View("Index", products);
        }

        public IActionResult SortAsc()
        {
            var products = context.Products.Include(p => p.Brand).OrderBy(p => p.Name).ToList();
            return View("Index", products);
        }

        public IActionResult SortDesc()
        {
            var products = context.Products.Include(p => p.Brand).OrderByDescending(p => p.Name).ToList();
            return View("Index", products);
        }
    }
}

using GraniteHouseV2.Data;
using GraniteHouseV2.Models;
using GraniteHouseV2.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraniteHouseV2.Controllers
{
    [Authorize(Roles = AppConstants.AdminRole)]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: ProductController
        public IActionResult Index()
        {
            IEnumerable<Product> productList = _db.Product.Include(x => x.Category).Include(a => a.ApplicationType);

            return View(productList);
        }

        // GET: ProductController/CreateEdit
        public IActionResult CreateEdit(int? id)
        {
            ProductVM productVM = new() {
                Product = new Product(),
                CategorySelectList = _db.Category.Select(i => new SelectListItem {
                    Text = i.Name,
                    Value = i.CategoryId.ToString()
                }),
                ApplicationTypeSelectList = _db.ApplicationType.Select(i => new SelectListItem { 
                    Text = i.Name,
                    Value = i.ApplicationTypeId.ToString()
                })
            };

            if (id == null)
            {
                return View(productVM);
            }

            productVM.Product = _db.Product.Find(id);
            if (productVM.Product == null)
            {
                return NotFound();
            }
            return View(productVM);
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEdit(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.ProductId == 0)
                {
                    //Creating a Product
                    string uploadPath = webRootPath + AppConstants.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string fileExtension = Path.GetExtension(files[0].FileName);

                    // Copy file
                    using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName + fileExtension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + fileExtension;

                    _db.Product.Add(productVM.Product);
                }
                else
                {
                    //Updating a product
                    var productFromDb = _db.Product.AsNoTracking().FirstOrDefault(x => x.ProductId == productVM.Product.ProductId);

                    if (productFromDb != null)
                    {
                        if (files.Count > 0)
                        {
                            string uploadPath = webRootPath + AppConstants.ImagePath;
                            string fileName = Guid.NewGuid().ToString();
                            string fileExtension = Path.GetExtension(files[0].FileName);

                            // Remove current file
                            var currentFilePath = Path.Combine(uploadPath, productFromDb.Image);

                            if (System.IO.File.Exists(currentFilePath))
                            {
                                System.IO.File.Delete(currentFilePath);
                            }

                            using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName + fileExtension), FileMode.Create))
                            {
                                files[0].CopyTo(fileStream);
                            }

                            // Update the image path
                            productVM.Product.Image = fileName + fileExtension;
                        }
                        else
                        {
                            productVM.Product.Image = productFromDb.Image;
                        }

                        _db.Product.Update(productVM.Product);
                    }
                }

                _db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            productVM.CategorySelectList = _db.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.CategoryId.ToString()
            });

            productVM.ApplicationTypeSelectList = _db.ApplicationType.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.ApplicationTypeId.ToString()
            });

            return View(productVM);
        }

        // GET: ProductController/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0 )
            {
                return NotFound();
            }
            Product product = _db.Product.Include(p => p.Category).Include(a => a.ApplicationType)
                .Where(p => p.ProductId == id).FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? productId)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;

            var productObj = _db.Product.Find(productId);
            if (productObj == null)
            {
                return NotFound();
            }
            _db.Product.Remove(productObj);
            _db.SaveChanges();

            // Remove image
            string uploadPath = webRootPath + AppConstants.ImagePath;
            var currentFilePath = Path.Combine(uploadPath, productObj.Image);

            if (System.IO.File.Exists(currentFilePath))
            {
                System.IO.File.Delete(currentFilePath);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

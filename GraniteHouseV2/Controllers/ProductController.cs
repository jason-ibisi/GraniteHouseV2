using GraniteHouseV2_DataAccess.Repository.IRepository;
using GraniteHouseV2_Models;
using GraniteHouseV2_Models.ViewModels;
using GraniteHouseV2_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

namespace GraniteHouseV2.Controllers
{
    [Authorize(Roles = AppConstants.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductRepository productRepository, IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: ProductController
        public IActionResult Index()
        {
            IEnumerable<Product> productList = _productRepository.GetAll(includeProperties:"Category,ApplicationType");
            return View(productList);
        }

        // GET: ProductController/CreateEdit
        public IActionResult CreateEdit(int? id)
        {
            ProductVM productVM = new() {
                Product = new Product(),
                CategorySelectList = _productRepository.GetAllDropdownList(AppConstants.CategoryName),
                ApplicationTypeSelectList = _productRepository.GetAllDropdownList(AppConstants.ApplicationTypeName)
            };

            if (id == null)
            {
                return View(productVM);
            }

            productVM.Product = _productRepository.Find(id.GetValueOrDefault());
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

                    _productRepository.Add(productVM.Product);
                }
                else
                {
                    //Updating a product
                    var productFromDb = _productRepository.FirstOrDefault(x => x.ProductId == productVM.Product.ProductId, isTracking: false);

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

                        _productRepository.Update(productVM.Product);
                    }
                }

                _productRepository.Save();
                TempData[AppConstants.Success] = "Request completed successfully";

                return RedirectToAction(nameof(Index));
            }

            productVM.CategorySelectList = _productRepository.GetAllDropdownList(AppConstants.CategoryName);
            productVM.ApplicationTypeSelectList = _productRepository.GetAllDropdownList(AppConstants.ApplicationTypeName);
            TempData[AppConstants.Error] = "Error completing request";

            return View(productVM);
        }

        // GET: ProductController/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0 )
            {
                return NotFound();
            }
            Product product = _productRepository.FirstOrDefault(p => p.ProductId == id, includeProperties: "Category,ApplicationType");
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

            var productObj = _productRepository.Find(productId.GetValueOrDefault());
            if (productObj == null)
            {
                return NotFound();
            }
            _productRepository.Remove(productObj);
            _productRepository.Save();

            // Remove image
            string uploadPath = webRootPath + AppConstants.ImagePath;
            var currentFilePath = Path.Combine(uploadPath, productObj.Image);

            if (System.IO.File.Exists(currentFilePath))
            {
                System.IO.File.Delete(currentFilePath);
            }

            TempData[AppConstants.Success] = "Product deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}

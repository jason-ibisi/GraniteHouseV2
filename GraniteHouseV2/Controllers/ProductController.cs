using GraniteHouseV2.Data;
using GraniteHouseV2.Models;
using GraniteHouseV2.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GraniteHouseV2.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: ProductController
        public IActionResult Index()
        {
            IEnumerable<Product> productList = _db.Product.Include(x => x.Category);

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
        public IActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductController/Edit/5
        public IActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductController/Delete/5
        public IActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

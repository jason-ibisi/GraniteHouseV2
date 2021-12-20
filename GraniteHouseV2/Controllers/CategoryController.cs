using GraniteHouseV2.Data;
using GraniteHouseV2_Models;
using GraniteHouseV2_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GraniteHouseV2.Controllers
{
    [Authorize(Roles = AppConstants.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = _db.Category;
            return View(categories);
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Category.Add(category);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        //GET - EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var categoryObj = _db.Category.Find(id);
            if (categoryObj == null)
            {
                return NotFound();
            }
            return View(categoryObj);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Category.Update(category);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        //GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var categoryObj = _db.Category.Find(id);
            if (categoryObj == null)
            {
                return NotFound();
            }
            return View(categoryObj);
        }

        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? CategoryId)
        {
            var categoryObj = _db.Category.Find(CategoryId);
            if (categoryObj == null)
            {
                return NotFound();
            }
            _db.Category.Remove(categoryObj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

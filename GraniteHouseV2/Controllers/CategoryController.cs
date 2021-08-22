using GraniteHouseV2.Data;
using GraniteHouseV2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GraniteHouseV2.Controllers
{
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
            _db.Category.Add(category);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}

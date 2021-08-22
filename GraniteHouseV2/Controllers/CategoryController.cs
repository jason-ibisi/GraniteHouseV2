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
    }
}

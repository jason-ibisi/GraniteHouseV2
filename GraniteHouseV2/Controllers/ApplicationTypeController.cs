using GraniteHouseV2.Data;
using GraniteHouseV2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GraniteHouseV2.Controllers
{
    public class ApplicationTypeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ApplicationTypeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<ApplicationType> applicationTypes = _db.ApplicationType;

            return View(applicationTypes);
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType applicationType)
        {
            _db.ApplicationType.Add(applicationType);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}

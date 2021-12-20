using GraniteHouseV2.Data;
using GraniteHouseV2_Models;
using GraniteHouseV2_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GraniteHouseV2.Controllers
{
    [Authorize(Roles = AppConstants.AdminRole)]
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

        // GET - EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var applicationTypeObj = _db.ApplicationType.Find(id);
            if(applicationTypeObj == null)
            {
                return NotFound();
            }
            return View(applicationTypeObj);
        }

        // POST - EDIT
        [HttpPost]
        public IActionResult Edit(ApplicationType applicationType)
        {
            if (ModelState.IsValid)
            {
                _db.ApplicationType.Update(applicationType);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicationType);
        }

        // GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var applicationTypeObj = _db.ApplicationType.Find(id);
            if (applicationTypeObj == null)
            {
                return NotFound();
            }
            return View(applicationTypeObj);
        }

        // POST - DELETE
        [HttpPost]
        public IActionResult DeleteConfirmed(int? applicationTypeId)
        {
            var applicationTypeObj = _db.ApplicationType.Find(applicationTypeId);
            if (applicationTypeObj == null)
            {
                return NotFound();
            }
            _db.ApplicationType.Remove(applicationTypeObj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

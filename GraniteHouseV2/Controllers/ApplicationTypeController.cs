using GraniteHouseV2_DataAccess.Repository.IRepository;
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
        private readonly IApplicationTypeRepository _applicationTypeRepository;

        public ApplicationTypeController(IApplicationTypeRepository applicationTypeRepository)
        {
            _applicationTypeRepository = applicationTypeRepository;
        }

        public IActionResult Index()
        {
            IEnumerable<ApplicationType> applicationTypes = _applicationTypeRepository.GetAll();
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
            if (ModelState.IsValid)
            {
                _applicationTypeRepository.Add(applicationType);
                _applicationTypeRepository.Save();
                TempData[AppConstants.Success] = "Application Type created successfully";
                return RedirectToAction("Index");
            }
            TempData[AppConstants.Error] = "Error while creating Application Type";
            return View(applicationType);
        }

        // GET - EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var applicationTypeObj = _applicationTypeRepository.Find(id.GetValueOrDefault());
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
                _applicationTypeRepository.Update(applicationType);
                _applicationTypeRepository.Save();
                TempData[AppConstants.Success] = "Application Type updated successfully";
                return RedirectToAction("Index");
            }
            TempData[AppConstants.Error] = "Error while updating Application Type";
            return View(applicationType);
        }

        // GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var applicationTypeObj = _applicationTypeRepository.Find(id.GetValueOrDefault());
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
            var applicationTypeObj = _applicationTypeRepository.Find(applicationTypeId.GetValueOrDefault());
            if (applicationTypeObj == null)
            {
                TempData[AppConstants.Error] = "Error getting Application Type details";
                return NotFound();
            }
            _applicationTypeRepository.Remove(applicationTypeObj);
            _applicationTypeRepository.Save();
            TempData[AppConstants.Success] = "Application Type deleted successfully";
            return RedirectToAction("Index");
        }
    }
}

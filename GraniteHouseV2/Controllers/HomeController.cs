﻿using GraniteHouseV2.Data;
using GraniteHouseV2.Models;
using GraniteHouseV2.Models.ViewModels;
using GraniteHouseV2.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GraniteHouseV2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Products = _db.Product.Include(x => x.Category).Include(x => x.ApplicationType),
                Categories = _db.Category
            };
            return View(homeVM);
        }

        public IActionResult Details(int id)
        {
            DetailsVM detailsVM = new DetailsVM()
            {
                Product = _db.Product.Include(x => x.Category).Include(x => x.ApplicationType)
                    .Where(x => x.ProductId == id).FirstOrDefault(),
                ExistsInCart = false
            };
            return View(detailsVM);
        }
        
        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart) != null 
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(AppConstants.SessionCart);
            }
            shoppingCartList.Add(new ShoppingCart { ProductId = id });
            HttpContext.Session.Set(AppConstants.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

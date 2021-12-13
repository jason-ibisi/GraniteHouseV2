﻿using GraniteHouseV2.Data;
using GraniteHouseV2.Models;
using GraniteHouseV2.Models.ViewModels;
using GraniteHouseV2.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace GraniteHouseV2.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            
            // check if session contains shopping cart session
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart) != null 
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart).ToList();
            }

            // get list of product ids in cart
            List<int> productInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> productsList = _db.Product.Where(p => productInCart.Contains(p.ProductId));

            return View(productsList);
        }

        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            // check if session contains shopping cart session
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart).ToList();
            }

            // Remove item from the session
            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(p => p.ProductId == id));

            // Set session variable again
            HttpContext.Session.Set(AppConstants.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult SubmitCartForSummary()
        {
            return RedirectToAction(nameof(ShoppingCartSummary));
        }

        public IActionResult ShoppingCartSummary()
        {
            // Get logged in user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            // var userId = User.FindFirstValue(ClaimTypes.Name);

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            // check if session contains shopping cart session
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart).ToList();
            }

            // get list of product ids in cart
            List<int> productInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> productsList = _db.Product.Where(p => productInCart.Contains(p.ProductId));

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value),
                ProductList = productsList
            };

            return View(ProductUserVM);
        }
    }
}

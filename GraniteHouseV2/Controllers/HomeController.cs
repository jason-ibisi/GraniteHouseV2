using GraniteHouseV2_DataAccess.Repository.IRepository;
using GraniteHouseV2_Models;
using GraniteHouseV2_Models.ViewModels;
using GraniteHouseV2_Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GraniteHouseV2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Products = _productRepository.GetAll(includeProperties: "Category,ApplicationType"),
                Categories = _categoryRepository.GetAll()
            };
            return View(homeVM);
        }

        public IActionResult Details(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart).Any())
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(AppConstants.SessionCart);
            }

            DetailsVM detailsVM = new DetailsVM()
            {
                Product = _productRepository.FirstOrDefault(p => p.ProductId == id, includeProperties: "Category,ApplicationType"),
                ExistsInCart = shoppingCartList.Where(x => x.ProductId == id).Any()
            };

            return View(detailsVM);
        }
        
        /// <summary>
        ///     Add product to shopping cart
        /// </summary>
        /// <param name="id">product id</param>
        /// <returns></returns>
        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id, DetailsVM detailsVM)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart) != null 
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart).Any())
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(AppConstants.SessionCart);
            }
            shoppingCartList.Add(new ShoppingCart { ProductId = id, SqFt = detailsVM.Product.TempSqFt });
            HttpContext.Session.Set(AppConstants.SessionCart, shoppingCartList);
            TempData[AppConstants.Success] = "Product added to cart successfully";
            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart).Any())
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(AppConstants.SessionCart);
            }

            var itemToRemoveFromCart = shoppingCartList.SingleOrDefault(x => x.ProductId == id);

            if (itemToRemoveFromCart != null)
            {
                shoppingCartList.Remove(itemToRemoveFromCart);
            }

            HttpContext.Session.Set(AppConstants.SessionCart, shoppingCartList);
            TempData[AppConstants.Success] = "Product removed from cart successfully";
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

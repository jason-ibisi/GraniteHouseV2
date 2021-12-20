using GraniteHouseV2.Data;
using GraniteHouseV2_Models;
using GraniteHouseV2_Models.ViewModels;
using GraniteHouseV2_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GraniteHouseV2.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
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
                ProductList = productsList.ToList()
            };

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShoppingCartSummary(ProductUserVM productUserVM)
        {
            // Prepare email template for Inquiry
            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() + "EmailInquiry.html";
            var subject = "New Inquiry";
            string htmlBody = "";
            using (StreamReader streamReader = System.IO.File.OpenText(PathToTemplate))
            {
                htmlBody = streamReader.ReadToEnd();
            }

            // build product list
            StringBuilder productListStringBuilder = new StringBuilder();
            foreach(var product in ProductUserVM.ProductList)
            {
                productListStringBuilder.Append($" - Name : {product.Name} <span style='font-size:14px;'> (ID: {product.ProductId})</span><br >");
            }

            string messageBody = string.Format(htmlBody,
                    ProductUserVM.ApplicationUser.FullName,
                    ProductUserVM.ApplicationUser.Email,
                    ProductUserVM.ApplicationUser.PhoneNumber,
                    productListStringBuilder.ToString());

            await _emailSender.SendEmailAsync(AppConstants.AdminEmail, subject, messageBody);

            return RedirectToAction(nameof(ShoppingCartInquiryConfirmation));
        }

        public IActionResult ShoppingCartInquiryConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
        }
    }
}

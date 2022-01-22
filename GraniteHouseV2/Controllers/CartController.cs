using GraniteHouseV2_DataAccess.Repository.IRepository;
using GraniteHouseV2_Models;
using GraniteHouseV2_Models.ViewModels;
using GraniteHouseV2_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
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
        private readonly IProductRepository _productRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IInquiryHeaderRepository _inquiryHeaderRepository;
        private readonly IInquiryDetailRepository _inquiryDetailRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(IProductRepository productRepository, IApplicationUserRepository applicationUserRepository, 
            IInquiryHeaderRepository inquiryHeaderRepository, IInquiryDetailRepository inquiryDetailRepository,  
            IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _productRepository = productRepository;
            _applicationUserRepository = applicationUserRepository;
            _inquiryHeaderRepository = inquiryHeaderRepository;
            _inquiryDetailRepository = inquiryDetailRepository;
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
            IEnumerable<Product> tempProductsList = _productRepository.GetAll(p => productInCart.Contains(p.ProductId));
            IList<Product> productsList = new List<Product>();

            // add sqft info to products from db
            foreach (var cartItem in shoppingCartList )
            {
                Product tempProduct = tempProductsList.FirstOrDefault(p => p.ProductId == cartItem.ProductId);
                tempProduct.TempSqFt = cartItem.SqFt;
                productsList.Add(tempProduct);
            }

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
            TempData[AppConstants.Success] = "Product removed from cart successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult SubmitCartForSummary(IEnumerable<Product> ProductList)
        {
            // re-calculate the total cost incase of quantity change without update
            IList<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Product product in ProductList)
            {
                // update the sqft 
                shoppingCartList.Add(new ShoppingCart { ProductId = product.ProductId, SqFt = product.TempSqFt });
            }

            // update the session
            HttpContext.Session.Set(AppConstants.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(ShoppingCartSummary));
        }

        public IActionResult ShoppingCartSummary()
        {
            ApplicationUser applicationUser;
            
            // handle user details to display for inquiry
            if (User.IsInRole(AppConstants.AdminRole))
            {
                int sessionInquiryId = HttpContext.Session.Get<int>(AppConstants.SessionInquiryId);
                // check if inquiry has been set
                if (sessionInquiryId != 0)
                {
                    // shopping cart has been loaded using inquiry
                    InquiryHeader inquiryHeader = _inquiryHeaderRepository.FirstOrDefault(i => i.InquiryId == sessionInquiryId);
                    
                    // intialize applicationUser using user details from inquiryHeader
                    applicationUser = new ApplicationUser()
                    {
                        FullName = inquiryHeader.FullName,
                        Email = inquiryHeader.Email,
                        PhoneNumber = inquiryHeader.PhoneNumber
                    };
                }
                else
                {
                    // shopping cart has not been loaded from inquiry i.e an admin has loaded it
                    // set applicationUser to blank so admin can fill the requester's detaiils
                    applicationUser = new ApplicationUser();
                }
            }
            else
            {
                // cart is being loaded by a normal user
                // get logged in user
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                // var userId = User.FindFirstValue(ClaimTypes.Name);

                applicationUser = _applicationUserRepository.FirstOrDefault(u => u.Id == claim.Value);
            }

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            // check if session contains shopping cart session
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppConstants.SessionCart).ToList();
            }

            // get list of product ids in cart
            List<int> productInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> productsList = _productRepository.GetAll(p => productInCart.Contains(p.ProductId));

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = applicationUser
            };

            // add sqft info to product and update productList in view model
            foreach (var prodInCart in shoppingCartList)
            {
                Product tempProduct = productsList.FirstOrDefault(p => p.ProductId == prodInCart.ProductId);
                tempProduct.TempSqFt = prodInCart.SqFt;
                ProductUserVM.ProductList.Add(tempProduct);
            }

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShoppingCartSummary(ProductUserVM productUserVM)
        {
            // Get currently logged on user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

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

            // Add inquiry header to database
            InquiryHeader inquiryHeader = new InquiryHeader()
            {
                ApplicationUserId = claim.Value,
                FullName = ProductUserVM.ApplicationUser.FullName,
                Email = ProductUserVM.ApplicationUser.Email,
                PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                InquiryDate = DateTime.Now
            };
            _inquiryHeaderRepository.Add(inquiryHeader);
            _inquiryHeaderRepository.Save();

            // Add inquiry detail to database
            foreach (var product in ProductUserVM.ProductList)
            {
                InquiryDetail inquiryDetail = new InquiryDetail()
                {
                    InquiryHeaderId = inquiryHeader.InquiryId,
                    ProductId = product.ProductId
                };
                _inquiryDetailRepository.Add(inquiryDetail);
            }
            _inquiryDetailRepository.Save();
            TempData[AppConstants.Success] = "Inquiry submitted successfully";

            return RedirectToAction(nameof(ShoppingCartInquiryConfirmation));
        }

        public IActionResult ShoppingCartInquiryConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
        }

        /// <summary>
        ///     Update the shopping cart 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> ProductList)
        {
            IList<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Product product in ProductList)
            {
                // update the sqft 
                shoppingCartList.Add(new ShoppingCart { ProductId = product.ProductId, SqFt = product.TempSqFt });
            }

            // update the session
            HttpContext.Session.Set(AppConstants.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }
    }
}

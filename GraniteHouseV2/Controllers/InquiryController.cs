using GraniteHouseV2_DataAccess.Repository.IRepository;
using GraniteHouseV2_Models;
using GraniteHouseV2_Models.ViewModels;
using GraniteHouseV2_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GraniteHouseV2.Controllers
{
    [Authorize(Roles = AppConstants.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderRepository _inquiryHeaderRepository;
        private readonly IInquiryDetailRepository _inquiryDetailRepository;
        [BindProperty]
        public InquiryVM InquiryVM { get; set; }

        public InquiryController(IInquiryHeaderRepository inquiryHeaderRepository, 
            IInquiryDetailRepository inquiryDetailRepository)
        {
            _inquiryHeaderRepository = inquiryHeaderRepository;
            _inquiryDetailRepository = inquiryDetailRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            InquiryVM = new InquiryVM()
            {
                InquiryHeader = _inquiryHeaderRepository.FirstOrDefault(i => i.InquiryId == id),
                InquiryDetail = _inquiryDetailRepository.GetAll(i => i.InquiryHeaderId == id, includeProperties: "Product")
            };

            return View(InquiryVM);
        }

        /// <summary>
        ///     Add the products from inquiry to the ShoppingCart
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            InquiryVM.InquiryDetail = _inquiryDetailRepository.GetAll(i => i.InquiryHeaderId == InquiryVM.InquiryHeader.InquiryId);

            // add to shopping cart list
            foreach (var productDetail in InquiryVM.InquiryDetail)
            {
                ShoppingCart shoppingCartItem = new ShoppingCart() { 
                    ProductId = productDetail.ProductId
                };
                shoppingCartList.Add(shoppingCartItem);
            }

            // add shopping cart to session
            HttpContext.Session.Remove(AppConstants.SessionCart);
            HttpContext.Session.Set(AppConstants.SessionCart, shoppingCartList);
            HttpContext.Session.Set(AppConstants.SessionInquiryId, InquiryVM.InquiryHeader.InquiryId);

            return RedirectToAction("Index", "Cart");
        }

        /// <summary>
        ///     Delete an Inquiry
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete()
        {
            InquiryHeader inquiryHeader = _inquiryHeaderRepository.FirstOrDefault(i => i.InquiryId == InquiryVM.InquiryHeader.InquiryId);
            IEnumerable<InquiryDetail> inquiryDetails = _inquiryDetailRepository.GetAll(i => i.InquiryHeaderId == InquiryVM.InquiryHeader.InquiryId);

            _inquiryDetailRepository.RemoveRange(inquiryDetails);
            _inquiryHeaderRepository.Remove(inquiryHeader);

            _inquiryHeaderRepository.Save();

            return RedirectToAction(nameof(Index));
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = _inquiryHeaderRepository.GetAll() });
        }
        #endregion
    }
}

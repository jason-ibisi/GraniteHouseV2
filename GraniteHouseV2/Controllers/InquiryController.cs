using GraniteHouseV2_DataAccess.Repository.IRepository;
using GraniteHouseV2_Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GraniteHouseV2.Controllers
{
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

        #region API CALLS
        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = _inquiryHeaderRepository.GetAll() });
        }
        #endregion
    }
}

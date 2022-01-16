using GraniteHouseV2_DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace GraniteHouseV2.Controllers
{
    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderRepository _inquiryHeaderRepository;
        private readonly IInquiryDetailRepository _inquiryDetailRepository;

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

        #region API CALLS
        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = _inquiryHeaderRepository.GetAll() });
        }
        #endregion
    }
}

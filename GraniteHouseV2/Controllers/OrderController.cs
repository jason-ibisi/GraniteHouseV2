using GraniteHouseV2_DataAccess.Repository.IRepository;
using GraniteHouseV2_Models.ViewModels;
using GraniteHouseV2_Utility;
using GraniteHouseV2_Utility.BrainTree;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GraniteHouseV2.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IBrainTreeGateway _brainTreeGateway;

        public OrderController(IOrderHeaderRepository orderHeaderRepository, IOrderDetailRepository orderDetailRepository,
            IBrainTreeGateway brainTreeGateway)
        {
            _orderHeaderRepository = orderHeaderRepository;
            _orderDetailRepository = orderDetailRepository;
            _brainTreeGateway = brainTreeGateway;
        }

        public IActionResult Index()
        {
            OrderListVM orderListVM = new OrderListVM()
            {
                OrderList = _orderHeaderRepository.GetAll(),
                StatusList = AppConstants.StatusList.ToList().Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { 
                    Text = i,
                    Value = i
                })
            };
            return View(orderListVM);
        }
    }
}

using Braintree;
using GraniteHouseV2_DataAccess.Repository.IRepository;
using GraniteHouseV2_Models;
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
        
        [BindProperty]
        public OrderVM orderVM { get; set; }

        public OrderController(IOrderHeaderRepository orderHeaderRepository, IOrderDetailRepository orderDetailRepository,
            IBrainTreeGateway brainTreeGateway)
        {
            _orderHeaderRepository = orderHeaderRepository;
            _orderDetailRepository = orderDetailRepository;
            _brainTreeGateway = brainTreeGateway;
        }

        public IActionResult Index(string searchName=null, string searchEmail=null, string searchPhoneNumber=null, string Status=null)
        {
            OrderListVM orderListVM = new OrderListVM()
            {
                OrderList = _orderHeaderRepository.GetAll(),
                StatusList = AppConstants.StatusList.ToList().Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { 
                    Text = i,
                    Value = i
                })
            };

            // Filter using the specified parameters
            if (!string.IsNullOrEmpty(searchName))
            {
                orderListVM.OrderList = orderListVM.OrderList.Where(u => u.FullName.ToLower().Contains(searchName.ToLower()));
            };
            
            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListVM.OrderList = orderListVM.OrderList.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower()));
            };
            
            if (!string.IsNullOrEmpty(searchPhoneNumber))
            {
                orderListVM.OrderList = orderListVM.OrderList.Where(u => u.PhoneNumber.ToLower().Contains(searchPhoneNumber.ToLower()));
            };
            
            if (!string.IsNullOrEmpty(Status) && Status != "--Order Status--")
            {
                orderListVM.OrderList = orderListVM.OrderList.Where(u => u.OrderStatus.ToLower().Contains(Status.ToLower()));
            };

            return View(orderListVM);
        }

        public IActionResult Details(int id)
        {
            orderVM = new OrderVM()
            {
                OrderHeader = _orderHeaderRepository.FirstOrDefault(u => u.OrderHeaderId == id),
                OrderDetail = _orderDetailRepository.GetAll(u => u.OrderHeaderId == id, includeProperties: "Product")
            };

            return View(orderVM);
        }

        [HttpPost]
        public IActionResult UpdateOrderDetails()
        {
            OrderHeader orderHeaderFromDb = _orderHeaderRepository.FirstOrDefault(u => u.OrderHeaderId == orderVM.OrderHeader.OrderHeaderId);
            orderHeaderFromDb.City = orderVM.OrderHeader.City;
            orderHeaderFromDb.Email = orderVM.OrderHeader.Email;
            orderHeaderFromDb.FullName = orderVM.OrderHeader.FullName;
            orderHeaderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.PostalCode = orderVM.OrderHeader.PostalCode;
            orderHeaderFromDb.State = orderVM.OrderHeader.State;
            orderHeaderFromDb.Street = orderVM.OrderHeader.Street;

            _orderHeaderRepository.Save();
            TempData[AppConstants.Success] = "Order details updated successfully";
            return RedirectToAction(nameof(Details), new { id = orderHeaderFromDb.OrderHeaderId });
        }

        [HttpPost]
        public IActionResult StartProcessingOrder()
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(u => u.OrderHeaderId == orderVM.OrderHeader.OrderHeaderId);
            orderHeader.OrderStatus = AppConstants.StatusInProcess;
            _orderHeaderRepository.Save();
            TempData[AppConstants.Success] = "Order scheduled for processing successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(u => u.OrderHeaderId == orderVM.OrderHeader.OrderHeaderId);
            orderHeader.OrderStatus = AppConstants.StatusShipped;
            orderHeader.ShippingDate = System.DateTime.Now;
            _orderHeaderRepository.Save();
            TempData[AppConstants.Success] = "Order scheduled for shipping successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult CancelOrder()
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(u => u.OrderHeaderId == orderVM.OrderHeader.OrderHeaderId);

            // Cancel the payment authorisation 
            IBraintreeGateway braintreeGateway = _brainTreeGateway.GetGateway();

            if (!string.IsNullOrEmpty(orderHeader.TransactionId))
            {
                Transaction transaction = braintreeGateway.Transaction.Find(orderHeader.TransactionId);

                // Check status of settlement to determine if refund is required
                if (transaction.Status == TransactionStatus.AUTHORIZED || transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
                {
                    // No refund is required, we just void the transaction
                    Result<Transaction> resultVoid = braintreeGateway.Transaction.Void(orderHeader.TransactionId);
                    orderHeader.OrderStatus = AppConstants.StatusCancelled;
                }
                else
                {
                    // Refund is required
                    Result<Transaction> resultRefund = braintreeGateway.Transaction.Refund(orderHeader.TransactionId);
                    orderHeader.OrderStatus = AppConstants.StatusRefunded;
                }
            }

            _orderHeaderRepository.Save();
            TempData[AppConstants.Success] = "Order cancelled successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}

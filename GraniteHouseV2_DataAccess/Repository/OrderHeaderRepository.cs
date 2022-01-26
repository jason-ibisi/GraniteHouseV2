using GraniteHouseV2_DataAccess.Repository.IRepository;
using GraniteHouseV2_Models;

namespace GraniteHouseV2_DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader orderHeader)
        {
            var orderHeaderObjFromDb = base.FirstOrDefault(p => p.OrderHeaderId == orderHeader.OrderHeaderId);

            if (orderHeaderObjFromDb != null)
            {
                orderHeaderObjFromDb.City = orderHeader.City;
                orderHeaderObjFromDb.FullName = orderHeader.FullName;
                orderHeaderObjFromDb.Email = orderHeader.Email;
                orderHeaderObjFromDb.CreatedByUserId = orderHeader.CreatedByUserId;
                orderHeaderObjFromDb.FinalOrderTotal = orderHeader.FinalOrderTotal;
                orderHeaderObjFromDb.OrderDate = orderHeader.OrderDate;
                orderHeaderObjFromDb.OrderStatus = orderHeader.OrderStatus;
                orderHeaderObjFromDb.PaymentDate = orderHeader.PaymentDate;
                orderHeaderObjFromDb.PhoneNumber = orderHeader.PhoneNumber;
                orderHeaderObjFromDb.PostalCode = orderHeader.PostalCode;
                orderHeaderObjFromDb.ShippingDate = orderHeader.ShippingDate;
                orderHeaderObjFromDb.State = orderHeader.State;
                orderHeaderObjFromDb.Street = orderHeader.Street;
                orderHeaderObjFromDb.TransactionId = orderHeader.TransactionId;
            }
        }
    }
}

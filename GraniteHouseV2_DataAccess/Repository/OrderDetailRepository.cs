using GraniteHouseV2_DataAccess.Repository.IRepository;
using GraniteHouseV2_Models;

namespace GraniteHouseV2_DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderDetail orderDetail)
        {
            var orderDetailObjFromDb = base.FirstOrDefault(p => p.OrderDetailId == orderDetail.OrderDetailId);

            if (orderDetailObjFromDb != null)
            {
                orderDetailObjFromDb.OrderHeaderId = orderDetail.OrderHeaderId;
                orderDetailObjFromDb.ProductId = orderDetail.ProductId;
                orderDetailObjFromDb.PricePerSqFt = orderDetail.PricePerSqFt;
                orderDetailObjFromDb.SqFt = orderDetail.SqFt;
            }
        }
    }
}

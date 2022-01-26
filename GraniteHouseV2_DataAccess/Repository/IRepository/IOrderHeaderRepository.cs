using GraniteHouseV2_Models;

namespace GraniteHouseV2_DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader inquiryHeader);
    }
}

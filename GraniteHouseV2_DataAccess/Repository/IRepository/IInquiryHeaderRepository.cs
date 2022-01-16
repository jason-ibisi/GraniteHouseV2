using GraniteHouseV2_Models;

namespace GraniteHouseV2_DataAccess.Repository.IRepository
{
    public interface IInquiryHeaderRepository : IRepository<InquiryHeader>
    {
        void Update(InquiryHeader inquiryHeader);
    }
}

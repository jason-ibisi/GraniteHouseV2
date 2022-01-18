using GraniteHouseV2_DataAccess.Repository.IRepository;
using GraniteHouseV2_Models;

namespace GraniteHouseV2_DataAccess.Repository
{
    public class InquiryDetailRepository : Repository<InquiryDetail>, IInquiryDetailRepository
    {
        private readonly ApplicationDbContext _db;

        public InquiryDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(InquiryDetail inquiryDetail)
        {
            var inquiryDetailObjFromDb = base.FirstOrDefault(p => p.InquiryDetailId == inquiryDetail.InquiryDetailId);

            if (inquiryDetailObjFromDb != null)
            {
                inquiryDetailObjFromDb.InquiryHeaderId = inquiryDetail.InquiryHeaderId;
                inquiryDetailObjFromDb.ProductId = inquiryDetail.ProductId;
            }
        }
    }
}

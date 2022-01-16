using GraniteHouseV2_DataAccess.Repository.IRepository;
using GraniteHouseV2_Models;
using System.Security.Claims;

namespace GraniteHouseV2_DataAccess.Repository
{
    public class InquiryHeaderRepository : Repository<InquiryHeader>, IInquiryHeaderRepository
    {
        private readonly ApplicationDbContext _db;

        public InquiryHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(InquiryHeader inquiryHeader)
        {
            var inquiryHeaderObjFromDb = base.FirstOrDefault(p => p.InquiryId == inquiryHeader.InquiryId);

            if (inquiryHeaderObjFromDb != null)
            {
                // get currently logged on user
                var claimsIdentity = (ClaimsIdentity)ClaimsPrincipal.Current.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                inquiryHeaderObjFromDb.ApplicationUserId = claim.Value;
                inquiryHeaderObjFromDb.FullName = claimsIdentity.Name;
                inquiryHeaderObjFromDb.Email = inquiryHeader.Email;
                inquiryHeaderObjFromDb.InquiryDate = inquiryHeader.InquiryDate;
                inquiryHeaderObjFromDb.InquiryId = inquiryHeader.InquiryId;
                inquiryHeaderObjFromDb.PhoneNumber = inquiryHeader.PhoneNumber;
            }
        }
    }
}

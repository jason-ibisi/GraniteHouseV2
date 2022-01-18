using GraniteHouseV2_DataAccess.Repository.IRepository;
using GraniteHouseV2_Models;

namespace GraniteHouseV2_DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}

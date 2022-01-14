using GraniteHouseV2_DataAccess.Repository.IRepository;
using GraniteHouseV2_Models;

namespace GraniteHouseV2_DataAccess.Repository
{
    public class ApplicationTypeRepository : Repository<ApplicationType>, IApplicationTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ApplicationType applicationType)
        {
            var applicationTypeFromDb = base.FirstOrDefault(a => a.ApplicationTypeId == applicationType.ApplicationTypeId);

            if (applicationTypeFromDb != null)
            {
                applicationTypeFromDb.Name = applicationType.Name;
            }
        }
    }
}

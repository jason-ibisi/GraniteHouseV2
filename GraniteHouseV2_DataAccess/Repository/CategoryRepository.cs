using GraniteHouseV2_DataAccess.Repository.IRepository;
using GraniteHouseV2_Models;

namespace GraniteHouseV2_DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category category)
        {
            var categoryObjFromDb = base.FirstOrDefault(c => c.CategoryId == category.CategoryId);

            if (categoryObjFromDb != null)
            {
                categoryObjFromDb.Name = category.Name;
                categoryObjFromDb.DisplayOrder = category.DisplayOrder;
            }
        }
    }
}

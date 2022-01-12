using GraniteHouseV2_Models;

namespace GraniteHouseV2_DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category);
    }
}

using GraniteHouseV2_DataAccess.Repository.IRepository;
using GraniteHouseV2_Models;
using GraniteHouseV2_Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace GraniteHouseV2_DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetAllDropdownList(string obj)
        {
            if (obj == AppConstants.CategoryName)
            {
                return _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CategoryId.ToString()
                });
            }

            if (obj == AppConstants.ApplicationTypeName)
            {
                return _db.ApplicationType.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.ApplicationTypeId.ToString()
                });
            }

            return null;
        }

        public void Update(Product product)
        {
            var productObjFromDb = base.FirstOrDefault(p => p.ProductId == product.ProductId);

            if (productObjFromDb != null)
            {
                productObjFromDb.ApplicationTypeId = product.ApplicationTypeId;
                productObjFromDb.CategoryId = product.CategoryId;
                productObjFromDb.Description = product.Description;
                productObjFromDb.Image = product.Image;
                productObjFromDb.Name = product.Name;
                productObjFromDb.Price = product.Price;
                productObjFromDb.ShortDesc = product.ShortDesc;
            }
        }
    }
}

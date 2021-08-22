using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GraniteHouseV2.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}

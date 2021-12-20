using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GraniteHouseV2_Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Display Order for catergory must be greater than 0")]
        public int DisplayOrder { get; set; }
    }
}

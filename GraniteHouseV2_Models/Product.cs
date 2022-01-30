using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraniteHouseV2_Models
{
    public class Product
    {
        public Product()
        {
            TempSqFt = 1;
        }

        [Key]
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        public string ShortDesc { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public double Price { get; set; }
        public string Image { get; set; }
        [Display(Name = "Category Type")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        [Display(Name = "Application Type")]
        public int ApplicationTypeId { get; set; }
        [ForeignKey("ApplicationTypeId")]
        public virtual ApplicationType ApplicationType { get; set; }

        [NotMapped]
        [Range(1, 10000, ErrorMessage = "SqFt must be greater than 0")]
        public int TempSqFt { get; set; }
    }
}

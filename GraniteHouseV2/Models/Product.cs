using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraniteHouseV2.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
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
    }
}

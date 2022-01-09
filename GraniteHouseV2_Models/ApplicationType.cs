using System.ComponentModel.DataAnnotations;

namespace GraniteHouseV2_Models
{
    public class ApplicationType
    {
        [Key]
        public int ApplicationTypeId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace GraniteHouseV2.Models
{
    public class ApplicationType
    {
        public int ApplicationTypeId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}

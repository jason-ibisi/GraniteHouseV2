using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraniteHouseV2_Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "This field is required")]
        public string FullName { get; set; }
        [NotMapped]
        public string Street { get; set; }
        [NotMapped]
        public string City { get; set; }
        [NotMapped]
        public string State { get; set; }
        [NotMapped]
        public string PostalCode { get; set; }
    }
}

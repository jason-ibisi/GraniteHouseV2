using Microsoft.AspNetCore.Identity;

namespace GraniteHouseV2_Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}

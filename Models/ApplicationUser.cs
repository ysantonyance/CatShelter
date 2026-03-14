using Microsoft.AspNetCore.Identity;

namespace CatShelter.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<CatCare> CatCares { get; set; }
        public ICollection<Adoption> Adoptions { get; set; }
    }
}

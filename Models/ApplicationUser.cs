using Microsoft.AspNetCore.Identity;

namespace CatShelter.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<CatCare> CatCares { get; set; } = new List<CatCare>();
        public ICollection<Adoption> Adoptions { get; set; } = new List<Adoption>();
    }
}

using Microsoft.AspNetCore.Identity;

namespace CatShelter.Models
{
    // модел за потребител на приложението, наследява IdentityUser
    public class ApplicationUser : IdentityUser
    {
        // колекция от грижи за котки, свързани с потребителя
        public ICollection<CatCare> CatCares { get; set; } = new List<CatCare>();
        // колекция от осиновявания, извършени от потребителя
        public ICollection<Adoption> Adoptions { get; set; } = new List<Adoption>();
    }
}

using System.ComponentModel;

namespace CatShelter.Models
{
    public class CatCare : BaseEntity
    {
        public int CatId { get; set; }
        public virtual Cat? Cat { get; set; }
        public int CareId { get; set; }
        public virtual Care? Care { get; set; }
        public bool IsSatisfied {  get; set; }
        [DefaultValue(0)]
        public decimal Price { get; set; } 
        public string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}

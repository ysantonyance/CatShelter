using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace CatShelter.Models
{
    public class CatCare : BaseEntity
    {
        [Range(1, int.MaxValue, ErrorMessage = "CatId must be greater than 0")]
        public int CatId { get; set; }
        public virtual Cat? Cat { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "CareId must be greater than 0")]
        public int CareId { get; set; }
        public virtual Care? Care { get; set; }
        public bool IsSatisfied {  get; set; }
        [DefaultValue(0)]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public decimal Price { get; set; }
        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}

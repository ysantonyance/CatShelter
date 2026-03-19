using System.ComponentModel.DataAnnotations;
namespace CatShelter.Models
{
    public class Adoption : BaseEntity
    {
        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "CatId must be greater than 0")]
        public int CatId { get; set; }
        public virtual Cat? Cat { get; set; }
        [Required(ErrorMessage = "Adoption date is required")]
        [NotDefaultDateAttribute(ErrorMessage = "AdoptionDate must be set")]
        public DateOnly AdoptionDate { get; set; }
        [Required]
        public ApplicationStatus Status {  get; set; }

        public class NotDefaultDateAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                if (value is DateOnly date)
                    return date != default;
                return false;
            }
        }
    }
}

using System.ComponentModel.DataAnnotations;
using RangeAttribute = System.ComponentModel.DataAnnotations.RangeAttribute;
namespace CatShelter.Models
{
    public class Adoption : BaseEntity
    {
        // идентификатор на потребителя, който осиновява
        [Required]
        public string UserId { get; set; }
        // навигационно свойство към потребителя
        public virtual ApplicationUser? User { get; set; }
        // идентификатор на котката, която се осиновява
        // трябва да е по-голям от 0
        [Range(1, int.MaxValue, ErrorMessage = "CatId must be greater than 0")]
        // навигационно свойство към котката
        public int CatId { get; set; }
        public virtual Cat? Cat { get; set; }
        // дата на осиновяването, задължително поле
        // не може да е стойност по подразбиране
        [Required(ErrorMessage = "Adoption date is required")]
        [NotDefaultDateAttribute(ErrorMessage = "AdoptionDate must be set")]
        public DateOnly AdoptionDate { get; set; }
        [Required]
        // статус на осиновяването (pending, approved, denied)
        public ApplicationStatus Status {  get; set; }

        // валидатор за проверка дали датата не е по подразбиране
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

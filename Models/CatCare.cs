using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;
using RangeAttribute = System.ComponentModel.DataAnnotations.RangeAttribute;

namespace CatShelter.Models
{
    // модел за грижа за котка, наследява базовия клас
    public class CatCare : BaseEntity
    {
        // идентификатор на котката, задължително поле, трябва да е > 0
        [Range(1, int.MaxValue, ErrorMessage = "CatId must be greater than 0")]
        public int CatId { get; set; }
        // навигационно свойство към котката
        public virtual Cat? Cat { get; set; }
        // идентификатор на грижата, задължително поле, трябва да е > 0
        [Range(1, int.MaxValue, ErrorMessage = "CareId must be greater than 0")]
        public int CareId { get; set; }
        // навигационно свойство към грижата
        public virtual Care? Care { get; set; }
        // флаг дали грижата е изпълнена
        public bool IsSatisfied {  get; set; }
        // цена на грижата, не може да е отрицателна, по подразбиране 0
        [DefaultValue(0)]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public decimal Price { get; set; }
        // идентификатор на потребителя, който е извършил грижата, задължително поле
        [Required]
        public string UserId { get; set; }
        // навигационно свойство към потребителя
        public virtual ApplicationUser? User { get; set; }
    }
}

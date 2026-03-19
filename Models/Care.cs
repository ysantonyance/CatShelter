using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CatShelter.Models
{
    // модел за вид грижа за котка, наследява базовия клас
    public class Care : BaseEntity
    {
        // име на грижата, задължително поле
        [Required(ErrorMessage = "CareName is required")]
        public string CareName { get; set; }
        // описание на грижата, задължително поле
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        // колекция от записи за грижи, свързани с котки
        public ICollection<CatCare> CatCares { get; set; } = new List<CatCare>();
    }
}

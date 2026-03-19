using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CatShelter.Models
{
    public class Care : BaseEntity
    {
        [Required(ErrorMessage = "CareName is required")]
        public string CareName { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        public ICollection<CatCare> CatCares { get; set; } = new List<CatCare>();
    }
}

using System.ComponentModel.DataAnnotations;
namespace CatShelter.Models
{
    public class Breed : BaseEntity
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        public ICollection<Cat> Cats { get; set; } = new List<Cat>();
    }
}

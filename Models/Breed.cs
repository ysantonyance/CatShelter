using System.ComponentModel.DataAnnotations;
namespace CatShelter.Models
{
    // модел за порода котка, наследява базовия клас
    public class Breed : BaseEntity
    {
        // име на породата, задължително поле
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        // описание на породата, задължително поле
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        // колекция от котки, които принадлежат към тази порода
        public ICollection<Cat> Cats { get; set; } = new List<Cat>();
    }
}

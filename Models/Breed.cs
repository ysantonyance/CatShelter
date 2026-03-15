namespace CatShelter.Models
{
    public class Breed : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Cat> Cats { get; set; } = new List<Cat>();
    }
}

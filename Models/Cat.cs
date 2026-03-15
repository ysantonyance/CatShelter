
namespace CatShelter.Models
{
    public class Cat : BaseEntity
    {
        public string Name { get; set; }
        public DateOnly BirthDate { get; set; }
        public decimal Kg { get; set; }
        public string Img { get; set; }
        public int BreedId { get; set; }
        public virtual Breed? Breed { get; set; }
        public bool IsAdopted { get; set; }
        public bool IsHealthy { get; set; }
        public ICollection<CatCare> CatCares { get; set; } = new List<CatCare>();
        public ICollection<Adoption> Adoptions { get; set; } = new List<Adoption>();
    }
}

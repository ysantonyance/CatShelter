using System.ComponentModel.DataAnnotations;

namespace CatShelter.Models
{
    public class Cat : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public DateOnly BirthDate { get; set; }
        [Range(0.1, double.MaxValue, ErrorMessage = "Kg must be positive")]
        public decimal Kg { get; set; }
        [Required]
        public string Img { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "BreedId must be greater than 0")]
        public int BreedId { get; set; }
        public virtual Breed? Breed { get; set; }
        public bool IsAdopted { get; set; }
        public bool IsHealthy { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Gender must be selected")]
        public Gender Gender { get; set; }
        [Required]
        public string Description { get; set; }
        public ICollection<CatCare> CatCares { get; set; } = new List<CatCare>();
        public ICollection<Adoption> Adoptions { get; set; } = new List<Adoption>();
    }
}

using System.ComponentModel.DataAnnotations;
using RangeAttribute = System.ComponentModel.DataAnnotations.RangeAttribute;

namespace CatShelter.Models
{
    // модел за котка, наследява базовия клас
    public class Cat : BaseEntity
    {
        // име на котката, задължително поле
        [Required]
        public string Name { get; set; }
        [Required]
        // дата на раждане на котката, задължително поле
        public DateOnly BirthDate { get; set; }
        // тегло на котката в килограми, трябва да е положително
        [Range(0.1, double.MaxValue, ErrorMessage = "Kg must be positive")]
        public decimal Kg { get; set; }
        [Required]
        // път до изображение на котката, задължително поле
        public string Img { get; set; }
        // идентификатор на породата, задължително поле, трябва да е > 0
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "BreedId must be greater than 0")]
        public int BreedId { get; set; }
        // навигационно свойство към породата
        public virtual Breed? Breed { get; set; }
        // флаг дали котката е осиновена
        public bool IsAdopted { get; set; }
        // флаг дали котката е здрава
        public bool IsHealthy { get; set; }
        // пол на котката, задължително поле
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Gender must be selected")]
        public Gender Gender { get; set; }
        // описание на котката, задължително поле
        [Required]
        public string Description { get; set; }
        // колекция от грижи, свързани с котката
        public ICollection<CatCare> CatCares { get; set; } = new List<CatCare>();
        // колекция от осиновявания на котката
        public ICollection<Adoption> Adoptions { get; set; } = new List<Adoption>();
    }
}

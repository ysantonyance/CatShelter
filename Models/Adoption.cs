namespace CatShelter.Models
{
    public class Adoption : BaseEntity
    {
        public string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public int CatId { get; set; }
        public virtual Cat? Cat { get; set; }
        public DateOnly AdoptionDate { get; set; }
        public ApplicationStatus Status {  get; set; }
    }
}

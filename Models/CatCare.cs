namespace CatShelter.Models
{
    public class CatCare : BaseEntity
    {
        public int CatId { get; set; }
        public virtual Cat? Cat { get; set; }
        public int CareId { get; set; }
        public virtual Care? Care { get; set; }
        public bool IsSatisfied {  get; set; }
        public decimal Price { get; set; } = 0;
        public string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}

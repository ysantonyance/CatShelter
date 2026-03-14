namespace CatShelter.Models
{
    public class Care : BaseEntity
    {
        public string CareName { get; set; }
        public string Description { get; set; }
        public ICollection<CatCare> CatCares { get; set; }
    }
}

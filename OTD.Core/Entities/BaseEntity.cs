namespace OTD.Core.Entities
{
    public class BaseEntity
    {
        public virtual DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedOn { get; set; }
        public bool DeleteFlag { get; set; } = false;
    }
}

using System.Text.Json.Serialization;

namespace OTD.Core.Entities
{
    public class BaseEntity
    {
        public virtual DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public virtual Guid? CreatedBy { get; set; }
        public virtual DateTime? ModifiedOn { get; set; }
        public virtual Guid? ModifiedBy { get; set; }
        [JsonIgnore] public virtual bool DeleteFlag { get; set; } = false;
    }
}

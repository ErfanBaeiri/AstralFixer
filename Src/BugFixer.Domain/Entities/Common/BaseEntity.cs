using System.ComponentModel.DataAnnotations;

namespace BugFixer.Domain.Entities.Common
{
    public class BaseEntity
    {
        [Key]
        public long Id { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public bool IsDelete { get; set; } = false;
    }
}

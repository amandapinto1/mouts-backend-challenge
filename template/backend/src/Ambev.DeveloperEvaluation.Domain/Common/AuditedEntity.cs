using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? DeletedBy { get; set; }

    public bool IsDeleted { get; set; } = false;

}

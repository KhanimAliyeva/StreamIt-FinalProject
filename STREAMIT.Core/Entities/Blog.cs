using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class Blog: BaseAuditableEntity
    {
        public string Title { get; set; }= string.Empty;
        public string Description { get; set; }=string.Empty;
        public string ImageUrl { get; set; }=   string.Empty;
        public string AuthorName { get; set; }= string.Empty;
    }
}

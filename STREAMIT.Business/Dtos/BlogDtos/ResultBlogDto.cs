using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.BlogDtos
{
    public class ResultBlogDto
    {
        public int Id { get; set; }
        public string Title { get; set; }= string.Empty;
        public string Description { get; set; }=string.Empty;
        public string ImageUrl { get; set; }=   string.Empty;
        public DateTime CreatedDate { get; set; }
        public string AuthorName { get; set; }= string.Empty;
    }
}

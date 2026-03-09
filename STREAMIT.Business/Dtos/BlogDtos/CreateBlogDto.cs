using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace STREAMIT.Business.Dtos.BlogDtos
{
    public class CreateBlogDto
    {
        public string Title { get; set; }= string.Empty;
        public string Description { get; set; }=string.Empty;
        public IFormFile? ImageFile { get; set; } = null;
        public string AuthorName { get; set; }= string.Empty;
    }

    public class PagedBlogViewModel
    {
        public List<STREAMIT.Business.Dtos.BlogDtos.ResultBlogDto> Blogs { get; set; } = new();

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }
}

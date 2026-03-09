using STREAMIT.Business.Dtos.BlogDtos;
using STREAMIT.Business.Dtos.GenreDtos;
using STREAMIT.Business.Dtos.LanguageDtos;
using STREAMIT.Business.Dtos.MembershipDtos;
using STREAMIT.Business.Dtos.MovieDtos;
using STREAMIT.Business.Dtos.PersonDtos;
using STREAMIT.Business.Dtos.RatingDtos;
using STREAMIT.Business.Dtos.SliderDtos;
using STREAMIT.Business.Dtos.TagDtos;
using STREAMIT.Business.Dtos.UserDtos;

namespace STREAMIT.MVC.Areas.Admin.ViewModels
{
    public class PagedRatingViewModel
    {
        public List<RatingDto> Reviews { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }

    public class PagedPersonViewModel
    {
        public List<GetPersonDto> Persons { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class PagedMovieViewModel
    {
        public List<GetMovieDto> Movies { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class PagedUserViewModel
    {
        public List<GetUserDto> Users { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class PagedGenreViewModel
    {
        public List<GetGenreDto> Genres { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class PagedLanguageViewModel
    {
        public List<GetLanguageDto> Languages { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class PagedTagViewModel
    {
        public List<GetTagDto> Tags { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class PagedMembershipViewModel
    {
        public List<GetMembershipDto> Memberships { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class PagedBlogViewModel
    {
        public List<ResultBlogDto> Blogs { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }


    public class PagedSliderViewModel
    {
        public List<SliderDto> Sliders { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
    public class PagedSlider2ViewModel
    {
        public List<Slider2Dto> Sliders { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

}

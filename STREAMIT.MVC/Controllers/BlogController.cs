using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using STREAMIT.Business.Dtos.BlogDtos;
using System.Net.Http;
using System.Net.Http.Json;

public class BlogController : Controller
{
    private readonly HttpClient _httpClient;

    public BlogController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 6)
    {
        var response = await _httpClient.GetAsync("api/blog");
        if (!response.IsSuccessStatusCode)
        {
            return View(new PagedBlogViewModel());
        }

        var json = await response.Content.ReadAsStringAsync();
        var blogs = JsonConvert.DeserializeObject<List<ResultBlogDto>>(json) ?? new List<ResultBlogDto>();

        var totalCount = blogs.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var pagedBlogs = blogs
            .OrderByDescending(x => x.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var model = new PagedBlogViewModel
        {
            Blogs = pagedBlogs,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };

        return View(model);
    }

    // Blog Detail
    public async Task<IActionResult> Detail(int id)
    {
        var blog = await _httpClient.GetFromJsonAsync<ResultBlogDto>($"api/blog/{id}");

        if (blog == null)
            return NotFound();

        return View(blog);
    }
}
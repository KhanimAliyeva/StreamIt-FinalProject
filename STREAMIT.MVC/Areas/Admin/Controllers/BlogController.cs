using Microsoft.AspNetCore.Mvc;
using STREAMIT.Business.Dtos.BlogDtos;
using System.Net.Http.Headers;

namespace STREAMIT.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly HttpClient _httpClient;

        public BlogController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var blogs = await SafeGetListFromApi<ResultBlogDto>("api/blog");

            int pageSize = 5;
            int totalCount = blogs.Count;

            var pagedBlogs = blogs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedBlogViewModel
            {
                Blogs = pagedBlogs,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View(model);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBlogDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(dto.Title ?? string.Empty), "Title");
            content.Add(new StringContent(dto.Description ?? string.Empty), "Description");
            content.Add(new StringContent(dto.AuthorName ?? string.Empty), "AuthorName");

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var streamContent = new StreamContent(dto.ImageFile.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ImageFile.ContentType);
                content.Add(streamContent, "ImageFile", dto.ImageFile.FileName);
            }

            var response = await _httpClient.PostAsync("api/blog", content);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", $"API Error {(int)response.StatusCode}: {responseText}");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            var blog = await SafeGetSingleFromApi<ResultBlogDto>($"api/blog/{id}");
            if (blog == null) return NotFound();

            var dto = new UpdateBlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Description = blog.Description,
                AuthorName = blog.AuthorName,
                ImageUrl = blog.ImageUrl
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateBlogDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(dto.Id.ToString()), "Id");
            content.Add(new StringContent(dto.Title ?? string.Empty), "Title");
            content.Add(new StringContent(dto.Description ?? string.Empty), "Description");
            content.Add(new StringContent(dto.AuthorName ?? string.Empty), "AuthorName");
            content.Add(new StringContent(dto.ImageUrl ?? string.Empty), "ImageUrl");

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var streamContent = new StreamContent(dto.ImageFile.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ImageFile.ContentType);
                content.Add(streamContent, "ImageFile", dto.ImageFile.FileName);
            }

            var response = await _httpClient.PutAsync($"api/blog/{dto.Id}", content);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", $"API Error {(int)response.StatusCode}: {responseText}");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/blog/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["DeleteError"] = "Delete failed";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<T>> SafeGetListFromApi<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode) return new List<T>();

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content)) return new List<T>();

                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(content) ?? new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }

        private async Task<T?> SafeGetSingleFromApi<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode) return default;

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content)) return default;

                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
            }
            catch
            {
                return default;
            }
        }
    }
}
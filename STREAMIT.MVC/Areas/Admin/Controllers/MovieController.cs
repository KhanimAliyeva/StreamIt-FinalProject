using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using STREAMIT.Business.Dtos;
using STREAMIT.Business.Dtos.GenreDtos;
using STREAMIT.Business.Dtos.LanguageDtos;
using STREAMIT.Business.Dtos.MembershipDtos;
using STREAMIT.Business.Dtos.MovieDtos;
using STREAMIT.Business.Dtos.PersonDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Dtos.TagDtos;
using STREAMIT.MVC.Areas.Admin.ViewModels;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace STREAMIT.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly HttpClient _httpClient;

        public MovieController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        #region INDEX
        public async Task<IActionResult> Index(int page = 1)
        {
            var movies = await SafeGetListFromApi<GetMovieDto>("api/movie");

            int pageSize = 5;
            int totalCount = movies.Count;

            var pagedMovies = movies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedMovieViewModel
            {
                Movies = pagedMovies,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View(model);
        }
        #endregion

        #region DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Movie/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await SafeReadJson<ResultDto>(response);
                    TempData["DeleteError"] = error?.Message ?? "Something went wrong...";
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["DeleteError"] = "Delete operation failed.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion

        #region CREATE
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View(new CreateMovieDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMovieDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(dto);
                return View(dto);
            }

            using var content = BuildMultipartContent(dto);

            var response = await _httpClient.PostAsync("api/Movie", content);
            var respText = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                await LoadDropdowns(dto);
                var error = await SafeReadJson<ResultDto>(response);
                ModelState.AddModelError(string.Empty, error?.Message ?? respText);
                return View(dto);
            }

            TempData["ApiResponse"] = respText;
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region UPDATE
        public async Task<IActionResult> Update(int id)
        {
            var movie = await SafeGetFromApi<GetMovieDto>($"api/Movie/{id}" );
            if (movie == null) return BadRequest();

            var allGenres = await SafeGetListFromApi<GenreDto>("Genre");
            var allTags = await SafeGetListFromApi<TagDto>("Tag");
            var allPeople = await SafeGetListFromApi<PersonDto>("Person");
            var allMemberships = await SafeGetListFromApi<MembershipDto>("Membership");
            var allLanguages = await SafeGetListFromApi<LanguageDto>("Language");

            var dto = new UpdateMovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Status = movie.Status,
                Content = movie.Content,
                YoutubeUrl = movie.YoutubeUrl,
                ReleaseDate = movie.ReleaseDate,
                Duration = movie.Duration,

                MembershipId = allMemberships.FirstOrDefault(m =>
                    string.Equals(m.Name?.Trim(), movie.MembershipName?.Trim(), StringComparison.OrdinalIgnoreCase))?.Id ?? 0,

                LanguageId = allLanguages.FirstOrDefault(l =>
                    string.Equals(l.Name?.Trim(), movie.LanguageName?.Trim(), StringComparison.OrdinalIgnoreCase))?.Id ?? 0,

                GenreIds = movie.Genres?.Select(g => allGenres.FirstOrDefault(a =>
                    string.Equals(a.Name?.Trim(), g.Name?.Trim(), StringComparison.OrdinalIgnoreCase))?.Id ?? g.Id).ToList() ?? new List<int>(),

                TagIds = movie.Tags?.Select(t => allTags.FirstOrDefault(a =>
                    string.Equals(a.Name?.Trim(), t.Name?.Trim(), StringComparison.OrdinalIgnoreCase))?.Id ?? t.Id).ToList() ?? new List<int>(),

                PersonIds = movie.People?.Select(p => p.Id).ToList() ?? new List<int>()
            };

            // expose raw movie JSON to view for debugging when options are missing
            ViewBag.RawMovieJson = Newtonsoft.Json.JsonConvert.SerializeObject(movie);

            await LoadDropdowns(dto);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateMovieDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(dto);
                return View(dto);
            }

            using var content = BuildMultipartContent(dto, true);

            var response = await _httpClient.PutAsync($"api/Movie/{dto.Id}", content);
            if (!response.IsSuccessStatusCode)
            {
                await LoadDropdowns(dto);
                var error = await SafeReadJson<ResultDto>(response);
                ModelState.AddModelError("", error?.Message ?? "Something went wrong...");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region DETAILS
        public async Task<IActionResult> Details(int id)
        {
            var movie = await SafeGetFromApi<GetMovieDto>($"api/Movie/{id}");
            if (movie == null) return BadRequest();

            movie.Genres ??= new List<GenreDto>();
            movie.Tags ??= new List<TagDto>();
            movie.People ??= new List<PersonDto>();

            return View(movie);
        }        
        #endregion

        #region HELPERS

        // Reusable helper for safe API lists
        private async Task<List<T>> SafeGetListFromApi<T>(string endpoint)
        {
            try
            {
                // ensure we call the API prefix if caller passed controller name only
                var url = endpoint.StartsWith("api/", StringComparison.OrdinalIgnoreCase) ? endpoint : $"api/{endpoint}";
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return new List<T>();

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content)) return new List<T>();

                if (content.TrimStart().StartsWith("{"))
                {
                    var j = JObject.Parse(content);
                    var dataToken = j["data"] ?? j.Properties().FirstOrDefault(p => p.Value.Type == JTokenType.Array)?.Value;

                    // support EF serialized collections that appear under $values
                    if (dataToken != null && dataToken.Type == JTokenType.Object && dataToken["$values"] != null)
                    {
                        return dataToken["$values"].ToObject<List<T>>() ?? new List<T>();
                    }

                    if (dataToken != null && dataToken.Type == JTokenType.Array)
                        return dataToken.ToObject<List<T>>() ?? new List<T>();
                }

                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(content) ?? new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }

        private async Task<T?> SafeReadJson<T>(HttpResponseMessage response)
        {
            try
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch
            {
                return default;
            }
        }

        private async Task LoadDropdowns(object? dto = null)
        {
            var allMemberships = await SafeGetListFromApi<MembershipDto>("Membership");
            var allLanguages = await SafeGetListFromApi<LanguageDto>("Language");
            var allGenres = await SafeGetListFromApi<GenreDto>("Genre");
            var allTags = await SafeGetListFromApi<TagDto>("Tag");
            var allPeople = await SafeGetListFromApi<PersonDto>("Person");

            int membershipId = 0;
            int languageId = 0;
            List<int> genreIds = new List<int>();
            List<int> tagIds = new List<int>();
            List<int> personIds = new List<int>();

            if (dto is UpdateMovieDto u)
            {
                membershipId = u.MembershipId;
                languageId = u.LanguageId;
                genreIds = u.GenreIds ?? new List<int>();
                tagIds = u.TagIds ?? new List<int>();
                personIds = u.PersonIds ?? new List<int>();
            }
            else if (dto is CreateMovieDto c)
            {
                membershipId = c.MembershipId;
                languageId = c.LanguageId;
                genreIds = c.GenreIds ?? new List<int>();
                tagIds = c.TagIds ?? new List<int>();
                personIds = c.PersonIds ?? new List<int>();
            }

            ViewBag.Memberships = new SelectList(allMemberships, "Id", "Name", membershipId);
            ViewBag.Languages = new SelectList(allLanguages, "Id", "Name", languageId);
            ViewBag.Genres = new MultiSelectList(allGenres, "Id", "Name", genreIds);
            ViewBag.Tags = new MultiSelectList(allTags, "Id", "Name", tagIds);
            ViewBag.People = new MultiSelectList(allPeople, "Id", "Name", personIds);

            // expose raw lists to view for quick debugging when items don't appear
            ViewBag.RawGenresJson = Newtonsoft.Json.JsonConvert.SerializeObject(allGenres);
            ViewBag.RawTagsJson = Newtonsoft.Json.JsonConvert.SerializeObject(allTags);
            ViewBag.RawPeopleJson = Newtonsoft.Json.JsonConvert.SerializeObject(allPeople);
        }

        private static MultipartFormDataContent BuildMultipartContent(UpdateMovieDto dto, bool isUpdate = false)
        {
            var content = new MultipartFormDataContent();

            if (isUpdate)
                content.Add(new StringContent(dto.Id.ToString()), "Id");

            content.Add(new StringContent(dto.Title ?? string.Empty), "Title");
            content.Add(new StringContent(dto.Status ?? string.Empty), "Status");
            content.Add(new StringContent(dto.Content ?? string.Empty), "Content");
            content.Add(new StringContent(dto.ReleaseDate.ToString("O")), "ReleaseDate");
            content.Add(new StringContent(dto.Duration.ToString()), "Duration");
            content.Add(new StringContent(dto.MembershipId.ToString()), "MembershipId");
            content.Add(new StringContent(dto.LanguageId.ToString()), "LanguageId");

            foreach (var genreId in dto.GenreIds ?? Enumerable.Empty<int>())
                content.Add(new StringContent(genreId.ToString()), "GenreIds");

            foreach (var tagId in dto.TagIds ?? Enumerable.Empty<int>())
                content.Add(new StringContent(tagId.ToString()), "TagIds");

            foreach (var personId in dto.PersonIds ?? Enumerable.Empty<int>())
                content.Add(new StringContent(personId.ToString()), "PersonIds");

            if (dto.Poster != null)
                AddFile(content, dto.Poster, "Poster");

            if (!string.IsNullOrWhiteSpace(dto.YoutubeUrl))
                content.Add(new StringContent(dto.YoutubeUrl), "YoutubeUrl");

            if (dto.Movie != null)
                AddFile(content, dto.Movie, "Movie");

            return content;
        }

        private static MultipartFormDataContent BuildMultipartContent(CreateMovieDto dto)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(dto.Title ?? string.Empty), "Title");
            content.Add(new StringContent(dto.Status ?? string.Empty), "Status");
            content.Add(new StringContent(dto.Content ?? string.Empty), "Content");
            content.Add(new StringContent(dto.ReleaseDate.ToString("O")), "ReleaseDate");
            content.Add(new StringContent(dto.Duration.ToString()), "Duration");
            content.Add(new StringContent(dto.MembershipId.ToString()), "MembershipId");
            content.Add(new StringContent(dto.LanguageId.ToString()), "LanguageId");

            foreach (var genreId in dto.GenreIds ?? Enumerable.Empty<int>())
                content.Add(new StringContent(genreId.ToString()), "GenreIds");

            foreach (var tagId in dto.TagIds ?? Enumerable.Empty<int>())
                content.Add(new StringContent(tagId.ToString()), "TagIds");

            foreach (var personId in dto.PersonIds ?? Enumerable.Empty<int>())
                content.Add(new StringContent(personId.ToString()), "PersonIds");

            if (dto.Poster != null)
                AddFile(content, dto.Poster, "Poster");

            if (!string.IsNullOrWhiteSpace(dto.YoutubeUrl))
                content.Add(new StringContent(dto.YoutubeUrl), "YoutubeUrl");

            if (dto.Movie != null)
                AddFile(content, dto.Movie, "Movie");

            return content;
        }

        private static void AddFile(MultipartFormDataContent content, IFormFile file, string name)
        {
            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
            content.Add(streamContent, name, file.FileName);
        }

        private async Task<T?> SafeGetFromApi<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode) return default;

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content)) return default;

                // 1️⃣ Try ResultDto<T>
                try
                {
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultDto<T>>(content);
                    if (result != null && result.Data != null)
                        return result.Data;
                }
                catch { }

                // 2️⃣ raw object
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
            }
            catch
            {
                return default;
            }
        }

        #endregion
    }
}
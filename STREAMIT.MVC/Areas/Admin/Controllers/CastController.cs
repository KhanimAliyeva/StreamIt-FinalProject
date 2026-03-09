using Microsoft.AspNetCore.Mvc;
using STREAMIT.Business.Dtos.PersonDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using STREAMIT.MVC.Areas.Admin.ViewModels;

namespace STREAMIT.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CastController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CastController> _logger;
        public CastController(IHttpClientFactory httpClientFactory, ILogger<CastController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _logger = logger;
        }

        // ================== INDEX ==================
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Person");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.DebugStatusCode = (int)response.StatusCode;
                    ViewBag.RawResponse = content;
                    return View(new PagedPersonViewModel());
                }

                List<GetPersonDto>? persons = null;

                try
                {
                    if (!string.IsNullOrWhiteSpace(content) && content.TrimStart().StartsWith("{"))
                    {
                        var j = JObject.Parse(content);
                        JToken? dataToken = null;

                        if (j.TryGetValue("data", StringComparison.OrdinalIgnoreCase, out var d))
                            dataToken = d;
                        else if (j.Properties().Any(p => p.Value.Type == JTokenType.Array))
                            dataToken = j.Properties().First(p => p.Value.Type == JTokenType.Array).Value;

                        if (dataToken != null && dataToken.Type == JTokenType.Array)
                            persons = dataToken.ToObject<List<GetPersonDto>>();
                    }
                    else
                    {
                        persons = JsonConvert.DeserializeObject<List<GetPersonDto>>(content);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse persons response");
                }

                persons ??= new List<GetPersonDto>();

                int pageSize = 5;
                int totalCount = persons.Count;

                var pagedPersons = persons
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var model = new PagedPersonViewModel
                {
                    Persons = pagedPersons,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.RawResponse = ex.Message;
                return View(new PagedPersonViewModel());
            }
        }
        // ================== CREATE ==================
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePersonDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(dto.Name), "Name");
            content.Add(new StringContent(dto.Gender), "Gender");
            content.Add(new StringContent(dto.Born.ToString("o")), "Born");
            content.Add(new StringContent(dto.Popularity.ToString()), "Popularity");
            content.Add(new StringContent(dto.Biography), "Biography");
            content.Add(new StringContent(dto.Role), "Role");
            if (dto.Died.HasValue)
                content.Add(new StringContent(dto.Died.Value.ToString("o")), "Died");

            // IMAGE
            if (dto.Image != null)
            {
                var fileContent = new StreamContent(dto.Image.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.Image.ContentType);
                content.Add(fileContent, "Image", dto.Image.FileName);
            }

            var response = await _httpClient.PostAsync("https://localhost:7108/api/Person", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.DebugStatusCode = (int)response.StatusCode;
                ViewBag.RawResponse = content;
                ModelState.AddModelError("", $"API Error {(int)response.StatusCode}: {content}");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        // ================== DETAILS ==================
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7108/api/Person/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.DebugStatusCode = (int)response.StatusCode;
                    ViewBag.RawResponse = content;
                    return View(null as GetPersonDto);
                }

                // Try to parse wrapped result first
                GetPersonDto? person = null;
                try
                {
                    var wrapper = System.Text.Json.JsonSerializer.Deserialize<ResultDto<GetPersonDto>>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    person = wrapper?.Data;
                }
                catch { }

                if (person == null)
                {
                    try
                    {
                        person = System.Text.Json.JsonSerializer.Deserialize<GetPersonDto>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch { }
                }

                if (person == null)
                {
                    ViewBag.RawResponse = content;
                    return View(null as GetPersonDto);
                }

                return View(person);
            }
            catch (Exception ex)
            {
                ViewBag.RawResponse = ex.Message;
                return View(null as GetPersonDto);
            }
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Try to get the person from the API. If the API returns a non-success status
            // show the edit view with a helpful error instead of returning NotFound (404)
            // which gives less debugging information in the browser.
            var response = await _httpClient.GetAsync($"https://localhost:7108/api/Person/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.DebugStatusCode = (int)response.StatusCode;
                ViewBag.RawResponse = content;
                ModelState.AddModelError("", $"API Error {(int)response.StatusCode}: {content}");

                // Return the Edit view with a minimal DTO so the page renders and shows the error.
                return View(new UpdatePersonDto { Id = id });
            }

            // Parse potential wrapper or raw object like elsewhere in this controller
            GetPersonDto? person = null;
            try
            {
                var wrapper = System.Text.Json.JsonSerializer.Deserialize<ResultDto<GetPersonDto>>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                person = wrapper?.Data;
            }
            catch { }

            if (person == null)
            {
                try
                {
                    person = System.Text.Json.JsonSerializer.Deserialize<GetPersonDto>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch { }
            }

            if (person == null)
            {
                // If parsing failed, show debug info and an empty DTO
                ViewBag.RawResponse = content;
                ModelState.AddModelError("", "Could not parse API response.");
                return View(new UpdatePersonDto { Id = id });
            }

            var dto = new UpdatePersonDto
            {
                Id = person.Id,
                Name = person.Name,
                Gender = person.Gender,
                Born = person.Born,
                Died = person.Died,
                Popularity = person.Popularity,
                Biography = person.Biography,
                Role = person.Role,
                ExistingImageUrl = person.ImageUrl
            };

            return View(dto);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdatePersonDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(dto.Id.ToString()), "Id");
            content.Add(new StringContent(dto.Name), "Name");
            content.Add(new StringContent(dto.Gender), "Gender");
            content.Add(new StringContent(dto.Born.ToString("o")), "Born");
            content.Add(new StringContent(dto.Popularity.ToString()), "Popularity");
            content.Add(new StringContent(dto.Biography), "Biography");
            content.Add(new StringContent(dto.Role), "Role");


            if (dto.Died.HasValue)
                content.Add(new StringContent(dto.Died.Value.ToString("o")), "Died");

            if (dto.Image != null)
            {
                var fileContent = new StreamContent(dto.Image.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(dto.Image.ContentType);
                content.Add(fileContent, "Image", dto.Image.FileName);
            }

            var response = await _httpClient.PutAsync("https://localhost:7108/api/Person", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", error);
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        // ================== DELETE ==================
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7108/api/Person/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(error);
            }

            return RedirectToAction(nameof(Index));
        }

        // ================== HELPERS ==================
        private async Task<T?> SafeGetFromJson<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7108/api/{endpoint}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch
            {
                return default;
            }
        }
    }
}
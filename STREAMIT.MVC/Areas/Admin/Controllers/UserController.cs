using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using STREAMIT.Business.Dtos.UserDtos;
using STREAMIT.MVC.Areas.Admin.ViewModels;

namespace STREAMIT.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        #region INDEX
        public async Task<IActionResult> Index(int page = 1)
        {
            var users = await SafeGetListFromApi<GetUserDto>("api/User");

            int pageSize = 5;
            int totalCount = users.Count;

            var pagedUsers = users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedUserViewModel
            {
                Users = pagedUsers,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View(model);
        }
        #endregion

        #region DETAILS
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var userList = await SafeGetListFromApi<GetUserDto>($"api/User/{id}");
            var user = userList.FirstOrDefault();

            if (user == null)
                return NotFound();

            return View(user);
        }
        #endregion

        #region HELPERS
        private async Task<List<T>> SafeGetListFromApi<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode) return new List<T>();

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content)) return new List<T>();

                if (content.TrimStart().StartsWith("{"))
                {
                    var j = JObject.Parse(content);
                    JToken? dataToken = j["data"];

                    if (dataToken != null && dataToken.Type == JTokenType.Object)
                        return new List<T> { dataToken.ToObject<T>()! };

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
        #endregion
    }
}
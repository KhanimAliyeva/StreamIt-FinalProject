using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STREAMIT.Business.Dtos.MembershipDtos;
using STREAMIT.Core.Entities;
using System.Security.Claims;
using System.Text;

namespace STREAMIT.MVC.Controllers
{
    public class MembershipController : Controller
    {
        private readonly HttpClient _httpClient;

        public MembershipController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var json = await _httpClient.GetStringAsync("api/membership");

            using var document = System.Text.Json.JsonDocument.Parse(json);

            List<GetMembershipDto>? memberships = null;
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            if (document.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                memberships = System.Text.Json.JsonSerializer.Deserialize<List<GetMembershipDto>>(
                    document.RootElement.GetRawText(), options);
            }
            else if (document.RootElement.ValueKind == System.Text.Json.JsonValueKind.Object)
            {
                if (document.RootElement.TryGetProperty("data", out var dataElement))
                {
                    if (dataElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        memberships = System.Text.Json.JsonSerializer.Deserialize<List<GetMembershipDto>>(
                            dataElement.GetRawText(), options);
                    }
                    else if (dataElement.ValueKind == System.Text.Json.JsonValueKind.Object &&
                             dataElement.TryGetProperty("$values", out var valuesElement))
                    {
                        memberships = System.Text.Json.JsonSerializer.Deserialize<List<GetMembershipDto>>(
                            valuesElement.GetRawText(), options);
                    }
                }
            }

            memberships ??= new List<GetMembershipDto>();
            return View(memberships);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout(int membershipId)
        {
            var response = await _httpClient.GetAsync($"api/membership/{membershipId}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(json);

            var membership = jObject["data"]?.ToObject<Membership>();
            if (membership == null)
                return NotFound();

            var vm = new MembershipCheckoutVM
            {
                Id = membership.Id,
                Name = membership.Name,
                Price = membership.Price,
                Currency = "AZN",
                Membership = membership
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Buy(int membershipId)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(email))
                return Content("Email is null");

            var response = await _httpClient.PostAsync(
                $"api/payment/create?membershipId={membershipId}&email={Uri.EscapeDataString(email)}", null);

            var text = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return Content($"API ERROR: {(int)response.StatusCode} - {text}");

            var result = JsonConvert.DeserializeObject<dynamic>(text);

            if (result == null || result.checkoutUrl == null)
                return Content("Unexpected response: " + text);

            string checkoutUrl = (string)result.checkoutUrl;
            return Redirect(checkoutUrl);
        }

        public async Task<IActionResult> RedirectPayment(int ID, string? STATUS = null)
        {
            string status = STATUS ?? string.Empty;

            if (string.IsNullOrWhiteSpace(status))
            {
                var statusResponse = await _httpClient.GetAsync($"api/payment/status/{ID}");
                if (statusResponse.IsSuccessStatusCode)
                {
                    var statusJson = await statusResponse.Content.ReadAsStringAsync();
                    var statusObj = JObject.Parse(statusJson);
                    status = statusObj["status"]?.ToString() ?? string.Empty;
                }
            }

            var callbackModel = new KapitalBankCallbackModel
            {
                OrderId = ID.ToString(),
                Status = status
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(callbackModel),
                Encoding.UTF8,
                "application/json");

            await _httpClient.PostAsync("api/payment/callback", content);

            if (string.Equals(status, "FullyPaid", StringComparison.OrdinalIgnoreCase))
                TempData["SuccessMessage"] = "Order successfully completed";
            else
                TempData["ErrorMessage"] = "Payment not completed";

            return RedirectToAction("Index", "Home");
        }
    }
}
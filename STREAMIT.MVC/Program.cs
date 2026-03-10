using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using STREAMIT.Business.ServiceRegistrations;
using STREAMIT.Business.Services.Abstractions;
using STREAMIT.Business.Services.Implementations;
using STREAMIT.MVC.Handlers;
using STREAMIT.MVC.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<AuthHeaderHandler>();

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7108/");
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
})
.AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register payment service (implementation uses HttpClient)
builder.Services.AddHttpClient<IPaymentService, KapitalBankPaymentService>();

// JWT section
var jwtSection = builder.Configuration.GetSection("JWTOptions");

// ✅ Cookie + JWT birlikdə
builder.Services
  .AddAuthentication(options =>
  {
      options.DefaultAuthenticateScheme = "Smart";
      options.DefaultChallengeScheme = "Smart";
  })
  .AddPolicyScheme("Smart", "JWT or Cookie", options =>
  {
      options.ForwardDefaultSelector = context =>
      {
          // Authorization header varsa -> JWT
          var auth = context.Request.Headers["Authorization"].ToString();
          if (!string.IsNullOrWhiteSpace(auth) &&
              auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
              return JwtBearerDefaults.AuthenticationScheme;

          // SignalR token querystring ilə gələrsə -> JWT (hub route-nu özünə görə düzəlt)
          if (context.Request.Path.StartsWithSegments("/hubs/community") &&
              context.Request.Query.ContainsKey("access_token"))
              return JwtBearerDefaults.AuthenticationScheme;

          // Yoxsa Cookie
          return CookieAuthenticationDefaults.AuthenticationScheme;
      };
  })
  .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
  {
      options.LoginPath = "/Account/Login";
      options.AccessDeniedPath = "/Account/Login";

      // Lokalhost-da bəzən SameSite None problem yaradır, amma https-də işləyir:
      options.Cookie.SameSite = SameSiteMode.Lax;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
  })
  .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
  {
      var secret = jwtSection["SecretKey"] ?? "";

      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidIssuer = jwtSection["Issuer"],
          ValidateAudience = true,
          ValidAudience = jwtSection["Audience"],
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
          ValidateLifetime = true,
          ClockSkew = TimeSpan.Zero
      };

      // ✅ SignalR üçün access_token querystring
      options.Events = new JwtBearerEvents
      {
          OnMessageReceived = context =>
          {
              var accessToken = context.Request.Query["access_token"];
              var path = context.HttpContext.Request.Path;

              if (!string.IsNullOrEmpty(accessToken) &&
                  path.StartsWithSegments("/hubs/community"))
              {
                  context.Token = accessToken;
              }
              return Task.CompletedTask;
          }
      };
  });

builder.Services.AddAuthorization();

// Əgər Hub MVC-dədirsə bunu da aç:
// builder.Services.AddSignalR();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Route-lar
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Əgər Hub MVC-dədirsə bunu da aç:
// app.MapHub<CommunityHub>("/hubs/community");

app.Run();
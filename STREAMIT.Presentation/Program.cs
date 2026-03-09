using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.ServiceRegistrations;
using STREAMIT.DataAccess.Abstractions;
using STREAMIT.DataAccess.ServiceRegistrations;
using STREAMIT.Presentation.Middlewares;
using System.Text;

// ⚠️ Hub namespace-ni özündə necədirsə elə yaz
// using STREAMIT.Presentation.Hubs;

namespace STREAMIT.Presentation;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // --------------------------------------
        // 1) Controllers & Model Validation
        // --------------------------------------
        builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(x => x.Value!.Errors.Count > 0)
                    .Select(x => new
                    {
                        Errors = x.Value!.Errors.Select(e => e.ErrorMessage)
                    });

                ResultDto resultDto = new()
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = string.Join(", ", errors.SelectMany(x => x.Errors))
                };

                return new BadRequestObjectResult(resultDto);
            };
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.MaxDepth = 64;
        });

        // --------------------------------------
        // 2) Authentication (Cookie + JWT)
        // --------------------------------------
        var jwtSection = builder.Configuration.GetSection("JWTOptions");
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
                  // 1) Authorization header varsa -> JWT
                  var auth = context.Request.Headers["Authorization"].ToString();
                  if (!string.IsNullOrWhiteSpace(auth) && auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                      return JwtBearerDefaults.AuthenticationScheme;

                  // 2) Hub üçün querystring access_token varsa -> JWT
                  if (context.Request.Path.StartsWithSegments("/hubs/community") &&
                      context.Request.Query.ContainsKey("access_token"))
                      return JwtBearerDefaults.AuthenticationScheme;

                  // 3) Yoxsa Cookie
                  return CookieAuthenticationDefaults.AuthenticationScheme;
              };
          })
          .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
          {
              options.LoginPath = "/Account/Login";
              options.AccessDeniedPath = "/Account/Login";
          })
          .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
          {
              var secret = jwtSection["SecretKey"] ?? string.Empty;

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

              options.Events = new JwtBearerEvents
              {
                  OnMessageReceived = context =>
                  {
                      // SignalR üçün querystring token
                      var accessToken = context.Request.Query["access_token"];
                      var path = context.HttpContext.Request.Path;

                      if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/community"))
                      {
                          context.Token = accessToken;
                          return Task.CompletedTask;
                      }

                      // İstəsən REST üçün cookie-dəki JWT-ni də oxuya bilər (şərt deyil, səndə Authorization gedir)
                      var cookieToken = context.Request.Cookies["AccessToken"];
                      if (!string.IsNullOrWhiteSpace(cookieToken) && cookieToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                          cookieToken = cookieToken.Substring(7).Trim();

                      // bu hissəni REST üçün aktiv etmək istəsən:
                      // if (!string.IsNullOrWhiteSpace(cookieToken)) context.Token = cookieToken;

                      return Task.CompletedTask;
                  },
                  OnChallenge = context =>
                  {
                      // API çağırışlarında redirect etməsin
                      context.HandleResponse();
                      context.Response.StatusCode = 401;
                      return Task.CompletedTask;
                  }
              };
          });

        builder.Services.AddAuthorization();
        // --------------------------------------
        // 3) Swagger / OpenAPI
        // --------------------------------------
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddOpenApi();

        // --------------------------------------
        // 4) SignalR
        // --------------------------------------
        builder.Services.AddSignalR();

        // --------------------------------------
        // 5) CORS (MVC -> Presentation)
        // --------------------------------------
        builder.Services.AddCors(o =>
        {
            var mvcOrigin = builder.Configuration["Mvc:BaseUrl"]?.TrimEnd('/');

            o.AddPolicy("MyPolicy", policy =>
            {
                if (!string.IsNullOrWhiteSpace(mvcOrigin))
                {
                    // ✅ credentials üçün WithOrigins lazımdır
                    policy.WithOrigins(mvcOrigin)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                }
                else
                {
                    // ⚠️ credentials ilə AllowAnyOrigin olmaz
                    // mvcOrigin verməsən, ən azından API test üçün açırıq (credentials YOX)
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                }
            });
        });

        // --------------------------------------
        // 6) HttpClient (səndə lazım idi)
        // --------------------------------------
        builder.Services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7108/");
        });

        // --------------------------------------
        // 7) Business & Data
        // --------------------------------------
        builder.Services.AddDataAccessServices(builder.Configuration);
        builder.Services.AddBusinessServices(builder.Configuration);

        // --------------------------------------
        // Build
        // --------------------------------------
        var app = builder.Build();

        // Database initializer
        using (var scope = app.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<IContextInitalizer>();
            await initializer.InitDatabaseAsync();
        }

        // --------------------------------------
        // Middleware pipeline (order is important)
        // --------------------------------------
        app.UseMiddleware<GlobalExceptionHandler>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors("MyPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // ✅ Hub burada olmalıdır + CORS tətbiqi
        app.MapHub<CommunityHub>("/hubs/community")
           .RequireCors("MyPolicy");

        await app.RunAsync();
    }
}
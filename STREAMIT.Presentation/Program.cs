using Microsoft.AspNetCore.Mvc;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.ServiceRegistrations;
using STREAMIT.DataAccess.Abstractions;
using STREAMIT.DataAccess.ServiceRegistrations;
using STREAMIT.Presentation.Middlewares;

namespace STREAMIT.Presentation;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

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
        });

        builder.Services.AddOpenApi();

        builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
        {
            //builder.WithOrigins("http://127.0.0.1:5500/", "http://127.0.0.1:5501/")
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        }));




        //builder.Services.AddDbContext<AppDbContext>

        builder.Services.AddDataAccessServices(builder.Configuration);
        builder.Services.AddBusinessServices(builder.Configuration);


        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7108/"); // API portu
        });
        var app = builder.Build();

        var scope = app.Services.CreateScope();
        var initalizer = scope.ServiceProvider.GetRequiredService<IContextInitalizer>();

        await initalizer.InitDatabaseAsync();

        app.UseMiddleware<GlobalExceptionHandler>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(); // Enables middleware to serve generated Swagger as a JSON endpoint
            app.UseSwaggerUI(); // Enables middleware to serve swagger-ui (HTML, JS, CSS, etc.)
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseCors("MyPolicy");


        app.MapControllers();

        await app.RunAsync();
    }
}
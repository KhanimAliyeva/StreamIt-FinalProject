using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Abstractions;
using STREAMIT.DataAccess.ContextInitializers;
using STREAMIT.DataAccess.Contexts;
using STREAMIT.DataAccess.Repositories.Abstractions;
using STREAMIT.DataAccess.Repositories.Abstractions.Generic;
using STREAMIT.DataAccess.Repositories.Implementations;
using STREAMIT.DataAccess.Repositories.Implementations.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.ServiceRegistrations
{
    public static class DataAccessServiceRegistration
    {
        public static void AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IContextInitalizer, DbContextInitalizer>();

            // register interceptor first so DbContext can resolve it
            services.AddScoped<BaseAuditableInterceptor>();

            // 1. Register DbContext (uses BaseAuditableInterceptor)
            services.AddDbContext<AppDbContext>((sp, opt) =>
            {
                var interceptor = sp.GetRequiredService<BaseAuditableInterceptor>();
                opt.UseSqlServer(configuration.GetConnectionString("Default"));
                opt.AddInterceptors(interceptor);
            });

            // 2. Register Identity
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<BaseAuditableInterceptor>();
            // Generic repository (çox vacibdir əgər istifadə edirsənsə)
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Custom repositories
            services.AddScoped<IMovieRepository, MovieRepository>();
            services.AddScoped<ITvShowRepository, TvShowRepository>();
            services.AddScoped<IGenreRepository, GenreRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<IMembershipRepository, MembershipRepository>();
        }
    }

}

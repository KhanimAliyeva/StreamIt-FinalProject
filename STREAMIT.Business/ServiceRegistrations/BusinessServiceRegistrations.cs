using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using STREAMIT.Business.Dtos.TokenDtos;
using STREAMIT.Business.Services.Abstractions;
using STREAMIT.Business.Services.Implementations;
using STREAMIT.Business.Validators;
using STREAMIT.Business.Validators.UserValidators;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.ServiceRegistrations
{
    public static class BusinessServiceRegistrations
    {
        public static void AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<CreateMovieDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateMovieDtoValidator>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<IGenreService, GenreService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IMembershipService,MembershipService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IMembershipService, MembershipService>();
            services.AddScoped<IGenreService, GenreService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPaymentService, KapitalBankPaymentService>();
            services.AddScoped<IAiChatService, GroqAiChatService>();


            // services.AddScoped<IEmailService, FakeEmailService>();




            services.AddAutoMapper(_ => { }, typeof(BusinessServiceRegistrations).Assembly);
           
            JWTOptionsDto options = configuration.GetSection("JWTOptions").Get<JWTOptionsDto>() ?? new();
        }
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using STREAMIT.Core.Entities;
using STREAMIT.Core.Enums;
using STREAMIT.DataAccess.Abstractions;
using STREAMIT.DataAccess.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.ContextInitializers
{
    internal class DbContextInitalizer : IContextInitalizer
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly string _adminPassword;
        private readonly string _adminEmail;
        private readonly string _adminFullname;
        private readonly string _adminUsername;
        public DbContextInitalizer(AppDbContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;


            var section = _configuration.GetSection("AdminSettings");

            _adminPassword = section.GetValue<string>("Password") ?? "";
            _adminEmail = section.GetValue<string>("Email") ?? "";
            _adminFullname = section.GetValue<string>("Fullname") ?? "";
            _adminUsername = section.GetValue<string>("UserName") ?? "";
        }


        public async Task InitDatabaseAsync()
        {


            await _context.Database.MigrateAsync();
            await CreateRolesAsync();
            await CreateAdminAsync();
        }

        private async Task CreateAdminAsync()
        {
            AppUser adminUser = new()
            {
                Fullname = _adminFullname,
                Email = _adminEmail,
                UserName = _adminUsername
                //GenderId=StaticData.MaleGender.Id
            };


            var result = await _userManager.CreateAsync(adminUser, _adminPassword);


            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, IdentityRoles.Admin.ToString());
            }
        }
        private async Task CreateRolesAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new AppRole
                {
                    Name = "Admin"
                });
            }

            if (!await _roleManager.RoleExistsAsync("Member"))
            {
                await _roleManager.CreateAsync(new AppRole
                {
                    Name = "Member"
                });
            }
        }


    }
}

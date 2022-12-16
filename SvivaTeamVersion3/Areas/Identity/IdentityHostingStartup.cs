using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SvivaTeamVersion3.Areas.Identity.Data;
using SvivaTeamVersion3.Data;

[assembly: HostingStartup(typeof(SvivaTeamVersion3.Areas.Identity.IdentityHostingStartup))]
namespace SvivaTeamVersion3.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AuthDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("AuthDbContextConnection")));

                services.AddDefaultIdentity<ApplicationUser>(options => {
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;

                    options.SignIn.RequireConfirmedAccount = true;
                }).AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AuthDbContext>();
            });


        }
    }
}
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using SvivaTeamVersion3.Services;
using Microsoft.AspNetCore.Identity;
using SvivaTeamVersion3.Areas.Identity.Data;
using SvivaTeamVersion3.Data;
using SvivaTeamVersion3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace SvivaTeamVersion3
{
    public class Startup
    {
        public string ClientID { get; }
        public string ClientSecret { get; }
        public string AppID { get; set; }
        public string AppSecret{ get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            this.ClientID = Properties.Resources.ClientID;
            this.ClientSecret = Properties.Resources.ClientSecret;
            this.AppID = Properties.Resources.AppID;
            this.AppSecret = Properties.Resources.AppSecret;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();

            //require using Microsoft.AspNetCore.Identity.UI.Services
            //using WebPWRecover.Services
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);


            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = this.ClientID;
                    options.ClientSecret = this.ClientSecret;
                }).AddFacebook(options =>
                {
                    options.AppId = this.AppID;
                    options.AppSecret = this.AppSecret;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                /*
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                */
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}

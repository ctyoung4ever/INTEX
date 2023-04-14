using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using INTEX.Data;
using INTEX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INTEX
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<mummyContext>(options =>
            {
                options.UseNpgsql(Configuration["ConnectionStrings:PostgresConnection"]);
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
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

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var roles = new[] { "Admin", "Authorized", "Public" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));

                }
            }


            //Admin User authorization RBAC
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                string email = "admin@admin.com";
                string password = "Admin123*";



                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new IdentityUser();
                    user.UserName = email;
                    user.Email = email;

                    await userManager.CreateAsync(user, password);

                    await userManager.AddToRoleAsync(user, "Admin");

                }
            }

            //Authorized User authorization RBAC
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                string email1 = "authorized@authorized.com";
                string password1 = "Authorized123*";


                if (await userManager.FindByEmailAsync(email1) == null)
                {
                    var user = new IdentityUser();
                    user.UserName = email1;
                    user.Email = email1;

                    await userManager.CreateAsync(user, password1);

                    await userManager.AddToRoleAsync(user, "Authorized");

                }

            }


            //Public User authorization RBAC
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                string email2 = "public@public.com";
                string password2 = "Public123*";


                if (await userManager.FindByEmailAsync(email2) == null)
                {
                    var user = new IdentityUser();
                    user.UserName = email2;
                    user.Email = email2;

                    await userManager.CreateAsync(user, password2);

                    await userManager.AddToRoleAsync(user, "Public");

                }

            }
        }
    }
}






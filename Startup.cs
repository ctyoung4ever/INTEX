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
using Microsoft.ML.OnnxRuntime;
using INTEX.ModelsTest;

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

            services.AddDbContext<ApplicationDbContextMain>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection")));
                services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContextMain>();

            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 12;
                options.Password.RequiredUniqueChars = 3;
            });


            services.AddControllersWithViews();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;

                //options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

            services.AddRazorPages();
            //////////////////////////
            services.AddSingleton<InferenceSession>
                (
                    new InferenceSession("wwwroot/model.onnx")
                );
            services.AddCors();
            ///////////////////////////
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //We unforntunatly could not get the csp header to work with the onix file however this is our attempt at creating one.  
            //app.Use(async (context, next) =>
             //{
             //  context.Response.Headers.Add("Content-Security-Policy",
             //            "default-src 'self'; " +
             //            "script-src 'self' https://code.jquery.com/jquery-3.4.1.min.js https://cdn.jsdelivr.net/npm/bootstrap@5.0.0/dist/js/bootstrap.bundle.min.js https://intex.byumummy.net/score " +
             //            "'unsafe-inline'; " +
             //            "style-src 'self' https://fonts.googleapis.com/css2?family=Open+Sans:wght@400 'unsafe-inline'; " +
             //            "style-src-elem 'self' https://fonts.googleapis.com/css2?family=Open+Sans:wght@400 https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.10.0/css/all.min.css https://cdn.jsdelivr.net/npm/bootstrap-icons@1.4.1/font/bootstrap-icons.css 'unsafe-inline'; " +
             //            "font-src 'self' https://fonts.gstatic.com https://fonts.googleapis.com https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.10.0/css/all.min.css https://cdn.jsdelivr.net/npm/bootstrap-icons@1.4.1/font/bootstrap-icons.css; " +
             //            "img-src 'self' https://images.squarespace-cdn.com/content/v1/56c13cc00442627a08632989/1567266169407-1ES9X6XT1S0JC675R67B/kingtut?format=1500w https://www.dropbox.com/s/ssgbs68pb3gjulr/download-2.png?dl=1 https://www.dropbox.com/s/f7o7ottojoxbxmr/download.png?dl=1 https://www.dropbox.com/s/z2ylc2avsfm7r8j/BurialDepthW%20%281%29.png?dl=1 https://www.dropbox.com/s/ceiunyfci5oejbr/NumClusters.png?dl=1 ; " +
             //            "frame-src 'self' https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d27832.20257320994!2d30.824776878469567!3d29.310932879397033!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x1459792fa8bf0013%3A0xa698b3d528236f63!2sFaiyum%2C%20Qesm%20Al%20Fayoum%2C%20Faiyum%2C%20Faiyum%20Governorate!5e0!3m2!1sen!2seg!4v1617013148790!5m2!1sen!2seg; "
             //           );
             //  await next();
             //});
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
                app.UseHttpsRedirection();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy();

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

            //////////////
            app.UseCors(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            //////////////

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
                string password = "Adminpassword1*";



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
                string password1 = "Authorizedpassword1*";


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
                string password2 = "Publicpassword1*";


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






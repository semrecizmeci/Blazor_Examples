using BlazorApp2.Server.Data;
using BlazorApp2.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorApp2.Server
{
    public class Startup
    {
        private void CreateUser(IServiceProvider serviceProvider)
        {
            var userManager=serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            if (userManager!=null)
            {
                string email = "sadomazo@hatotato.com";
                string securePassword = "123Sc*123";
                Task<ApplicationUser> adminUser = userManager.FindByEmailAsync(email);
                adminUser.Wait();
                if (adminUser.Result == null)
                {
                    var admin = new ApplicationUser();
                    admin.Email = email;
                    admin.UserName = email;
                    
                    Task<IdentityResult> newUser = userManager.CreateAsync(admin, securePassword);
                    newUser.Wait();
                    if (newUser.Result.Succeeded)
                    {
                        var createAdminUser = userManager.FindByEmailAsync(email);
                        createAdminUser.Wait();

                        userManager?.AddClaimAsync(createAdminUser.Result, new System.Security.Claims.Claim("role", "admin"));
                    }
                    else
                    {
                        Console.WriteLine(newUser.Result.Errors);
                    }
                }
                  
            }
        } 
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
                {
                    options.IdentityResources["openid"].UserClaims.Add("role");
                    options.ApiResources.Single().UserClaims.Add("role");
            });
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("role");
            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IServiceProvider sp)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
            CreateUser(sp);
        }
        
    }
}

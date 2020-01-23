using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace IdentityService
{
    public class Startup
    {
        public Startup
        (
            IConfiguration configuration,
            IWebHostEnvironment environment
        )
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        public void Configure
        (
            IApplicationBuilder application,
            ApplicationIdentityDbContext identityContext,
            ConfigurationDbContext configurationContext,
            PersistedGrantDbContext persistedGrantContext,
            UserManager<ApplicationIdentityUser> userManager
        )
        {
            application.UseCors(cors => cors.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            application.UseHsts();
            application.UseHttpsRedirection();
            application.UseStaticFiles();
            application.UseRouting();
            application.UseAuthentication();
            application.UseAuthorization();
            application.UseEndpoints(builder => builder.MapDefaultControllerRoute());
            application.UseIdentityServer();

            identityContext.Database.Migrate();
            configurationContext.Database.Migrate();
            persistedGrantContext.Database.Migrate();

            configurationContext.Seed();
            userManager.Seed();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddAuthentication();
            services.AddAuthorization();
            services.AddControllersWithViews();

            var connectionString = Configuration.GetConnectionString("Connection");

            services.AddDbContext<ApplicationIdentityDbContext>(builder => builder.Connection(connectionString));

            services
                .AddIdentity<ApplicationIdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            var identityServer = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddConfigurationStore(options => options.ConfigureDbContext = builder => builder.Connection(connectionString))
            .AddOperationalStore(options => options.ConfigureDbContext = builder => builder.Connection(connectionString))
            .AddAspNetIdentity<ApplicationIdentityUser>();

            if (Environment.IsDevelopment())
            {
                identityServer.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("DeveloperSigningCredential");
            }
        }
    }
}

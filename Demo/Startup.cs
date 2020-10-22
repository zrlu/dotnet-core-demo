using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Demo.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Demo.Infrastructures;
using Demo.Models;
using VueCliMiddleware;
using Microsoft.AspNetCore.SpaServices;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;

namespace Demo
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
      services.AddLocalization(options => options.ResourcesPath = "Resources");
      services.AddMvc()
        .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
        .AddDataAnnotationsLocalization();
      services.Configure<RequestLocalizationOptions>(options =>
      {
        var supportedCultures = new[]
        {
                new CultureInfo("en-US"),
                new CultureInfo("zh-CN"),
            };

        options.DefaultRequestCulture = new RequestCulture(culture: "zh-CN", uiCulture: "zh-CN");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;
      });

      services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(
        Configuration.GetConnectionString("DefaultConnection"))
      );
      services.AddIdentity<AppUser, IdentityRole>(options =>
      {
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
      })
      .AddRoleManager<RoleManager<IdentityRole>>()
      .AddUserManager<AppUserManager>()
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultUI()
      .AddDefaultTokenProviders();
      services.AddControllersWithViews();
      services.AddRazorPages();

      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "ClientApp";
      });

      // Add Policy
      services.AddAuthorization(options =>
      {
        options.AddPolicy("RequireAdministrator",
          policy => policy.RequireRole("Administrator"));
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
      app.UseRequestLocalization();


      // SPA only
      //app.UseSpaStaticFiles();

      //app.UseCors(options =>
      //{
      //  options.AllowAnyHeader();
      //  options.AllowAnyMethod();
      //  options.AllowAnyOrigin();
      //});

      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");
        endpoints.MapRazorPages();

        // SPA only
        //endpoints.MapToVueCliProxy(
        //  "{*path}",
        //  new SpaOptions { SourcePath = "ClientApp" },
        //  npmScript: (System.Diagnostics.Debugger.IsAttached) ? "serve" : null,
        //  regex: "Compiled successfully",
        //  forceKill: true
        //);
      });

      // Add roles and Administrator
      using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
      var userManager = scope.ServiceProvider.GetRequiredService<AppUserManager>();
      var roleManager = scope.ServiceProvider.GetRequiredService <RoleManager<IdentityRole>>();
      var seeder = new IdentitySeeder(userManager, roleManager);
      seeder.SeedAll();
    }
  }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RecycleSystem.DataEntity.Entities;
using RecycleSystem.IService;
using RecycleSystem.Service;
using RecycleSystem.Ulitity;
using RecycleSystem.Ulitity.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RecycleSystem.MVC
{
    public class Startup
    {
        public IWebHostEnvironment WebHostEnvironment { get; set; }
        public Startup(IConfiguration configuration,IWebHostEnvironment env)
        {
            Configuration = configuration;
            WebHostEnvironment = env;
            GlobalContext.LogWhenStart(env);
            GlobalContext.HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSession();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserManageService, UserManageService>();
            services.AddScoped<IDepartmentManageService, DepartmentManageService>();
            services.AddScoped<IRoleManageService, RoleManageService>();
            services.AddScoped<IOrderManageService, OrderManageService>();
            services.AddScoped<IWareHouseService, WareHouseService>();
            services.AddScoped<ICategoryManageService, CategoryManageService>();
            services.AddScoped<IFinancialManageService, FinancialManageService>();
            services.AddScoped<ILogManageService, LogManageService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDetection();
            services.AddDetectionCore().AddBrowser();
            services.AddDetectionCore().AddDevice();

            services.AddScoped<DbContext, RecycleSystemDBContext>();
            services.AddDbContext<RecycleSystemDBContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SQLConnection"));
            });
            GlobalContext.SystemConfig = Configuration.GetSection("SystemConfig").Get<SystemConfig>();
            GlobalContext.Services = services;
            GlobalContext.Configuration = Configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            //loggerFactory.AddLog4Net();

            app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });

            app.UseAuthorization();

            string resource = Path.Combine(env.ContentRootPath, "Resource");
            FileHelper.CreateDirectory(resource);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            GlobalContext.ServiceProvider = app.ApplicationServices;
        }
    }
}

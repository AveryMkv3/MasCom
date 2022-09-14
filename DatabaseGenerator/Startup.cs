using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseGenerator
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration _config)
        {
            Configuration = _config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddControllersWithViews();

            //var conStr = Configuration.GetConnectionString("mascomdbMSSQL");

            var conStr = Configuration.GetConnectionString("MysqlDb");

            services.AddDbContextPool<AppDbContext>(options =>
                            options.UseMySql(conStr,
                                            ServerVersion.AutoDetect(conStr)));
            //services.AddDbContext<AppDbContext>(options => options.UseMySQL(conStr));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(ep =>
            {
                ep.MapRazorPages();
                ep.MapDefaultControllerRoute();
            });
        }
    }
}

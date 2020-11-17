using ItemManagement.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ItemManagement
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
            // EntityFramework 를 포함시키기
            // => Nuget 에서 설치 : Microsoft.EntityFrameworkCore.SqlServer
            services.AddDbContext<AppDbContext>(option => option.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // 프로그램 실행 중에 수정된 내용을 바로 반영할 수 있게 함. (웹 브라우저에서 새로고침으로 확인 가능)
            // => Nuget 에서 설치 : Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Item}/{action=Index}/{id?}");     // 기본 컨트롤로 및 Action 설정 
            });
        }
    }
}

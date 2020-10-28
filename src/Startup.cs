using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Honeypox.Nutritionizer.Data;

namespace Honeypox.Nutritionizer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<RecipeContext>(opt =>
                opt.UseNpgsql(Configuration.GetConnectionString("PostgreSqlConnection")));

            services.AddControllers();

            // Use service collection: services, to register IRecipeAPIRepo with SqlRecipeAPIRepo
            services.AddScoped<IRecipeAPIRepo, SqlRecipeAPIRepo>();

            services.AddOptions();
            
            /* This is in case I want to use custom configurations from the appsettings.json file
             * I had used it for the GLog.LogPath at one point, but I'm on the fence about it
             *
             * services.Configure<Settings>(Configuration.GetSection("Paths"));
             * services.AddSingleton<IConfiguration>(Configuration);
             */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
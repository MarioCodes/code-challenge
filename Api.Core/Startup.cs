using Api.Core.Configuration;
using Api.Core.Services;
using Api.Core.Services.interfaces;
using Api.External.Consumer.Common;
using Api.External.Consumer.Common.Interfaces;
using Api.External.Consumer.Services;
using Api.External.Consumer.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

namespace apicore
{
    public class Startup
    {

        public IConfiguration _config { get; }

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Service Configuration
            services.AddControllers();
            services.AddSwaggerGen();

            // Services
            services.AddScoped<ISlotsService, SlotsService>();
            services.AddScoped<IExternalApiService, ExternalApiService>();
            services.AddScoped<IHttpService, HttpService>();
            services.AddHttpClient<ExternalApiService>(c => c.BaseAddress = new System.Uri("https://draliatest.azurewebsites.net/api"));

            // add health checks
            services.AddHealthChecks();

            // Custom Configurations
            services.Configure<ExternalApiConfig>(_config.GetSection(ExternalApiConfig.Section));
            services.Configure<AuthConfig>(_config.GetSection(AuthConfig.Section));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // register concrete endpoint for health checks
            app.UseHealthChecks("/beat");
        }
    }
}

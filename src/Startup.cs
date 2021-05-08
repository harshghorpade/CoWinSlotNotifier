// ============================
// CoWin Service Startup class 
// ============================

using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Amazon.SimpleNotificationService;
using CoWinService.Domain.Interfaces;
using CoWinService.Services;

namespace CoWinService
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Amazon.Extensions.NETCore.Setup.AWSOptions awsOptions = Configuration.GetAWSOptions();
            awsOptions.Region = Amazon.RegionEndpoint.GetBySystemName(System.Environment.GetEnvironmentVariable("ENV_AWS_REGION"));
            services.AddDefaultAWSOptions(awsOptions);

            services.AddSingleton(provider => Configuration);
            services.AddSingleton<IHostedService, Application.CoWinListener>();
            services.AddSingleton<ICoWinServer, CoWinServer>();
            services.AddAWSService<IAmazonSimpleNotificationService>(ServiceLifetime.Singleton);
            services.AddSingleton<HttpClient>();
            services.AddSession();
            services.AddMvc()
                .AddNewtonsoftJson()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);

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
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}

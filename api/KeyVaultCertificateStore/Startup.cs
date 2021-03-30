using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using KeyVaultSecretGet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KeyVaultCertificateStore
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
            var configuration = new ConfigurationBuilder()
                .SetBasePath("/var/openfaas/secrets")
                .AddJsonFile("keyvault-secret.json", false,  true)
                .Build();
            services.Configure<KeyVaultOptions>(configuration.GetSection("Keyvault"));
            
            services.AddScoped(provider =>
            {
                var options = provider.GetService<IOptions<KeyVaultOptions>>().Value;

                return
                    new CertificateClient(
                        new Uri(options.Host ?? throw new ArgumentException("Hostname environment variable should be set!")),
                        options.CreateCredential());
            });
            services.AddScoped<KeyVaultCertificateAdder>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            
            app.UseExceptionHandler(builder => builder.Run( async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                if (contextFeature?.Error is RequestFailedException exception)
                {
                    context.Response.StatusCode = exception.Status;
                    context.Response.ContentType = "text/plain";
                    logger.LogError($"Something went wrong: {exception}");
                    await context.Response.WriteAsync(exception.ToString());
                }
            }));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}

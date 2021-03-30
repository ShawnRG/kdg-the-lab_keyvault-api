using System;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;


namespace KeyVaultKeyStore
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
                .AddJsonFile("keyvault-secret.json", false, true)
                .Build();
            services.Configure<KeyVaultOptions>(configuration.GetSection("keyvault"));

            services.AddScoped(provider =>
            {

                var options = provider.GetService<IOptions<KeyVaultOptions>>().Value;
                
        
                return
                    new KeyClient(
                        new Uri(options.Host ?? throw new ArgumentException("Hostname environment variable should be set!")),
                        options.CreateCredential());
            });
            
            services.AddScoped<KeyVaultKeyStorer>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
using CAKafka.Domain.Models;
using KafkaBasicSubscriber.Services.Subscriber;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace KafkaBasicSubscriber
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
            var kafkaOption = new KafkaOptions();
            Configuration.GetSection("Kafka").Bind(kafkaOption);
            services.AddOptions();
            services.Configure<KafkaOptions>(Configuration.GetSection("Kafka"));
            // services.AddSingleton<SuperEasySubscriber>();
            services.AddSingleton<IHostedService, EasyBackgroundListener>();
            services.AddSwaggerGen(c =>{ 
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Kafka API", Version = "v1"});
            });

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
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kafka API V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
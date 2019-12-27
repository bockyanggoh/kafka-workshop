using System;
using System.Reflection;
using Confluent.Kafka;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.Infrastructure;
using OrderMicroservice.Infrastructure.Repositories;
using OrderMicroservice.Kafka.Services;
using OrderMicroservice.OptionModel;

namespace OrderMicroservice
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
            services.AddOptions();
            services.Configure<KafkaOption>(Configuration.GetSection("Kafka"));
            services.AddSwaggerGen(c =>{ 
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Order APIs", Version = "v1"});
            });
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<OrdersDBContext>(options =>
                {
                    options.UseSqlServer(Configuration["ConnectionString"],
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(1),
                                errorNumbersToAdd: null);
                        });
                });
            
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddSingleton(typeof(IKafkaProducer<>), typeof(KafkaProducer<>));
            services.AddSingleton(typeof(IKafkaSubscriber<>), typeof(KafkaSubscriber<>));
            services.AddMvc()
                .AddJsonOptions(options => { options.JsonSerializerOptions.IgnoreNullValues = true; });
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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kafka API V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
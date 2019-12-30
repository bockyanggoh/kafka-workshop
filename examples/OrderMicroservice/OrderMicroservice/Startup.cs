using System;
using System.Linq;
using System.Reflection;
using Avro.Specific;
using BaselineTypeDiscovery;
using Kafka.Communication.Models;
using Lamar;
using Lamar.Scanning.Conventions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Win32;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.Infrastructure;
using OrderMicroservice.Infrastructure.Repositories;
using OrderMicroservice.Kafka.BackgroundService;
using OrderMicroservice.Kafka.Services;
using OrderMicroservice.Kafka.Services.impl;
using OrderMicroservice.Mediatr.Commands.CreateItemCommand;
using OrderMicroservice.Mediatr.Commands.CreateItemsCommand;
using OrderMicroservice.Mediatr.Commands.CreateOrderCommand;
using OrderMicroservice.Mediatr.Commands.RollbackOrderCommand;
using OrderMicroservice.Mediatr.Queries.GetItemQuery;
using OrderMicroservice.Mediatr.Queries.GetItemsQuery;
using OrderMicroservice.OptionModel;
using Container = Lamar.Container;

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
            
            

        }
        
        public void ConfigureContainer(ServiceRegistry services)
        {
            // Supports ASP.Net Core DI abstractions
            services.AddOptions();
            services.Configure<KafkaOption>(Configuration.GetSection("Kafka"));
            services.AddSwaggerGen(c =>{ 
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Order APIs", Version = "v1"});
            });

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
            services.AddMvc()
                .AddJsonOptions(options => { options.JsonSerializerOptions.IgnoreNullValues = true; });
            services.AddLogging();
            services.AddControllers();
            services.Scan(scanner =>
            {
                scanner.AssemblyContainingType<CreateOrderCommand>();
                scanner.AssemblyContainingType<CreateItemCommand>();
                scanner.AssemblyContainingType<CreateItemsCommand>();
                scanner.AssemblyContainingType<RollbackOrderCommand>();
                scanner.AssemblyContainingType<GetItemQuery>();
                scanner.AssemblyContainingType<GetItemsQuery>();
                scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
            });
            services.For<IMediator>().Use<Mediator>().Transient();
            services.For<ServiceFactory>().Use(ctx => ctx.GetInstance);
            services.For(typeof(IItemRepository)).Add(typeof(ItemRepository)).Singleton();
            services.For(typeof(IOrderRepository)).Add(typeof(OrderRepository)).Singleton();
            services.For(typeof(IKafkaProducer<>)).Add(typeof(KafkaProducer<>)).Singleton();
            services.For(typeof(IKafkaSubscriber<>)).Add(typeof(KafkaSubscriber<>)).Singleton();
            services.For(typeof(IKafkaMessageService<,>)).Add(typeof(KafkaMessageService<,>)).Singleton();

            services.AddCors(options => { options.AddDefaultPolicy(builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }); });
            // services.AddHostedService<PaymentBackgroundService>();
            // Also exposes Lamar specific registrations
            // and functionality
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseCors();
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
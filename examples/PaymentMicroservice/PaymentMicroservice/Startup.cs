using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Lamar;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OrderMicroservice.OptionModel;
using PaymentMicroservice.Domain.AggregateModel;
using PaymentMicroservice.Infrastructure;
using PaymentMicroservice.Infrastructure.Repositories;
using PaymentMicroservice.Kafka.BackgroundServices;
using PaymentMicroservice.Kafka.Services;
using PaymentMicroservice.Kafka.Services.impl;
using PaymentMicroservice.Mediatr.Commands.CreatePaymentCommand;
using PaymentMicroservice.Services.Publisher;
using PaymentMicroservice.Services.Subscriber;

namespace PaymentMicroservice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureContainer(ServiceRegistry services)
        {
            services.AddOptions();
            services.Configure<KafkaOption>(Configuration.GetSection("Kafka"));
            services.AddSwaggerGen(c =>{ 
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Order APIs", Version = "v1"});
            });
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<PaymentDBContext>(options =>
                {
                    options.UseSqlServer(Configuration["ConnectionString"],
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(1),
                                errorNumbersToAdd: null);
                        });
                });
            
            
            services.For<IMediator>().Use<Mediator>().Transient();
            services.For<ServiceFactory>().Use(ctx => ctx.GetInstance);
            services.Scan(scanner =>
            {
                scanner.AssemblyContainingType<CreatePaymentCommand>();
                scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
            });
            services.AddMvc()
                .AddJsonOptions(options => { options.JsonSerializerOptions.IgnoreNullValues = true; });
            services.AddControllers();
            services.AddSingleton<PublishPaymentResponseService>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.For(typeof(IKafkaProducer<>)).Add(typeof(KafkaProducer<>)).Singleton();
            services.For(typeof(IKafkaSubscriber<>)).Add(typeof(KafkaSubscriber<>)).Singleton();
            services.For(typeof(IKafkaMessageService<,>)).Add(typeof(KafkaMessageService<,>)).Singleton();
            services.AddHostedService<PaymentBackgroundService>();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kafka API V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        }
    }
}
using Application.Enums.Queries;
using Application.Interfaces;
using Domain;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace API.Server
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin();
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                });
            });

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Documentation",
                    Version = "v1"
                });
            });

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            }

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.Scan(scan => scan
                .FromAssemblyOf<DataAssembly>()
                .AddClasses()
                .AsImplementedInterfaces()
                .WithScopedLifetime()

                .FromAssemblyOf<DomainAssembly>()
                .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)))
                .AsSelfWithInterfaces()
                .WithTransientLifetime()
            );

            var domainAssembly = typeof(EnumExampleQuery).GetTypeInfo().Assembly;
            services.AddMediatR(config => config
                .RegisterServicesFromAssembly(domainAssembly)
            );

            services.AddScoped<ISaleRepository, SaleRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configuração do pipeline HTTP
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Documentation v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseCors();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}

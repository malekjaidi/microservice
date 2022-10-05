using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Identity.Commands.Handlers;
using Services.Identity.Data;
using Shared.Kafka;

namespace Services.Identity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string CorsPolicy = "_corsPolicy";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<IdentityDBContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddMediatR(typeof(RegisterUserCommandHandler).GetTypeInfo().Assembly);

            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy(name: CorsPolicy,
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin()
                                       .AllowAnyMethod()
                                       .AllowAnyHeader();
                                  });
            });

            services.AddKafkaMessageBus();

            services.AddKafkaProducer<string, User>(p =>
            {
                p.Topic = "users";
                p.BootstrapServers = "localhost:9092";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            DbInitilializer.Initialize(app.ApplicationServices);

            app.UseCors(CorsPolicy);
            app.UseRouting();
            app.UseMiddleware<CorsMiddleware>();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

using System;
using HumanCaptchaBackend.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HumanCaptchaBackend
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private IHostEnvironment environment;
        private readonly string allowedOrigins = string.Empty;

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;
            this.environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var allowOrigins = Configuration.GetSection("AppSettings:AllowedOrigins").Value;
            var arrOrigins = allowOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries);
            services.AddCors(options =>
            {
                options.AddPolicy(name: allowedOrigins,
                builder =>
                {
                    builder.WithOrigins(arrOrigins).AllowAnyHeader().AllowAnyMethod();
                });
            });
            if (environment.IsDevelopment())
            {
                services.AddCors(o => o.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                }));
            }


            services.AddDbContext<Data.HumanCaptchaContext>(options => options.UseSqlite("Data Source = captcha.db"));

            services.AddScoped<IExceptionManager>(s => new ExceptionManager(s.GetService<Data.HumanCaptchaContext>(), this.environment));

            services.AddScoped<TokenAuthenticationAttribute>();

            //services.AddScoped<TokenAuthenticationAttribute>(s => new TokenAuthenticationAttribute(s.GetService<Data.HumanCaptchaContext>(), s.GetService<IExceptionManager>()));


            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors();
            }
            else
                app.UseCors(allowedOrigins);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

using ChallengeDisney.Context;
using ChallengeDisney.Data;
using ChallengeDisney.Entities;
using ChallengeDisney.Interfaces;
using ChallengeDisney.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SendGrid.Extensions.DependencyInjection;
using System;
using System.Text;

namespace ChallengeDisney
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
           services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
           services.AddSwaggerGen(c =>
           {
               c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChallengeDisney", Version = "v1" });
           });
            
           services.AddIdentity<User, IdentityRole>()
              .AddEntityFrameworkStores<UserContext>()
              .AddDefaultTokenProviders();

           services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = "https://localhost:5001",
                        ValidIssuer = "https://localhost:5001",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Secret_Key"]))
                    };
                });

           services.AddEntityFrameworkSqlServer();
           services.AddDbContextPool<ChallengeDisneyContext>((services, options) =>
           {
               options.UseInternalServiceProvider(services);
               options.UseSqlServer(Configuration.GetConnectionString("ChallengeDisneyConnectionString"));
           });
            
           services.AddDbContextPool<UserContext>((services, options) =>
           {
               options.UseInternalServiceProvider(services);
               options.UseSqlServer(Configuration.GetConnectionString("UserConnectionString"));
           });

            services.AddScoped<IApiRepository, ApiRepository>();

            services.AddSendGrid(x => x.ApiKey = Configuration["ChallengeDisneyKey"]);
            services.AddTransient<IMailService, SendGridMailService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChallengeDisney v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

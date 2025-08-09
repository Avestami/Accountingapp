using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using Accounting.Infrastructure.Data;
using Accounting.Application.Interfaces;
using Accounting.Application.Features.Tickets.Handlers;
using Accounting.Infrastructure;
using Accounting.Infrastructure.Repositories;
using Accounting.Application.Common.Authorization;
using Accounting.Application.Services;

namespace Accounting.API
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
            // Database
            services.AddDbContext<AccountingDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Register DbContext interface
            services.AddScoped<IAccountingDbContext>(provider => provider.GetService<AccountingDbContext>());

            // Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register services
            services.AddScoped<IDocumentNumberService, DocumentNumberService>();
            services.AddScoped<IFxFifoService, FxFifoService>();

            // Register Query Handlers
            services.AddScoped<Accounting.Application.Common.Queries.IQueryHandler<Accounting.Application.Features.Dashboard.Queries.GetDashboardStatsQuery, Accounting.Application.Common.Models.Result<Accounting.Application.Features.Dashboard.Queries.DashboardStatsDto>>, Accounting.Application.Features.Dashboard.Queries.GetDashboardStatsQueryHandler>();

            // Add MediatR
            services.AddMediatR(typeof(CreateTicketCommandHandler).Assembly);

            // Add AutoMapper
            services.AddAutoMapper(typeof(CreateTicketCommandHandler).Assembly);

            // Add Authorization
            services.AddAuthorization(options =>
            {
                foreach (var permission in typeof(Permissions).GetFields())
                {
                    if (permission.IsLiteral && !permission.IsInitOnly)
                    {
                        var permissionValue = permission.GetValue(null)?.ToString();
                        if (!string.IsNullOrEmpty(permissionValue))
                        {
                            options.AddPolicy(permissionValue, policy =>
                                policy.Requirements.Add(new PermissionRequirement(permissionValue)));
                        }
                    }
                }
            });

            services.AddScoped<IAuthorizationHandler, PermissionHandler>();

            // Add JWT Authentication
            var jwtKey = Configuration["Jwt:Key"];
            if (!string.IsNullOrEmpty(jwtKey))
            {
                var key = Encoding.ASCII.GetBytes(jwtKey);
                services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            }
            else
            {
                // Add basic authentication if JWT key is not configured
                services.AddAuthentication();
            }

            // Add CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Accounting.API", Version = "v1" });
                
                // Add JWT to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Accounting.API v1"));
            }

            // app.UseHttpsRedirection(); // Temporarily disabled for testing

            // Add CORS
            app.UseCors("AllowAll");

            app.UseRouting();

            // Add Authentication and Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

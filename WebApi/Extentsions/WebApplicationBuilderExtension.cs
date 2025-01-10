using System.Runtime.CompilerServices;
using System.Text;
using InfrastructureLayer.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApi.Options;

namespace WebApi.Extentsions;

public static class WebApplicationBuilderExtension
{
    public static WebApplicationBuilder RegisterAuthentication(this WebApplicationBuilder builder)
    {
        var jwtSettings = builder.Configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
        if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SigninKey))
        {
            throw new InvalidOperationException("JWT SigningKey is not properly configured. Check your appsettings.json.");
        }

        builder.Services.AddAuthentication(a =>
        {
            a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            a.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(jwt =>
        {
            jwt.SaveToken = true;
            jwt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SigninKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudiences = jwtSettings.Audiences,
                RequireExpirationTime = false,
                ValidateLifetime = true,
            };
        });

        builder.Services.AddIdentityCore<IdentityUser>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 5;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
        })
        .AddRoles<IdentityRole>()
        .AddSignInManager()
        .AddEntityFrameworkStores<AppDbContext>();

        return builder;
    }


    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "League App", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });

        return services;
    }
}

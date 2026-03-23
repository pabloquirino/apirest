using System.Text;
using ApiRest.Application.Interfaces;
using ApiRest.Domain.Interfaces;
using ApiRest.Infrastructure.Auth;
using ApiRest.Infrastructure.Data;
using ApiRest.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ApiRest.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // EF Core + PostgreSQL
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("Default")));

        // JWT Settings
        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        // JWT Authentication
        var jwtSection = configuration.GetSection(JwtSettings.SectionName);
        var secretKey  = jwtSection["SecretKey"]!;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey  = true,
                    ValidIssuer              = jwtSection["Issuer"],
                    ValidAudience            = jwtSection["Audience"],
                    IssuerSigningKey         = new SymmetricSecurityKey(
                                                  Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew                = TimeSpan.Zero // sem tolerância extra
                };
            });

        services.AddAuthorization();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Auth service
        services.AddScoped<IAuthService, AuthService>();

        // Repositórios
        services.AddScoped<IUserRepository,         UserRepository>();
        services.AddScoped<IProductRepository,      ProductRepository>();
        services.AddScoped<IOrderRepository,        OrderRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }
}
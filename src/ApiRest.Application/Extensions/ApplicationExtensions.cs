using ApiRest.Application.Behaviors;
using ApiRest.Application.Products.Commands;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ApiRest.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        var assembly = typeof(CreateProductCommand).Assembly;

        // MediatR — todos os handlers do assembly
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(assembly));

        // FluentValidation — todos os validators do assembly
        services.AddValidatorsFromAssembly(assembly);

        // Pipeline behavior — roda antes de cada handler
        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        return services;
    }
}
using ApplicationLayer.Abstractions;
using InfrastructureLayer;
using InfrastructureLayer.Context;

namespace WebApi.Config;

public static class ServiceConfig
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository,UserRepository>();
        //services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public static void AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IUserRepository).Assembly));
    }

    public static void AddDbContext(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>();
    }
    //public static void AddJsonOptions(this IServiceCollection services)
    //{
    //    services.AddControllers()
    //    .AddJsonOptions(options =>
    //    {
    //        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    //    });
    //}
}

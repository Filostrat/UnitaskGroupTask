using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Reflection;


namespace Application.Extensions;

public static class ApplicationServiceRegistration
{

    extension(IServiceCollection services)
    {
        public void AddApplicationLayer(IConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(assembly));

            services.AddAutoMapper(assembly);
        }
    }
}

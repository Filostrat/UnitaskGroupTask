using Domain.Contracts.Infrastructure;

using Infrastructure.Repositories;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Infrastructure.Extensions;

public static class InfrastructureServiceRegistration
{
    extension(IServiceCollection provider)
    {
        public void AddInfrastructureLayer(IConfiguration configuration)
        {
            provider.AddScoped<IEmployeeRepository>(sp =>
            {
                var cs = configuration.GetConnectionString("DefaultConnection")
                         ?? throw new InvalidOperationException("DefaultConnection not found");
                return new EmployeeRepository(cs);
            });
        }
    }
}
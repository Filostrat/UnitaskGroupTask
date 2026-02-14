using Microsoft.OpenApi;


namespace UnitaskGroupApi.Extensions;

public static class ApiServiceRegistration
{

    extension(IServiceCollection services)
    {
        public void AddApiLayer(IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Employees API",
                    Version = "v1"
                });
            });
        }
    }
}

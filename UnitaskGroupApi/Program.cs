using Application.Extensions;

using Infrastructure.Extensions;

using Serilog;

using UnitaskGroupApi.Extensions;
using UnitaskGroupApi.Logging;
using UnitaskGroupApi.Middlewares;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Host.UseSerilog((context, services, loggerConfig) =>
{
    var httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();

    loggerConfig.ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.With(new HttpContextTraceIdEnricher(httpContextAccessor));
});

builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddApiLayer(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employees API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
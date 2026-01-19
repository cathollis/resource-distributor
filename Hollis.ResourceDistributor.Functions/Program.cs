using Hollis.ResourceDistributor.Functions;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var projectName = nameof(ResourceDistributorDbContext).Replace(nameof(DbContext), string.Empty);
var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Services.AddDbContext<ResourceDistributorDbContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable(string.Join("_", ["SQLCONNSTR", projectName]));
    options.UseSqlServer(connectionString, sqlServer =>
    {
        sqlServer.MigrationsHistoryTable("__EFMigrationsHistory", projectName);
    });
});

builder.Services.AddHttpClient();

builder.Build().Run();

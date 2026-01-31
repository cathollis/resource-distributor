using Hollis.ResourceDistributor.Functions;
using Hollis.ResourceDistributor.Functions.Configs;
using Hollis.ResourceDistributor.Ip2LocationClient.Configs;
using Hollis.ResourceDistributor.Ip2LocationClient.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var projectName = nameof(ResourceDistributorDbContext).Replace(nameof(DbContext), string.Empty);

var builder = FunctionsApplication.CreateBuilder(args);
builder.ConfigureFunctionsWebApplication();

builder.Services.Configure<AppConfig>(builder.Configuration.GetSection(nameof(AppConfig)));

builder.Services.AddHttpClient();
builder.Services.AddIp2Location(builder.Configuration.GetSection(nameof(Ip2LocationConfig)));
builder.Services.AddDbContext<ResourceDistributorDbContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable(string.Join("_", ["SQLCONNSTR", projectName]));
    options.UseSqlServer(connectionString, sqlServer =>
    {
        sqlServer.MigrationsHistoryTable("__EFMigrationsHistory", projectName);
    });
});
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();

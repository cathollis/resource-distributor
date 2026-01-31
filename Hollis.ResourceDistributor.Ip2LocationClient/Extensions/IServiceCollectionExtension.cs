using Hollis.ResourceDistributor.Ip2LocationClient.Configs;
using Hollis.ResourceDistributor.Ip2LocationClient.Consts;
using Hollis.ResourceDistributor.Ip2LocationClient.DelegatingHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Hollis.ResourceDistributor.Ip2LocationClient.Extensions;

public static class IServiceCollectionExtension
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public static void AddIp2Location(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<Ip2LocationConfig>(configuration);

        AddIp2Location(services);
    }

    public static void AddIp2Location(this IServiceCollection services)
    {
        var refitSetting = new RefitSettings(new SystemTextJsonContentSerializer(_jsonOptions));

        services.AddTransient<TokenHeaderHandler>();
        services.AddRefitClient<IIp2LocationApi>(refitSetting)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(Ip2LocationApiConst.BaseUrl))
            .AddHttpMessageHandler<TokenHeaderHandler>();
    }
}

using Hollis.ResourceDistributor.Ip2LocationClient.Configs;
using Hollis.ResourceDistributor.Ip2LocationClient.Consts;
using Hollis.ResourceDistributor.Ip2LocationClient.DelegatingHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hollis.ResourceDistributor.Ip2LocationClient.Extensions;

public static class IServiceCollectionExtension
{
    public static void AddIp2Location(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<Ip2LocationConfig>(configuration);

        AddIp2Location(services);
    }

    public static void AddIp2Location(this IServiceCollection services)
    {
        services.AddTransient<TokenHeaderHandler>();
        services.AddRefitClient<IIp2LocationApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(Ip2LocationApiConst.BaseUrl))
            .AddHttpMessageHandler<TokenHeaderHandler>();
    }
}

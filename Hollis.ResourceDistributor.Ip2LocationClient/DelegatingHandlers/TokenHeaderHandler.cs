using Hollis.ResourceDistributor.Ip2LocationClient.Configs;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Hollis.ResourceDistributor.Ip2LocationClient.DelegatingHandlers;

class TokenHeaderHandler(IOptionsMonitor<Ip2LocationConfig> options) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = options.CurrentValue.ApiKey;

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hollis.ResourceDistributor.Functions;

public class AppConfig
{
    public required Uri TargetUrl { get; init; }

    public required string DefaultUserAgent { get; init; }

    public required IList<string> CopiedRequestHeaders { get; init; } = [];

    public required IList<string> CopiedResponseHeaders { get; init; } = [];
}

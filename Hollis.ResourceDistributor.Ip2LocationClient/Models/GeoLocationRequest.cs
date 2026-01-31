using System;
using System.Collections.Generic;
using System.Text;

namespace Hollis.ResourceDistributor.Ip2LocationClient.Models;

public class GeoLocationRequest
{
    public required string Ip { get; set; }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Hollis.ResourceDistributor.Functions.Entities;

[Index(nameof(ClearTextKey))]
public class User
{
    public required Guid Id { get; init; }

    [MaxLength(64)]
    public required string ClearTextKey { get; set; }

    public bool Disabled { get; set; }

    [MaxLength(128)]
    public string? IdentificationName { get; set; }

    public virtual IList<Resource>? Resources { get; set; }
}

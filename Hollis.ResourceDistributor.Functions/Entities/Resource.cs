using System.ComponentModel.DataAnnotations;

namespace Hollis.ResourceDistributor.Functions.Entities;

public class Resource
{
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    [MaxLength(64)]
    public required string Name { get; set; }

    public bool AllowAnymouse { get; set; }

    [MaxLength(128)]
    public required Uri TargetUrl { get; set; }

    [MaxLength(2048)]
    public List<string> RequestCopyHeaderName { get; private set; } = [];

    [MaxLength(2048)]
    public List<string> ResponseCopyHeaderName { get; private set; } = [];

    [MaxLength(512)]
    public string? Comment { get; set; }

    public virtual IList<User>? AllowUsers { get; set; }
}

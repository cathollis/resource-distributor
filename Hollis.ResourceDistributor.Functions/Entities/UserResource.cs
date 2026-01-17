using System.ComponentModel.DataAnnotations;

namespace Hollis.ResourceDistributor.Functions.Entities;

public class UserResource
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public required Guid UserId { get; init; }

    public required Guid ResourceId { get; init; }

    public virtual User User { get; private set; } = null!;

    public virtual Resource Resource { get; private set; } = null!;
}

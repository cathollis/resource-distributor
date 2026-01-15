namespace Hollis.ResourceDistributor.Functions.Entities;

public class UserResource
{
    public Guid Id { get; init; }

    public Guid UserId { get; set; }

    public Guid ResourceId { get; set; }

    public virtual User User { get; private set; } = null!;

    public virtual Resource Resource { get; private set; } = null!;
}

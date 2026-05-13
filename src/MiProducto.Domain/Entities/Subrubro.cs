using MiProducto.Domain.Common;

namespace MiProducto.Domain.Entities;

public class Subrubro : BaseEntity
{
    public string Name { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;
    public Guid RubroId { get; private set; }
    public Rubro Rubro { get; private set; } = default!;

    private Subrubro() { }

    public static Subrubro Create(string name, Guid rubroId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Subrubro { Name = name, RubroId = rubroId };
    }

    public void Update(string name, Guid rubroId)
    {
        Name = name;
        RubroId = rubroId;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}
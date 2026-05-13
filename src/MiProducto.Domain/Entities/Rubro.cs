using MiProducto.Domain.Common;

namespace MiProducto.Domain.Entities;

public class Rubro : BaseEntity
{
    public string Name { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;
    public ICollection<Subrubro> Subrubros { get; private set; } = new List<Subrubro>();

    private Rubro() { }

    public static Rubro Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Rubro { Name = name };
    }

    public void Update(string name)
    {
        Name = name;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}
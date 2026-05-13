using MiProducto.Domain.Common;

namespace MiProducto.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public string Role { get; private set; } = "User";
    public string? GoogleId { get; private set; }
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

    private User() { }

    public static User Create(string email, string passwordHash, string fullName, string role = "User")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);

        return new User
        {
            Email = email.ToLower().Trim(),
            PasswordHash = passwordHash,
            FullName = fullName,
            Role = role
        };
    }

    public static User CreateFromGoogle(string email, string fullName, string googleId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(googleId);

        return new User
        {
            Email = email.ToLower().Trim(),
            PasswordHash = Guid.NewGuid().ToString(),
            FullName = fullName,
            Role = "User",
            GoogleId = googleId
        };
    }

    public void LinkGoogle(string googleId)
    {
        GoogleId = googleId;
        SetUpdatedAt();
    }
}
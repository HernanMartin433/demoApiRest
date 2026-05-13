using MiProducto.Domain.Entities;

namespace MiProducto.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}
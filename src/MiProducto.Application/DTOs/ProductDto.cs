namespace MiProducto.Application.DTOs;

public record ProductImageDto(Guid Id, string ImageUrl, int Order);

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    bool IsActive,
    Guid RubroId,
    string RubroName,
    Guid SubrubroId,
    string SubrubroName,
    IEnumerable<ProductImageDto> Images,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record RubroDto(Guid Id, string Name, bool IsActive, IEnumerable<SubrubroDto> Subrubros);
public record SubrubroDto(Guid Id, string Name, bool IsActive, Guid RubroId, string RubroName);
public record CartItemDto(Guid Id, Guid ProductId, string ProductName, string? ImageUrl, decimal UnitPrice, int Quantity, decimal Subtotal);
public record CartDto(Guid Id, IEnumerable<CartItemDto> Items, decimal Total, int ItemCount);

public record OrderItemDto(Guid Id, Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal Subtotal);
public record OrderDto(
    Guid Id,
    string Status,
    string PaymentStatus,
    string? PaymentMethod,
    DateTime? PaidAt,
    decimal Total,
    IEnumerable<OrderItemDto> Items,
    DateTime CreatedAt
);
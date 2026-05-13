using Moq;
using MiProducto.Application.Common.Interfaces;

namespace MiProducto.UnitTests.Helpers;

public static class MockUnitOfWork
{
    public static Mock<IUnitOfWork> Create()
    {
        var mock = new Mock<IUnitOfWork>();
        var productRepo = new Mock<IProductRepository>();
        var userRepo = new Mock<IUserRepository>();
        var refreshTokenRepo = new Mock<IRefreshTokenRepository>();
        var productImageRepo = new Mock<IProductImageRepository>();
        var rubroRepo = new Mock<IRubroRepository>();
        var subrubroRepo = new Mock<ISubrubroRepository>();

        mock.Setup(u => u.Products).Returns(productRepo.Object);
        mock.Setup(u => u.Users).Returns(userRepo.Object);
        mock.Setup(u => u.RefreshTokens).Returns(refreshTokenRepo.Object);
        mock.Setup(u => u.ProductImages).Returns(productImageRepo.Object);
        mock.Setup(u => u.Rubros).Returns(rubroRepo.Object);
        mock.Setup(u => u.Subrubros).Returns(subrubroRepo.Object);
        mock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        return mock;
    }
}
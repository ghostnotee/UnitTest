using Moq;
using UnitTest.Web.Controllers;
using UnitTest.Web.Models;
using UnitTest.Web.Repository;

namespace UnitTest.Test;

public class ProductControllerTest
{
    private readonly Mock<IRepository<Product>> _mockRepository;
    private readonly ProductsController _productsController;
    private List<Product> products;

    public ProductControllerTest()
    {
        _mockRepository = new Mock<IRepository<Product>>();
        _productsController = new ProductsController(_mockRepository.Object);
        products = new List<Product>() {
            new Product { Id = 1, Name = "Monitör", Color = "Kırmızı", Price = 25, Stock = 10 },
            new Product { Id = 1, Name = "Notebook", Color = "Kırmızı", Price = 25, Stock = 10 }
        };
    }
}

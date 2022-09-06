using Microsoft.AspNetCore.Mvc;
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
        _mockRepository = new Mock<IRepository<Product>>();   // default MockBehavior.Loose
        _productsController = new ProductsController(_mockRepository.Object);
        products = new List<Product>() {
            new Product { Id = 1, Name = "Monitör", Color = "Kırmızı", Price = 25, Stock = 10 },
            new Product { Id = 2, Name = "Notebook", Color = "Kırmızı", Price = 25, Stock = 10 }
        };
    }

    [Fact]
    public async void Index_ActionExecutes_ReturnView()
    {
        var result = await _productsController.Index();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async void Index_ActionExecutes_ReturnProductList()
    {
        _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);
        var result = await _productsController.Index();
        var viewResult = Assert.IsType<ViewResult>(result);
        var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
        Assert.Equal<int>(2, productList.Count());
    }

    [Fact]
    public async void Details_IdisNull_ReturnRedirectToIndexAction()
    {
        var result = await _productsController.Details(null);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async void Details_IdInvalid_ReturnNotFound()
    {
        Product? product = null;
        _mockRepository.Setup(x => x.GetByIdAsync(0)).ReturnsAsync(product);
        var result = await _productsController.Details(0);
        var redirect = Assert.IsType<NotFoundResult>(result);
        Assert.Equal<int>(404, redirect.StatusCode);
    }

    [Theory]
    [InlineData(1)]
    public async void Details_ValidId_ReturnProduct(int productId)
    {
        var product = products.First(x => x.Id == productId);
        _mockRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
        var result = await _productsController.Details(productId);
        var viewResult = Assert.IsType<ViewResult>(result);
        var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);
        Assert.Equal(product.Id, resultProduct.Id);
        Assert.Equal(product.Name, resultProduct.Name);
    }

    [Fact]
    public void Create_ActionExecutes_ReturnView()
    {
        var result = _productsController.Create();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async void Create_InvalidModelState_ReturnView()
    {
        _productsController.ModelState.AddModelError("Name", "Name field is required.");
        var result = await _productsController.Create(products.First());
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<Product>(viewResult.Model);
    }

    [Fact]
    public async void CreatePOST_ValidModelState_ReturnRedirectToIndexAction()
    {
        var result = await _productsController.Create(products.First());
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async void CreatePOST_ValidModelState_CreateMethodExecute()
    {
        Product newProduct = null;
        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<Product>())).Callback<Product>(x => newProduct = x);
        var result = await _productsController.Create(products.First());
        _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Once);
        Assert.Equal(products.First().Id, newProduct.Id);
    }

    [Fact]
    public async void CreatePOST_InValidModelState_NeverCreateExecute()
    {
        _productsController.ModelState.AddModelError("Name", "");
        var result = await _productsController.Create(products.First());
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async void Edit_IdIsNull_RedirectToIndexAction()
    {
        var result = await _productsController.Edit(null);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Theory]
    [InlineData(3)]
    public async void Edit_IdInvalid_ReturnNotFound(int productId)
    {
        Product? product = null;
        _mockRepository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);
        var result = await _productsController.Edit(productId);
        var redirect = Assert.IsType<NotFoundResult>(result);
        Assert.Equal(404, redirect.StatusCode);
    }

    [Theory]
    [InlineData(1)]
    public async void Edit_ActionExecutes_ReturnProduct(int productId)
    {
        var product = products.First(p => p.Id == productId);
        _mockRepository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);
        var result = await _productsController.Edit(productId);
        var viewResult = Assert.IsType<ViewResult>(result);
        var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);
        Assert.Equal(product.Id, resultProduct.Id);
        Assert.Equal(product.Name, resultProduct.Name);
    }

    [Theory]
    [InlineData(1)]
    public void EditPOST_IdIsNotEqualProduct_ReturnNotFound(int productId)
    {
        var result = _productsController.Edit(2, products.First(p => p.Id == productId));
        var resirect = Assert.IsType<NotFoundResult>(result);
    }

    [Theory]
    [InlineData(1)]
    public void EditPOST_InvalidModelState_ReturnView(int productId)
    {
        _productsController.ModelState.AddModelError("Name", "");
        var result = _productsController.Edit(productId, products.First(p => p.Id == productId));
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<Product>(viewResult.Model);
    }

    [Theory]
    [InlineData(1)]
    public void EditPOST_ValidModelState_UpdateMethodExecute(int productId)
    {
        var product = products.First(x => x.Id == productId);
        _mockRepository.Setup(x => x.Update(It.IsAny<Product>()));
        _productsController.Edit(productId, product);
        _mockRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async void Delete_IdIsNull_ReturnNotFound()
    {
        var result = await _productsController.Delete(null);
        Assert.IsType<NotFoundResult>(result);
    }

    [Theory]
    [InlineData(1)]
    public async void DeleteConfirmed_ActionExecutes_ReturnRedirectToActionAsync(int productId)
    {
        var result = await _productsController.DeleteConfirmed(productId);
        Assert.IsType<RedirectToActionResult>(result);
    }

    [Theory]
    [InlineData(1)]
    public async void DeleteConfirmed_ActionExecutes_DeleteMethodExecute(int productId)
    {
        var product = products.First(x => x.Id == productId);
        _mockRepository.Setup(x => x.Delete(product));
        await _productsController.DeleteConfirmed(productId);
        _mockRepository.Verify(x => x.Delete(It.IsAny<Product>()), Times.Once);
    }
}

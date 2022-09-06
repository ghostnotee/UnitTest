using Microsoft.AspNetCore.Mvc;
using UnitTest.Web.Models;
using UnitTest.Web.Repository;

namespace UnitTest.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IRepository<Product> _productsRepository;

    public ProductsController(IRepository<Product> productsRepository)
    {
        _productsRepository = productsRepository;
    }

    // GET: Products
    public async Task<IActionResult> Index()
    {
        return View(await _productsRepository.GetAllAsync());
    }

    // GET: Products/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _productsRepository == null)
        {
            return RedirectToAction("Index");
        }

        var product = await _productsRepository.GetByIdAsync((int)id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // GET: Products/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Products/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Price,Stock,Color")] Product product)
    {
        if (ModelState.IsValid)
        {
            await _productsRepository.CreateAsync(product);
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    // GET: Products/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _productsRepository == null)
        {
            return RedirectToAction("Index");
        }

        var product = await _productsRepository.GetByIdAsync((int)id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    // POST: Products/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("Id,Name,Price,Stock,Color")] Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _productsRepository.Update(product);
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    // GET: Products/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _productsRepository == null)
        {
            return NotFound();
        }

        var product = await _productsRepository.GetByIdAsync((int)id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // POST: Products/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_productsRepository == null)
        {
            return Problem("Entity set 'unittestdbContext.Products'  is null.");
        }
        var product = await _productsRepository.GetByIdAsync(id);
        if (product != null)
        {
            _productsRepository.Delete(product);
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ProductExists(int id)
    {
        var dbProduct = _productsRepository.GetByIdAsync(id).Result;
        return dbProduct is not null ? true : false;
    }
}

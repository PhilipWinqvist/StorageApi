using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorageApi.Models;
using StorageApi.DTOs;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly StorageContext _context;
    public ProductsController(StorageContext context)
    {
        _context = context;
    }

    // GET: api/Products
    // GET: api/Products?category=Leksaker
    // GET: api/Products?name=bad
    // GET: api/Products?category=Leksaker&name=bad
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProduct(string? category, string? name)
    {
        var query = _context.Product.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p => p.Name.Contains(name));
        }

        return await query
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Count = p.Count
            })
            .ToListAsync();
    }

    // GET: api/Products/stats
    [HttpGet("stats")]
    public async Task<ActionResult<StatsDto>> GetStats()
    {
        var totalProducts = await _context.Product.CountAsync();

        if (totalProducts == 0)
        {
            return new StatsDto();
        }

        var totalValue = await _context.Product.SumAsync(p => (long)p.Price * p.Count);
        var averagePrice = await _context.Product.AverageAsync(p => (double)p.Price);

        return new StatsDto
        {
            TotalProducts = totalProducts,
            TotalInventoryValue = totalValue,
            AveragePrice = Math.Round(averagePrice, 2)
        };
    }

    // GET: api/Products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _context.Product.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Count = product.Count
        };
    }

    // PUT: api/Products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(int? id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest();
        }

        _context.Entry(product).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Products
    [HttpPost]
    public async Task<ActionResult<ProductDto>> PostProduct(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            Category = dto.Category,
            Shelf = dto.Shelf,
            Count = dto.Count,
            Description = dto.Description
        };

        _context.Product.Add(product);
        await _context.SaveChangesAsync();

        var result = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Count = product.Count
        };

        return CreatedAtAction("GetProduct", new { id = product.Id }, result);
    }

    // DELETE: api/Products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int? id)
    {
        var product = await _context.Product.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Product.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductExists(int? id)
    {
        return _context.Product.Any(e => e.Id == id);
    }
}
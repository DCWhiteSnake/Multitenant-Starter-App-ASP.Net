using MultitenantStarter.Common.Entities;
using MultitenantStarter.Investments.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MultitenantStarter.LoanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "RequireTenant")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContextFactory _dbFactory;
        public ProductsController(AppDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll([FromHeader] string Authorization)
        {
            using var db = _dbFactory.CreateDbContext();
            var items = await db.Products.AsNoTracking().ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            var item = await db.Products.FindAsync(id);
            if (item is null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create([FromHeader] string Authorization, [FromBody] Product input)
        {
            using var db = _dbFactory.CreateDbContext();
            input.CreatedAt = DateTime.UtcNow;
            input.UpdatedAt = DateTime.UtcNow;
            db.Products.Add(input);
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Product input)
        {
            if (id != input.Id) return BadRequest();
            using var db = _dbFactory.CreateDbContext();
            var existing = await db.Products.FindAsync(id);
            if (existing is null) return NotFound();
            existing.Name = input.Name;
            existing.Description = input.Description;
            existing.Price = input.Price;
            existing.MinimumInvestmentAmount = input.MinimumInvestmentAmount;
            existing.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            var existing = await db.Products.FindAsync(id);
            if (existing is null) return NotFound();
            db.Products.Remove(existing);
            await db.SaveChangesAsync();
            return NoContent();
        }
    }
}

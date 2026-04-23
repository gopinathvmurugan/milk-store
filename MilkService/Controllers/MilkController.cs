using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilkService.Data;
using MilkService.Models;

namespace MilkService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MilkController : ControllerBase
    {
        private readonly MilkDbContext _context;

        public MilkController(MilkDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL (with Customer + Supplier)
        [HttpGet]
        public IActionResult Get()
        {
            var data = _context.Milks
                .Include(m => m.Customer)
                .Include(m => m.Supplier)
                .ToList();

            return Ok(data);
        }

        // ✅ ADD
        [HttpPost]
        public IActionResult Add([FromBody] Milk data)
        {
            // 🔥 VALIDATION
            if (!_context.Customers.Any(c => c.Id == data.CustomerId))
                return BadRequest("Invalid Customer");

            if (!_context.Suppliers.Any(s => s.Id == data.SupplierId))
                return BadRequest("Invalid Supplier");

            _context.Milks.Add(data);
            _context.SaveChanges();

            return Ok(data);
        }
        // ✅ UPDATE MILK
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Milk data)
        {
            var existing = _context.Milks.Find(id);

            if (existing == null)
                return NotFound("Milk not found");

            // ✅ Validate foreign keys
            if (!_context.Customers.Any(c => c.Id == data.CustomerId))
                return BadRequest("Invalid Customer");

            if (!_context.Suppliers.Any(s => s.Id == data.SupplierId))
                return BadRequest("Invalid Supplier");

            // ✅ Update fields
            existing.Liters = data.Liters;
            existing.CustomerId = data.CustomerId;
            existing.SupplierId = data.SupplierId;
            existing.Session = data.Session;
            existing.Date = data.Date;

            _context.SaveChanges();

            return Ok(existing);
        }
        // ✅ DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = _context.Milks.Find(id);

            if (item == null)
                return NotFound();

            _context.Milks.Remove(item);
            _context.SaveChanges();

            return Ok();
        }
        [HttpGet("report")]
        public IActionResult GetReport(
    DateTime fromDate,
    DateTime toDate,
    int page = 1,
    int pageSize = 10)
        {
            var query = _context.Milks
                .Include(x => x.Customer)
                .Include(x => x.Supplier)
                .Where(x => x.Date >= fromDate && x.Date <= toDate);

            var totalCount = query.Count();

            var data = query
                .OrderByDescending(x => x.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                data
            });
        }
    }
}
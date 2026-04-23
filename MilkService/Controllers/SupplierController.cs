using Microsoft.AspNetCore.Mvc;
using MilkService.Data;
using MilkService.Models;

namespace MilkService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly MilkDbContext _context;

        public SupplierController(MilkDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_context.Suppliers.ToList());
        }

        [HttpPost]
        public IActionResult Add(Supplier data)
        {
            _context.Suppliers.Add(data);
            _context.SaveChanges();
            return Ok(data);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Supplier updated)
        {
            var item = _context.Suppliers.Find(id);
            if (item == null) return NotFound();

            item.Name = updated.Name;
            item.Phone = updated.Phone;
            item.LineNo = updated.LineNo;
            item.Address = updated.Address;

            _context.SaveChanges();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = _context.Suppliers.Find(id);
            if (item == null) return NotFound();

            _context.Suppliers.Remove(item);
            _context.SaveChanges();

            return Ok();
        }
    }
}
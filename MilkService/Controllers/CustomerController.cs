using Microsoft.AspNetCore.Mvc;
using MilkService.Data;
using MilkService.Models;
using System;

namespace MilkService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly MilkDbContext _context;

        public CustomerController(MilkDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL
        [HttpGet]
        public IActionResult Get()
        {
            var customers = _context.Customers.ToList();
            return Ok(customers);
        }

        // ✅ ADD
        [HttpPost]
        public IActionResult Add([FromBody] Customer data)
        {
            if (string.IsNullOrEmpty(data.Name) || string.IsNullOrEmpty(data.Phone))
                return BadRequest("Name and Phone are required");

            _context.Customers.Add(data);
            _context.SaveChanges();

            return Ok(data);
        }

        // ✅ UPDATE
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Customer data)
        {
            var existing = _context.Customers.Find(id);

            if (existing == null)
                return NotFound("Customer not found");

            existing.Name = data.Name;
            existing.Phone = data.Phone;
            existing.LineNo = data.LineNo;
            existing.Address = data.Address;

            _context.SaveChanges();

            return Ok(existing);
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = _context.Customers.Find(id);

            if (item == null)
                return NotFound("Customer not found");

            _context.Customers.Remove(item);
            _context.SaveChanges();

            return Ok("Deleted successfully");
        }
    }
}
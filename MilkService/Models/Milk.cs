namespace MilkService.Models
{
    public class Milk
    {
        public int Id { get; set; }

        public double Liters { get; set; }

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        

        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public string Session { get; set; }
        public DateTime Date { get; set; }

        // 🔥 NEW
        public double CustomerRate { get; set; }
        public double SupplierRate { get; set; }

    }
}
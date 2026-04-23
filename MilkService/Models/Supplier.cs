namespace MilkService.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        // ✅ NEW FIELDS
        public string LineNo { get; set; }
        public string Address { get; set; }
    }
}

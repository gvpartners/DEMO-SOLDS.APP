using System.ComponentModel.DataAnnotations;

namespace DEMO_SOLDS.APP.Models
{
    public class Customers
    {
        [Key]
        public Guid Id { get; set; }
        public string? CustomerName { get; set; }
        public string? IdentificationType { get; set; }
        public string? IdentificationInfo { get; set; }
        public string? CustomerAddress { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class CustomerListResponse
    {
        public List<Customers> Customers { get; set; }
        public int Total { get; set; }
    }
}
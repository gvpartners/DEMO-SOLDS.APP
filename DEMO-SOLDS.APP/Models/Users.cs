using System.ComponentModel.DataAnnotations;

namespace DEMO_SOLDS.APP.Models
{
    public class Users
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? FirstLastName { get; set; }
        public string? SecondLastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Prefix { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        public string? Phone { get; set; }
    }
    public class UsersModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? FirstLastName { get; set; }
        public string? SecondLastName { get; set; }
        public string? Email { get; set; }
        public string? Prefix { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        public string? Phone { get; set; }
    }
}

namespace DEMO_SOLDS.APP.Models
{
    public class Visit
    {
        public Guid Id { get; set; }
        public string Client { get; set; }
        public string Work { get; set; }
        public string WorkAddress { get; set; }
        public string Contacts { get; set; }
        public string VisitReason { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string? VisitCode { get; set; }
        public int StatusOrder { get; set; }
        public string? StatusName { get; set; }
        public string? Comment { get; set; }
    }
    public class VisitModel
    {
        public string Client { get; set; }
        public string Work { get; set; }
        public string WorkAddress { get; set; }
        public List<ContactModel> Contacts { get; set; }
        public string VisitReason { get; set; }
        public string OtherReason { get; set; }
        public string CreatedBy { get; set; }
    }
    public class AuxVisitModel
    {
        public Guid Id { get; set; }
        public string Client { get; set; }
        public string Work { get; set; }
        public string WorkAddress { get; set; }
        public string Contacts { get; set; }
        public string VisitReason { get; set; }
        public string VisitCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int StatusOrder { get; set; }
        public string? StatusName { get; set; }
        public string? Comment { get; set; }

    }
    public class VisitListResponse
    {
        public List<AuxVisitModel> Visits { get; set; }
        public int Total { get; set; }
    }
    public class ContactModel
    {
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}

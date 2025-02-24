using DocumentFormat.OpenXml.Spreadsheet;

namespace DEMO_SOLDS.APP.Models
{
    public class VisitPage
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public FiltersVisit? Filters { get; set; }
    }

    public class FiltersVisit
    {
        public string? VisitCodeFilter { get; set; }
        public string? ClientFilter { get; set; }
        public string? WorkFilter { get; set; }
        public string? WorkAddressFilter { get; set; }
        public string? ContactsFilter { get; set; }
        public string? VisitReasonFilter { get; set; }
        public string? CreatedByFilter { get; set; }
        public string? StatusNameFilter { get; set; }
        public DateTime? CreatedOnFilter { get; set; }

    }

}

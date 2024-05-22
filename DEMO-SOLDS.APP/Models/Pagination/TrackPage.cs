using DocumentFormat.OpenXml.Spreadsheet;

namespace DEMO_SOLDS.APP.Models
{
    public class TrackPage
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public FiltersTrack? Filters { get; set; }
    }

    public class FiltersTrack
    {
        public string? TrackCodeFilter { get; set; }
        public string? IdentificationInfoFilter { get; set; }
        public string? SelectedCategoryFilter { get; set; }
        public string? IdentificationTypeFilter { get; set; }
        public string? DocumentInfoFilter { get; set; }
        public string? DeliveryTypeFilter { get; set; }
        public string? EmployeeFilter { get; set; }
        public string? StatusNameFilter { get; set; }
        public string? TotalOfPieces { get; set; }
        public string? UnitPieceFilter { get; set; }
        public string? TelephoneFilter { get; set; }
        public string? ContactFilter { get; set; } 
        public DateTime? TrackDate { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

    }

}

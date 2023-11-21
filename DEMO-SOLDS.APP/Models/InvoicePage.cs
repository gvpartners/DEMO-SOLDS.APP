using DocumentFormat.OpenXml.Spreadsheet;

namespace DEMO_SOLDS.APP.Models
{
    public class InvoicePage
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public FiltersInvoice? Filters { get; set; }
    }

    public class FiltersInvoice
    {
        public string? InvoiceCodeFilter { get; set; }
        public string? IdentificationInfoFilter { get; set; }
        public string? SelectedCategoryFilter { get; set; }
        public string? IdentificationTypeFilter { get; set; }
        public string? DocumentInfoFilter { get; set; }
        public string? TotalInvoiceFilter { get; set; }
        public string? DeliveryTypeFilter { get; set; }
        public string? EmployeeFilter { get; set; }
        public string? StatusNameFilter { get; set; }
        public string? TotalPriceParihuelaFilter { get; set; }
        public string? UnitPieceFilter { get; set; }
        public string? SelectedDistrictFilter { get; set; }
        public string? AddressFilter { get; set; }
        public string? ReferenceFilter { get; set; }
        public string? TelephoneFilter { get; set; }
        public string? ContactFilter { get; set; }        
    }

}

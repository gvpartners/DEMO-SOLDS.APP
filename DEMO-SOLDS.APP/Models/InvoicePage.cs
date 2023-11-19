using DocumentFormat.OpenXml.Spreadsheet;

namespace DEMO_SOLDS.APP.Models
{
    public class InvoicePage
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string FilterColumn { get; set; }
        public string FilterValue { get; set; }
        public Filters filters { get; set; }
    }

    public class Filters{
        public string InvoiceCodeFilter { get; set; }
        public string IdentificationInfoFilter { get; set; }


    }
}

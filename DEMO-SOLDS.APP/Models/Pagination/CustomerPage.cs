namespace DEMO_SOLDS.APP.Models.Pagination
{
    public class CustomerPage
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public FiltersCustomer? Filters { get; set; }
    }

    public class FiltersCustomer
    {
        public string CustomerNameFilter { get; set; }
        public string IdentificationTypeFilter { get; set; }
        public string IdentificationInfoFilter { get; set; }
    }
}

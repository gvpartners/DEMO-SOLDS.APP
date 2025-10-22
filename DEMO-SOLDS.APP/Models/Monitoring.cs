using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DEMO_SOLDS.APP.Models
{
    public class AuxMonitoringModel
    {
        public Guid Id { get; set; }
        public string? IdentificationType { get; set; }
        public string? DocumentInfo { get; set; }
        public string? IdentificationInfo { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public string? Contact { get; set; }
        public decimal Quantity { get; set; }
        public string? SelectedDistrict { get; set; }
        public string? SelectedCategory { get; set; }
        public int DaysToComplete { get; set; }
        public string? DeliveryType { get; set; }
        public DateTime? RequirementDate { get; set; }
        public DateTime? QuotedDate { get; set; }
        public string? Responsible { get; set; }
        public string? Executive { get; set; }
        public string? Segment { get; set; }
        public string? Address { get; set; }
        public string? Comment { get; set; }
        public Guid UserId { get; set; }
    }

    public class MonitoringModel
    {
        public Guid Id { get; set; }
        public string? MonitoringCode { get; set; }
        public string? IdentificationType { get; set; }
        public string? DocumentInfo { get; set; }
        public string? IdentificationInfo { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public string? Contact { get; set; }
        public decimal Quantity { get; set; }
        public string? SelectedDistrict { get; set; }
        public string? SelectedCategory { get; set; }
        public int DaysToComplete { get; set; }
        public string? DeliveryType { get; set; }
        public DateTime? RequirementDate { get; set; }
        public DateTime? QuotedDate { get; set; }
        public int? ResponseDays { get; set; }
        public int StatusOrder { get; set; }
        public string? StatusName { get; set; }
        public string? Responsible { get; set; }
        public string? Executive { get; set; }
        public string? Segment { get; set; }
        public string? Address { get; set; }
        public string? Comment { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public Guid UserId { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class MonitoringListResponse
    {
        public List<MonitoringModel> Monitorings { get; set; }
        public int Total { get; set; }
    }

    public class Monitoring
    {
        public Guid Id { get; set; }
        public string? MonitoringCode { get; set; }
        public string? IdentificationType { get; set; }
        public string? DocumentInfo { get; set; }
        public string? IdentificationInfo { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public string? Contact { get; set; }
        public decimal Quantity { get; set; }
        public string? SelectedDistrict { get; set; }
        public string? SelectedCategory { get; set; }
        public int DaysToComplete { get; set; }
        public string? DeliveryType { get; set; }
        public DateTime? RequirementDate { get; set; }
        public DateTime? QuotedDate { get; set; }
        public int? ResponseDays { get; set; }
        public int StatusOrder { get; set; }
        public string? StatusName { get; set; }
        public string? Responsible { get; set; }
        public string? Executive { get; set; }
        public string? Segment { get; set; }
        public string? Address { get; set; }
        public string? Comment { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public Guid UserId { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class MonitoringPage
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public MonitoringFilters? Filters { get; set; }
    }

    public class MonitoringFilters
    {
        public string? MonitoringCodeFilter { get; set; }
        public string? DocumentInfoFilter { get; set; }
        public string? ContactFilter { get; set; }
        public string? TelephoneFilter { get; set; }
        public string? QuantityFilter { get; set; }
        public string? SelectedDistrictFilter { get; set; }
        public string? SelectedCategoryFilter { get; set; }
        public string? DaysToCompleteFilter { get; set; }
        public string? DeliveryTypeFilter { get; set; }
        public string? StatusNameFilter { get; set; }
        public string? ResponsibleFilter { get; set; }
        public string? ExecutiveFilter { get; set; }
        public string? SegmentFilter { get; set; }
        public DateTime? RequirementDateFilter { get; set; }
        public DateTime? QuotedDateFilter { get; set; }
        public string? SearchFilter { get; set; }
    }
}

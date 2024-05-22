using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DEMO_SOLDS.APP.Models
{
    
    public class AuxTrackModel
    {
        public Guid Id { get; set; }
        public string? TrackCode { get; set; }
        public string Contact { get; set; }
        public string? IdentificationType { get; set; }
        public string? DocumentInfo { get; set; }
        public string? IdentificationInfo { get; set; }
        public string? Telephone { get; set; }
        public string? SelectedCategory { get; set; }
        public List<string>? SelectedMeasures { get; set; }
        public List<decimal>? MeasureQuantities { get; set; }
        public string? DeliveryType { get; set; }
        public string SelectedClient { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string SelectedTruck { get; set; }
        public decimal? TotalWeight { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public int? StatusOrder { get; set; }
        public string StatusName { get; set; }
        public string Employee { get; set; }
        public string UnitPiece { get; set; }
        public Guid UserId { get; set; }
        public string? Comment { get; set; }
        public decimal? TotalOfPieces { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class TrackModel
    {
        public string? TrackCode { get; set; }
        public string? IdentificationType { get; set; }
        public string? DocumentInfo { get; set; }
        public string? IdentificationInfo { get; set; }
        public string? Telephone { get; set; }
        public string? SelectedCategory { get; set; }
        public List<string>? SelectedMeasures { get; set; }
        public List<decimal>? MeasureQuantities { get; set; }
        public string? DeliveryType { get; set; }
        public string Contact { get; set; }
        public string SelectedClient { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string SelectedTruck { get; set; }
        public decimal? TotalWeight { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public string UnitPiece { get; set; }
        public Guid UserId { get; set; }
        public decimal? TotalOfPieces { get; set; }
    }

    public class TrackListResponse
    {
        public List<AuxTrackModel> Tracks { get; set; }
        public int Total { get; set; }
    }

    public class Tracks
    {
        public Guid Id { get; set; }
        public string IdentificationType { get; set; }
        public string DocumentInfo { get; set; }
        public string IdentificationInfo { get; set; }
        public string Contact { get; set; }
        public string Telephone { get; set; }
        public string SelectedCategory { get; set; }
        public string SelectedClient { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string SelectedTruck { get; set; }
        public string SelectedMeasures { get; set; }
        public string MeasureQuantities { get; set; }
        public string DeliveryType { get; set; }
        public decimal? TotalWeight { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public int? StatusOrder { get; set; }
        public string StatusName { get; set; }
        public string TrackCode { get; set; }
        public string Employee { get; set; }
        public string UnitPiece { get; set; }
        public Guid UserId { get; set; }
        public string? Comment { get; set; }
        public decimal? TotalOfPieces { get; set; }
        public bool IsDeleted { get; set; }
    }
}

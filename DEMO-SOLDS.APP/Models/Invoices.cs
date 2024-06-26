﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DEMO_SOLDS.APP.Models
{
    public class TruckModel
    {
        public string? Truck { get; set; }
        public string? CountTruck { get; set; }
        public decimal TruckQuantity { get; set; }
        public decimal TruckPrice { get; set; }
    }
    public class ProductModel
    {
        public string? Product { get; set; }
        public decimal Quantity { get; set; }
        public decimal PriceUnit { get; set; }
        public decimal TotalPrice { get; set; }
    }
    public class AuxInvoiceModel
    {
        public Guid Id { get; set; }
        public string? IdentificationType { get; set; }
        public string? DocumentInfo { get; set; }
        public string? IdentificationInfo { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public string? CreatedBy { get; set; }
        public string? SelectedCategory { get; set; }
        public List<string>? SelectedMeasures { get; set; }
        public List<decimal>? MeasureQuantities { get; set; }
        public string? DeliveryType { get; set; }
        public string? SelectedDistrict { get; set; }
        public decimal Truck9TN { get; set; }
        public decimal Truck20TN { get; set; }
        public decimal Truck32TN { get; set; }
        public List<ProductModel> ?ProductsList { get; set; }
        public List<TruckModel>? FleteList { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IgvRate { get; set; }
        public decimal TotalInvoice { get; set; }
        public string? IsParihuelaNeeded { get; set; }
        public decimal CantParihuela { get; set; }
        public decimal CostParihuela { get; set; }
        public decimal TotalPriceParihuela { get; set; }
        public string? Address { get; set; }
        public decimal TotalOfPieces { get; set; }
        public string? UnitPiece { get; set; }
        public string? Contact { get; set; }
        public Guid UserId { get; set; }
        public string? Reference { get; set; }
        public string? Comment { get; set; }
        public string? DiscountApplies { get; set; }
        public decimal PercentageOfDiscount { get; set; }
        public string? IsOtherDistrict { get; set; }
        public decimal ManualTotalPriceFlete { get; set; }
    }

    public class InvoiceModel
    {
        public Guid Id { get; set; }
        public string? InvoiceCode { get; set; }
        public string? IdentificationType { get; set; }
        public string? DocumentInfo { get; set; }
        public string? IdentificationInfo { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public string? SelectedCategory { get; set; }
        public List<string>? SelectedMeasures { get; set; }
        public List<decimal>? MeasureQuantities { get; set; }
        public string? DeliveryType { get; set; }
        public string? SelectedDistrict { get; set; }
        public decimal Truck9TN { get; set; }
        public decimal Truck20TN { get; set; }
        public decimal Truck32TN { get; set; }
        public List<ProductModel>? ProductsList { get; set; }
        public List<TruckModel>? FleteList { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IgvRate { get; set; }
        public decimal TotalInvoice { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public int StatusOrder { get; set; }
        public string? StatusName { get; set; }
        public bool IsDeleted { get; set; }
        public string? IsParihuelaNeeded { get; set; }
        public decimal CantParihuela { get; set; }
        public decimal CostParihuela { get; set; }
        public decimal TotalPriceParihuela { get; set; }
        public string? Address { get; set; }
        public string? Employee { get; set; }
        public decimal TotalOfPieces { get; set; }
        public string? UnitPiece { get; set; }
        public string? Contact { get; set; }
        public Guid UserId { get; set; }
        public string? Reference { get; set; }
        public string? Comment { get; set; }
        public string? DiscountApplies { get; set; }
        public decimal PercentageOfDiscount { get; set; }
        public string? IsOtherDistrict { get; set; }
        public decimal ManualTotalPriceFlete { get; set; }
    }

    public class InvoiceListResponse
    {
        public List<InvoiceModel> Invoices { get; set; }
        public int Total { get; set; }
    }
    public class Invoices
    {
        public Guid Id { get; set; }
        public string? InvoiceCode { get; set; }
        public string? IdentificationType { get; set; }
        public string? DocumentInfo { get; set; }
        public string? IdentificationInfo { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public string? SelectedMeasures { get; set; }
        public string? SelectedCategory { get; set; }
        public string? MeasureQuantities { get; set; }
        public string? DeliveryType { get; set; }
        public string? SelectedDistrict { get; set; }
        public decimal Truck9TN { get; set; }
        public decimal Truck20TN { get; set; }
        public decimal Truck32TN { get; set; }
        public string? ProductsList { get; set; }
        public string? FleteList { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IgvRate { get; set; }
        public decimal TotalInvoice { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public int StatusOrder { get; set; }
        public string? StatusName { get; set; }
        public bool IsDeleted { get; set; }
        public string? IsParihuelaNeeded { get; set; }
        public decimal CantParihuela { get; set; }
        public decimal CostParihuela { get; set; }
        public decimal TotalPriceParihuela { get; set; }
        public string? Address { get; set; }
        public string? Employee { get; set; }
        public decimal TotalOfPieces { get; set; }
        public string? UnitPiece { get; set; }
        public string? Contact { get; set; }
        public Guid UserId { get; set; }
        public string? Reference { get; set; }
        public string? Comment { get; set; }
        public string? DiscountApplies { get; set; }
        public decimal PercentageOfDiscount { get; set; }
        public string? IsOtherDistrict { get; set; }
        public decimal ManualTotalPriceFlete { get; set; }
    }
}

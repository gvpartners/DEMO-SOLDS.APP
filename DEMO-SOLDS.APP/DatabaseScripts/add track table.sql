CREATE TABLE Track
(
    Id                uniqueidentifier NOT NULL,
    IdentificationType nvarchar(max) NULL,
    DocumentInfo      nvarchar(max) NULL,
    IdentificationInfo nvarchar(max) NULL,
    Contact           nvarchar(max) NULL,
    Telephone         nvarchar(max) NULL,
    SelectedCategory  nvarchar(max) NULL,
    SelectedClient    nvarchar(max) NULL,
    OrderDate         datetime NULL,
    DeliveryDate           datetime NULL,
    SelectedTruck     nvarchar(max) NULL,
    SelectedMeasures  nvarchar(max) NULL,
    MeasureQuantities nvarchar(max) NULL,
    DeliveryType      nvarchar(max) NULL,
    TotalWeight       decimal(20,4) NULL,
    CreatedBy         nvarchar(max) NULL,
    CreatedOn         datetime NULL,
    LastUpdatedBy     nvarchar(max) NULL,
    LastUpdatedOn     datetime NULL,
    StatusOrder       int NULL,
    StatusName        nvarchar(max) NULL,
    TrackCode       nvarchar(max) NULL,
    Employee          nvarchar(max) NULL,
    UnitPiece         nvarchar(max) NULL,
    UserId            uniqueidentifier NULL,
    Comment           nvarchar(max) NULL,
    TotalOfPieces     decimal(20,4) NULL,
    IsDeleted         bit NULL
);

--drop table Track
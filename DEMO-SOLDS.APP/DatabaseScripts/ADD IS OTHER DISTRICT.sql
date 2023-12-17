ALTER TABLE [Invoices]
ADD IsOtherDistrict NVARCHAR(256);

ALTER TABLE [Invoices]
ADD ManualTotalPriceFlete decimal(20,2) NULL DEFAULT 0;
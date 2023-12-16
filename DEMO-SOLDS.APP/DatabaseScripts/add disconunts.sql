-- Agregar columna DiscountApplies a la tabla Invoice
ALTER TABLE [Invoices]
ADD DiscountApplies NVARCHAR(256);

-- Agregar columna PercentageOfDiscount a la tabla Invoice con valor predeterminado 0
ALTER TABLE [Invoices]
ADD PercentageOfDiscount INT NULL DEFAULT 0;
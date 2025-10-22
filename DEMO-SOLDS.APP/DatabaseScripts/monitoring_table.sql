-- Create Monitoring Table
CREATE TABLE Monitoring (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MonitoringCode NVARCHAR(50) NOT NULL,
    IdentificationType NVARCHAR(10) NOT NULL, -- RUC, DNI
    DocumentInfo NVARCHAR(500) NOT NULL, -- Razón Social
    IdentificationInfo NVARCHAR(20) NOT NULL, -- RUC/DNI Number
    Telephone NVARCHAR(20),
    Email NVARCHAR(100),
    Contact NVARCHAR(200),
    Quantity DECIMAL(18,2),
    SelectedDistrict NVARCHAR(100),
    SelectedCategory NVARCHAR(100),
    DaysToComplete INT DEFAULT 0,
    DeliveryType NVARCHAR(50),
    RequirementDate DATETIME2,
    QuotedDate DATETIME2,
    ResponseDays INT DEFAULT 0,
    StatusOrder INT DEFAULT 1,
    StatusName NVARCHAR(50) DEFAULT 'En seguimiento',
    Responsible NVARCHAR(200),
    Executive NVARCHAR(200),
    Segment NVARCHAR(100),
    Address NVARCHAR(500),
    Comment NVARCHAR(MAX),
    CreatedBy NVARCHAR(200),
    CreatedOn DATETIME2 DEFAULT GETDATE(),
    LastUpdatedBy NVARCHAR(200),
    LastUpdatedOn DATETIME2 DEFAULT GETDATE(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    IsDeleted BIT DEFAULT 0,
    
    CONSTRAINT FK_Monitoring_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

-- Create Index for better performance
CREATE INDEX IX_Monitoring_UserId ON Monitoring(UserId);
CREATE INDEX IX_Monitoring_StatusOrder ON Monitoring(StatusOrder);
CREATE INDEX IX_Monitoring_CreatedOn ON Monitoring(CreatedOn);
CREATE INDEX IX_Monitoring_IdentificationInfo ON Monitoring(IdentificationInfo);

-- Insert default status options
INSERT INTO Monitoring (MonitoringCode, IdentificationType, DocumentInfo, IdentificationInfo, UserId, StatusOrder, StatusName)
VALUES 
('SEG-PREFIX-000001', 'RUC', 'Ejemplo Empresa S.A.C.', '20123456789', '00000000-0000-0000-0000-000000000000', 1, 'En seguimiento'),
('SEG-PREFIX-000002', 'DNI', 'Juan Pérez García', '12345678', '00000000-0000-0000-0000-000000000000', 2, 'Cotizado'),
('SEG-PREFIX-000003', 'RUC', 'Constructora ABC S.A.C.', '20987654321', '00000000-0000-0000-0000-000000000000', 3, 'Cerrado');

-- Update IsDeleted to true for example records
UPDATE Monitoring SET IsDeleted = 1 WHERE MonitoringCode LIKE 'SEG-PREFIX-%';

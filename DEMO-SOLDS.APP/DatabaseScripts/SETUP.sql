GO

CREATE DATABASE [DEMO.SOLD.APP]
GO

USE [DEMO.SOLD.APP]

GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[FirstLastName] [nvarchar](max) NOT NULL,
	[SecondLastName] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[Prefix] [nvarchar](max) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsApproved] [bit] NOT NULL,
	[Phone] [nvarchar](256) NULL,
	[IsAdmin] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customers]    Script Date: 3/12/2023 16:44:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
	[Id] [uniqueidentifier] NOT NULL,
	[IdentificationType] [varchar](max) NULL,
	[IdentificationInfo] [varchar](max) NULL,
	[IsDeleted] [bit] NULL,
	[CustomerName] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Invoices]    Script Date: 3/12/2023 16:44:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoices](
	[Id] [uniqueidentifier] NOT NULL,
	[IdentificationType] [nvarchar](50) NULL,
	[DocumentInfo] [nvarchar](50) NULL,
	[IdentificationInfo] [nvarchar](max) NULL,
	[Telephone] [nvarchar](20) NULL,
	[Email] [nvarchar](max) NULL,
	[SelectedCategory] [nvarchar](50) NULL,
	[SelectedMeasures] [nvarchar](max) NULL,
	[MeasureQuantities] [nvarchar](max) NULL,
	[DeliveryType] [nvarchar](50) NULL,
	[SelectedDistrict] [nvarchar](50) NULL,
	[Truck9TN] [int] NULL,
	[Truck20TN] [int] NULL,
	[Truck32TN] [int] NULL,
	[ProductsList] [nvarchar](max) NULL,
	[FleteList] [nvarchar](max) NULL,
	[TotalWeight] [decimal](20, 4) NULL,
	[Subtotal] [decimal](20, 4) NULL,
	[IgvRate] [decimal](20, 4) NULL,
	[TotalInvoice] [decimal](20, 4) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdatedBy] [nvarchar](max) NULL,
	[LastUpdatedOn] [datetime] NULL,
	[StatusOrder] [int] NULL,
	[StatusName] [nvarchar](max) NULL,
	[IsDeleted] [bit] NULL,
	[InvoiceCode] [nvarchar](max) NULL,
	[IsParihuelaNeeded] [nvarchar](256) NULL,
	[CantParihuela] [int] NULL,
	[CostParihuela] [decimal](20, 2) NULL,
	[TotalPriceParihuela] [decimal](20, 4) NULL,
	[Address] [nvarchar](max) NULL,
	[Employee] [nvarchar](256) NULL,
	[TotalOfPieces] [decimal](20, 4) NULL,
	[UnitPiece] [nvarchar](10) NULL,
	[Contact] [nvarchar](max) NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Reference] [nvarchar](max) NULL,
	[Comment] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[AspNetUsers] ([Id], [Name], [FirstLastName], [SecondLastName], [Email], [Password], [Prefix], [IsDeleted], [IsApproved], [Phone], [IsAdmin]) VALUES (N'638f1e8f-6f39-459c-b386-1e8ad0c883fc', N'Sebastian', N'García', N'Villacorta', N's.gvrodrigo@gmail.com', N'Dt2IVVdknkgeeQTKb8lIAH7U9hLlV7EOL8xt+VACbuU=', N'SGV', 0, 1, NULL, 1)
GO
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT (newid()) FOR [Id]
GO
USE [master]
GO
ALTER DATABASE [DEMO.SOLD.APP] SET  READ_WRITE 
GO

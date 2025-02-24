CREATE TABLE [dbo].[Visit](
	[Id] [uniqueidentifier] NOT NULL,
	[Client] [nvarchar](max) NOT NULL,
	[Work] [nvarchar](max) NOT NULL,
	[WorkAddress] [nvarchar](max) NOT NULL,
	[Contacts] [nvarchar](max) NOT NULL,
	[VisitReason] [nvarchar](max) NOT NULL,
	[CreatedBy] [nvarchar](max) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[VisitCode] [nvarchar](max) NULL,
	[StatusOrder] [int] NULL,
	[StatusName] [nvarchar](255) NULL,
	[Comment] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Visit] ADD  DEFAULT ((0)) FOR [StatusOrder]
GO



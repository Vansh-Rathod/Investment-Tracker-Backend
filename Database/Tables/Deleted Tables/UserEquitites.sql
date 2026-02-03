/****** Object:  Table [dbo].[UserEquities]    Script Date: 02-02-2026 11:22:27 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserEquities](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[EquityName] [nvarchar](max) NOT NULL,
	[EquityShortForm] [nvarchar](50) NULL,
	[EquityType] [int] NOT NULL,
	[PurchasePrice] [decimal](10, 2) NOT NULL,
	[Quantity] [decimal](10, 2) NOT NULL,
	[InvestedAmount] [decimal](10, 2) NOT NULL,
	[CurrentPrice] [decimal](10, 2) NOT NULL,
	[InvestmentDate] [datetime2](7) NOT NULL,
	[OrderId] [nvarchar](200) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CompanyName] [nvarchar](255) NULL,
 CONSTRAINT [PK_UserEquities_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_UserEquities_OrderId] UNIQUE NONCLUSTERED 
(
	[OrderId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserEquities] ADD  CONSTRAINT [DF_UserEquities_CreatedDate]  DEFAULT (sysutcdatetime()) FOR [CreatedDate]
GO

ALTER TABLE [dbo].[UserEquities] ADD  CONSTRAINT [DF_UserEquities_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[UserEquities] ADD  CONSTRAINT [DF_UserEquities_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[UserEquities]  WITH CHECK ADD  CONSTRAINT [FK_UserEquities_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[UserEquities] CHECK CONSTRAINT [FK_UserEquities_Users_UserId]
GO



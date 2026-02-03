/****** Object:  Table [dbo].[Transactions]    Script Date: 02-02-2026 11:19:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Transactions](
	[TransactionId] [int] IDENTITY(1,1) NOT NULL,
	[PortfolioId] [int] NOT NULL,
	[AssetTypeId] [int] NOT NULL,
	[AssetId] [int] NOT NULL,
	[TransactionType] [int] NOT NULL,
	[Units] [decimal](10, 2) NOT NULL,
	[Price] [decimal](10, 2) NOT NULL,
	[Amount] [decimal](10, 2) NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
	[SourceType] [nvarchar](200) NULL,
	[SourceId] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Transaction_Id] PRIMARY KEY CLUSTERED 
(
	[TransactionId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Transactions] ADD  DEFAULT (getdate()) FOR [TransactionDate]
GO

ALTER TABLE [dbo].[Transactions] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO



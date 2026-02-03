/****** Object:  Table [dbo].[MutualFundNAVHistory]    Script Date: 02-02-2026 11:17:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MutualFundNAVHistory](
	[FundId] [int] IDENTITY(1,1) NOT NULL,
	[NavDate] [datetime] NOT NULL,
	[NAV] [decimal](10, 2) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[FundId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[MutualFundNAVHistory] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO



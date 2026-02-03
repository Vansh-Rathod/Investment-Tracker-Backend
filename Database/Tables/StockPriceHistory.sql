/****** Object:  Table [dbo].[StockPriceHistory]    Script Date: 02-02-2026 11:19:18 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[StockPriceHistory](
	[StockId] [int] IDENTITY(1,1) NOT NULL,
	[PriceDate] [datetime] NOT NULL,
	[ClosePrice] [decimal](10, 2) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[StockId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[StockPriceHistory] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO



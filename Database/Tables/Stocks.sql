/****** Object:  Table [dbo].[Stocks]    Script Date: 02-02-2026 11:19:34 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Stocks](
	[StockId] [int] IDENTITY(1,1) NOT NULL,
	[StockName] [nvarchar](500) NOT NULL,
	[Symbol] [nvarchar](50) NOT NULL,
	[ExchangeId] [int] NOT NULL,
	[ISIN] [nvarchar](200) NULL,
	[IsActive] [bit] NOT NULL,
	[IsETF] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Stock_Id] PRIMARY KEY CLUSTERED 
(
	[StockId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Stocks] ADD  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[Stocks] ADD  DEFAULT ((0)) FOR [IsETF]
GO

ALTER TABLE [dbo].[Stocks] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO

ALTER TABLE [dbo].[Stocks]  WITH CHECK ADD  CONSTRAINT [FK_Stocks_Exchanges] FOREIGN KEY([ExchangeId])
REFERENCES [dbo].[Exchanges] ([ExchangeId])
GO

ALTER TABLE [dbo].[Stocks] CHECK CONSTRAINT [FK_Stocks_Exchanges]
GO



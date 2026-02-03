/****** Object:  Table [dbo].[MutualFunds]    Script Date: 02-02-2026 11:17:39 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MutualFunds](
	[FundId] [int] IDENTITY(1,1) NOT NULL,
	[AmcId] [int] NOT NULL,
	[FundName] [nvarchar](500) NOT NULL,
	[Category] [int] NULL,
	[ISIN] [nvarchar](200) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Fund_Id] PRIMARY KEY CLUSTERED 
(
	[FundId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[MutualFunds] ADD  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[MutualFunds] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO

ALTER TABLE [dbo].[MutualFunds]  WITH CHECK ADD  CONSTRAINT [FK_MutualFunds_AMCs] FOREIGN KEY([AmcId])
REFERENCES [dbo].[AMCs] ([AmcId])
GO

ALTER TABLE [dbo].[MutualFunds] CHECK CONSTRAINT [FK_MutualFunds_AMCs]
GO



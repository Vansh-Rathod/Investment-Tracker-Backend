/****** Object:  Table [dbo].[SIPs]    Script Date: 02-02-2026 11:19:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SIPs](
	[SipId] [int] IDENTITY(1,1) NOT NULL,
	[PortfolioId] [int] NOT NULL,
	[AssetTypeId] [int] NOT NULL,
	[AssetId] [int] NOT NULL,
	[SipAmount] [decimal](10, 2) NOT NULL,
	[Frequency] [int] NOT NULL,
	[SipDate] [datetime] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[Status] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Sip_Id] PRIMARY KEY CLUSTERED 
(
	[SipId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SIPs] ADD  DEFAULT ((1)) FOR [Frequency]
GO

ALTER TABLE [dbo].[SIPs] ADD  DEFAULT (getdate()) FOR [StartDate]
GO

ALTER TABLE [dbo].[SIPs] ADD  DEFAULT ((1)) FOR [Status]
GO

ALTER TABLE [dbo].[SIPs] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO



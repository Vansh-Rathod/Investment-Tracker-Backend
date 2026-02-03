/****** Object:  Table [dbo].[SipExecutions]    Script Date: 02-02-2026 11:18:10 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SipExecutions](
	[SipExecutionId] [int] IDENTITY(1,1) NOT NULL,
	[SipId] [int] NOT NULL,
	[PortfolioId] [int] NOT NULL,
	[AssetTypeId] [int] NOT NULL,
	[AssetId] [int] NOT NULL,
	[ScheduledDate] [datetime] NULL,
	[ExecutedDate] [datetime] NOT NULL,
	[SipAmount] [decimal](10, 2) NOT NULL,
	[NAVAtExecution] [decimal](10, 2) NOT NULL,
	[UnitsAllocated] [decimal](10, 2) NOT NULL,
	[OrderReference] [nvarchar](100) NULL,
	[ExecutionStatus] [int] NOT NULL,
	[FailureReason] [nvarchar](200) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_SipExecution_Id] PRIMARY KEY CLUSTERED 
(
	[SipExecutionId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SipExecutions] ADD  DEFAULT (getdate()) FOR [ExecutedDate]
GO

ALTER TABLE [dbo].[SipExecutions] ADD  DEFAULT ((1)) FOR [ExecutionStatus]
GO

ALTER TABLE [dbo].[SipExecutions] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO



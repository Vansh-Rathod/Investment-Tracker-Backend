-- ============================================================
-- Migration: Add UserId to Transactions, SIPs, SipExecutions
-- Purpose: Replace portfolio-based filtering with userId so
--          Transactions, Mutual Funds, Stocks, SIPs are fetched by userId.
-- Run this script on your database. Portfolios table/controller
-- can be excluded later; this migration does NOT drop Portfolios.
-- ============================================================

-- 1) Add UserId column to Transactions (nullable first for backfill)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Transactions') AND name = 'UserId')
BEGIN
    ALTER TABLE [dbo].[Transactions] ADD [UserId] INT NULL;
END
GO

-- Backfill Transactions.UserId from Portfolios
UPDATE t
SET t.UserId = p.UserId
FROM [dbo].[Transactions] t
INNER JOIN [dbo].[Portfolios] p ON t.PortfolioId = p.PortfolioId
WHERE t.UserId IS NULL;
GO

-- Make UserId NOT NULL and add FK
ALTER TABLE [dbo].[Transactions] ALTER COLUMN [UserId] INT NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Transactions_Users')
BEGIN
    ALTER TABLE [dbo].[Transactions] WITH CHECK ADD CONSTRAINT [FK_Transactions_Users]
    FOREIGN KEY([UserId]) REFERENCES [dbo].[Users] ([Id]);
END
GO

-- 2) Add UserId column to SIPs
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SIPs') AND name = 'UserId')
BEGIN
    ALTER TABLE [dbo].[SIPs] ADD [UserId] INT NULL;
END
GO

UPDATE s
SET s.UserId = p.UserId
FROM [dbo].[SIPs] s
INNER JOIN [dbo].[Portfolios] p ON s.PortfolioId = p.PortfolioId
WHERE s.UserId IS NULL;
GO

ALTER TABLE [dbo].[SIPs] ALTER COLUMN [UserId] INT NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SIPs_Users')
BEGIN
    ALTER TABLE [dbo].[SIPs] WITH CHECK ADD CONSTRAINT [FK_SIPs_Users]
    FOREIGN KEY([UserId]) REFERENCES [dbo].[Users] ([Id]);
END
GO

-- 3) Add UserId column to SipExecutions
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SipExecutions') AND name = 'UserId')
BEGIN
    ALTER TABLE [dbo].[SipExecutions] ADD [UserId] INT NULL;
END
GO

UPDATE se
SET se.UserId = s.UserId
FROM [dbo].[SipExecutions] se
INNER JOIN [dbo].[SIPs] s ON se.SipId = s.SipId
WHERE se.UserId IS NULL;
GO

ALTER TABLE [dbo].[SipExecutions] ALTER COLUMN [UserId] INT NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SipExecutions_Users')
BEGIN
    ALTER TABLE [dbo].[SipExecutions] WITH CHECK ADD CONSTRAINT [FK_SipExecutions_Users]
    FOREIGN KEY([UserId]) REFERENCES [dbo].[Users] ([Id]);
END
GO

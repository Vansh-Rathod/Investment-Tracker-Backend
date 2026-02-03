/****** Object:  StoredProcedure [dbo].[GetUserTransactions]    Script Date: 02-02-2026 11:33:10 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetUserTransactions]
(
    @UserId INT,                  -- Required
    @PortfolioId INT = 0,
    @AssetId INT = 0,              -- FundId, StockId
    @AssetTypeId INT = 0,          -- 0 = All, 1 = Stock, 2 = Mutual Fund
    @TransactionType INT = 0,      -- 0 = All (Buy / Sell)
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL
)
AS

--DECLARE 
--    @UserId INT = 1,                  -- Required
--    @PortfolioId INT = 0,
--    @AssetId INT = 0,              -- FundId, StockId
--    @AssetTypeId INT = 0,          -- 0 = All, 1 = Stock, 2 = Mutual Fund
--    @TransactionType INT = 0,      -- 0 = All (Buy / Sell)
--    @FromDate DATETIME = NULL,
--    @ToDate DATETIME = NULL 

BEGIN
    SET NOCOUNT ON;

    SELECT
        t.TransactionId,
        t.PortfolioId,
        pf.Name AS PortfolioName,
        pf.PortfolioType,
        at.Name AS PortfolioTypeName,

        t.AssetTypeId,
        CASE 
            WHEN t.AssetTypeId = 1 THEN 'Stock'
            WHEN t.AssetTypeId = 2 THEN 'Mutual Fund'
        END AS AssetTypeName,

        t.AssetId,

        -- Asset Name
        CASE 
            WHEN t.AssetTypeId = 1 THEN st.StockName
            WHEN t.AssetTypeId = 2 THEN mf.FundName
        END AS AssetName,

        -- MF extras
        amc.Name AS AMCName,
        ct.CategoryName,

        t.TransactionType,
        t.Units,
        t.Price,
        t.Amount,
        t.TransactionDate,

        t.SourceType,
        t.SourceId,

        t.CreatedDate,
        t.ModifiedDate

    FROM Transactions t

    INNER JOIN Portfolios pf
        ON t.PortfolioId = pf.PortfolioId

    INNER JOIN AssetTypes at
        ON pf.PortfolioType = at.AssetTypeId

    -- Mutual fund joins
    LEFT JOIN MutualFunds mf
        ON t.AssetTypeId = 2 AND t.AssetId = mf.FundId
    LEFT JOIN AMCs amc
        ON mf.AmcId = amc.AmcId
    LEFT JOIN Categories ct
        ON mf.Category = ct.CategoryId

    -- Stock joins
    LEFT JOIN Stocks st
        ON t.AssetTypeId = 1 AND t.AssetId = st.StockId

    WHERE
        pf.UserId = @UserId
    AND (ISNULL(@PortfolioId, 0) = 0 OR t.PortfolioId = @PortfolioId)
    AND (ISNULL(@AssetId, 0) = 0 OR t.AssetId = @AssetId)
    AND (ISNULL(@AssetTypeId, 0) = 0 OR t.AssetTypeId = @AssetTypeId)
    AND (ISNULL(@TransactionType, 0) = 0 OR t.TransactionType = @TransactionType)
    AND (@FromDate IS NULL OR t.TransactionDate >= @FromDate)
    AND (@ToDate IS NULL OR t.TransactionDate <= @ToDate)

    ORDER BY t.TransactionDate DESC;
END;
GO



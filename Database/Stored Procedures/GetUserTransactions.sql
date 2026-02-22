-- Fetches by UserId (portfolio removed).
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetUserTransactions]
(
    @UserId INT,
    @AssetId INT = 0,
    @AssetTypeId INT = 0,
    @TransactionType INT = 0,
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.TransactionId,
        t.UserId,
        t.AssetTypeId,
        CASE WHEN t.AssetTypeId = 1 THEN 'Stock' WHEN t.AssetTypeId = 2 THEN 'Mutual Fund' END AS AssetTypeName,
        t.AssetId,
        CASE WHEN t.AssetTypeId = 1 THEN st.StockName WHEN t.AssetTypeId = 2 THEN mf.FundName END AS AssetName,
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
    LEFT JOIN MutualFunds mf ON t.AssetTypeId = 2 AND t.AssetId = mf.FundId
    LEFT JOIN AMCs amc ON mf.AmcId = amc.AmcId
    LEFT JOIN Categories ct ON mf.Category = ct.CategoryId
    LEFT JOIN Stocks st ON t.AssetTypeId = 1 AND t.AssetId = st.StockId
    WHERE t.UserId = @UserId
      AND (ISNULL(@AssetId, 0) = 0 OR t.AssetId = @AssetId)
      AND (ISNULL(@AssetTypeId, 0) = 0 OR t.AssetTypeId = @AssetTypeId)
      AND (ISNULL(@TransactionType, 0) = 0 OR t.TransactionType = @TransactionType)
      AND (@FromDate IS NULL OR t.TransactionDate >= @FromDate)
      AND (@ToDate IS NULL OR t.TransactionDate <= @ToDate)
    ORDER BY t.TransactionDate DESC;
END;
GO

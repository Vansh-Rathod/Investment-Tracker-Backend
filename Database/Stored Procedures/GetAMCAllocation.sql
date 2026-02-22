-- Fetches by UserId (portfolio removed).
CREATE PROCEDURE [dbo].[GetAMCAllocation]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    WITH Holdings AS (
        SELECT 
            t.AssetTypeId,
            t.AssetId,
            SUM(CASE WHEN t.TransactionType = 1 THEN t.Units ELSE -t.Units END) AS NetUnits
        FROM Transactions t
        WHERE t.UserId = @UserId
        GROUP BY t.AssetTypeId, t.AssetId
        HAVING SUM(CASE WHEN t.TransactionType = 1 THEN t.Units ELSE -t.Units END) > 0
    ),
    Valuations AS (
        SELECT 
            h.AssetTypeId,
            h.AssetId,
            h.NetUnits * (SELECT TOP 1 Price FROM Transactions t2 
                          WHERE t2.AssetId = h.AssetId AND t2.AssetTypeId = h.AssetTypeId AND t2.UserId = @UserId
                          ORDER BY t2.TransactionDate DESC) AS CurrentValue
        FROM Holdings h
    ),
    TotalValue AS (
        SELECT SUM(CurrentValue) AS TotalPortfolioValue FROM Valuations
    )
    SELECT 
        ISNULL(a.AmcId, 0) AS Id,
        ISNULL(a.Name, CASE WHEN v.AssetTypeId = 1 THEN 'Direct Equity' ELSE 'Other' END) AS Name,
        SUM(v.CurrentValue) AS Value,
        CASE WHEN tv.TotalPortfolioValue > 0 THEN (SUM(v.CurrentValue) / tv.TotalPortfolioValue) * 100 ELSE 0 END AS Percentage
    FROM Valuations v
    LEFT JOIN MutualFunds mf ON v.AssetTypeId = 2 AND v.AssetId = mf.FundId
    LEFT JOIN AMCs a ON mf.AmcId = a.AmcId
    CROSS JOIN TotalValue tv
    GROUP BY a.AmcId, a.Name, v.AssetTypeId, tv.TotalPortfolioValue;
END
GO

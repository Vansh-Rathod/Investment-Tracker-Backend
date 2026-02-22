-- Fetches by UserId (portfolio removed). Uses AMCs table (Name), not AssetManagementCompanies.
ALTER PROCEDURE [dbo].[GetMutualFundHoldings]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    WITH Holdings AS (
        SELECT 
            mf.FundId,
            mf.FundName,
            amc.Name AS AMCCode,
            c.CategoryName,
            SUM(CASE WHEN t.TransactionType = 1 THEN t.Units ELSE -t.Units END) AS UnitsHeld,
            SUM(CASE WHEN t.TransactionType = 1 THEN t.Amount ELSE -t.Amount END) AS InvestedAmount
        FROM Transactions t
        JOIN MutualFunds mf ON t.AssetId = mf.FundId
        LEFT JOIN AMCs amc ON mf.AmcId = amc.AmcId
        LEFT JOIN Categories c ON mf.Category = c.CategoryId
        WHERE t.UserId = @UserId
          AND t.AssetTypeId = 2
        GROUP BY mf.FundId, mf.FundName, amc.Name, c.CategoryName
        HAVING SUM(CASE WHEN t.TransactionType = 1 THEN t.Units ELSE -t.Units END) > 0.001
    ),
    EnrichedHoldings AS (
        SELECT 
            h.FundId,
            h.FundName,
            h.AMCCode,
            h.CategoryName,
            h.UnitsHeld,
            (h.InvestedAmount / NULLIF(h.UnitsHeld, 0)) AS AverageNAV,
            h.InvestedAmount,
            COALESCE((SELECT TOP 1 Price FROM Transactions WHERE AssetId = h.FundId AND AssetTypeId = 2 AND UserId = @UserId ORDER BY TransactionDate DESC), (h.InvestedAmount / NULLIF(h.UnitsHeld, 0))) AS CurrentNAV 
        FROM Holdings h
    )
    SELECT 
        FundId,
        FundName,
        AMCCode,
        CategoryName,
        UnitsHeld,
        AverageNAV,
        CurrentNAV,
        InvestedAmount,
        (UnitsHeld * CurrentNAV) AS CurrentValue,
        ((UnitsHeld * CurrentNAV) - InvestedAmount) AS AbsoluteReturn,
        CASE WHEN InvestedAmount > 0 THEN (((UnitsHeld * CurrentNAV) - InvestedAmount) / InvestedAmount) * 100 ELSE 0 END AS AbsoluteReturnPercentage,
        0 AS XIRR
    FROM EnrichedHoldings;
END
GO

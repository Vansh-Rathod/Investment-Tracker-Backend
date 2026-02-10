CREATE PROCEDURE [dbo].[GetDashboardSummary]
    @UserId INT,
    @PortfolioId INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Summary TABLE (
        InvestedAmount DECIMAL(18, 2),
        CurrentValue DECIMAL(18, 2),
        RealizedReturns DECIMAL(18, 2)
    );

    -- Calculate Metrics per Asset
    WITH AssetMetrics AS (
        SELECT 
            t.AssetTypeId,
            t.AssetId,
            SUM(CASE WHEN t.TransactionType = 1 THEN t.Units ELSE 0 END) AS BuyUnits,
            SUM(CASE WHEN t.TransactionType = 1 THEN t.Amount ELSE 0 END) AS BuyAmount,
            SUM(CASE WHEN t.TransactionType = 2 THEN t.Units ELSE 0 END) AS SellUnits,
            SUM(CASE WHEN t.TransactionType = 2 THEN t.Amount ELSE 0 END) AS SellAmount
        FROM Transactions t
        JOIN Portfolios p ON t.PortfolioId = p.PortfolioId
        WHERE p.UserId = @UserId
          AND (@PortfolioId = 0 OR t.PortfolioId = @PortfolioId)
        GROUP BY t.AssetTypeId, t.AssetId
        HAVING SUM(CASE WHEN t.TransactionType = 1 THEN t.Units ELSE 0 END) > 0
    ),
    CalculatedMetrics AS (
        SELECT 
            AssetTypeId,
            AssetId,
            BuyUnits,
            BuyAmount,
            SellUnits,
            SellAmount,
            (BuyUnits - SellUnits) AS NetUnits,
            (BuyAmount / NULLIF(BuyUnits, 0)) AS AvgBuyPrice
        FROM AssetMetrics
    ),
    CurrentPrices AS (
        SELECT 
            cm.AssetTypeId,
            cm.AssetId,
            (SELECT TOP 1 Price FROM Transactions t2 
             WHERE t2.AssetId = cm.AssetId AND t2.AssetTypeId = cm.AssetTypeId 
             ORDER BY t2.TransactionDate DESC) AS LatestPrice
        FROM CalculatedMetrics cm
        WHERE cm.NetUnits > 0
    )
    INSERT INTO @Summary (InvestedAmount, CurrentValue, RealizedReturns)
    SELECT 
        ISNULL(SUM(cm.NetUnits * cm.AvgBuyPrice), 0),
        ISNULL(SUM(cm.NetUnits * cp.LatestPrice), 0),
        ISNULL(SUM(cm.SellAmount - (cm.SellUnits * cm.AvgBuyPrice)), 0)
    FROM CalculatedMetrics cm
    LEFT JOIN CurrentPrices cp ON cm.AssetTypeId = cp.AssetTypeId AND cm.AssetId = cp.AssetId;

    -- Return Result
    SELECT 
        InvestedAmount,
        CurrentValue,
        ((CurrentValue - InvestedAmount) + RealizedReturns) AS TotalReturns,
        CASE WHEN InvestedAmount > 0 THEN (((CurrentValue - InvestedAmount) + RealizedReturns) / InvestedAmount) * 100 ELSE 0 END AS AbsReturns,
        0 AS XIRR, -- Placeholder
        0 AS DayChange, -- Placeholder
        0 AS DayChangePercentage -- Placeholder
    FROM @Summary;
END
GO

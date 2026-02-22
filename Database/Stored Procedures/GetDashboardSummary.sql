-- Fetches by UserId (portfolio removed).
CREATE PROCEDURE [dbo].[GetDashboardSummary]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Summary TABLE (
        InvestedAmount DECIMAL(18, 2) DEFAULT 0,
        CurrentValue DECIMAL(18, 2) DEFAULT 0,
        RealizedReturns DECIMAL(18, 2) DEFAULT 0,
        StockInvestedAmount DECIMAL(18, 2) DEFAULT 0,
        StockCurrentValue DECIMAL(18, 2) DEFAULT 0,
        MFInvestedAmount DECIMAL(18, 2) DEFAULT 0,
        MFCurrentValue DECIMAL(18, 2) DEFAULT 0
    );

    WITH AssetMetrics AS (
        SELECT 
            t.AssetTypeId,
            t.AssetId,
            SUM(CASE WHEN t.TransactionType = 1 THEN t.Units ELSE 0 END) AS BuyUnits,
            SUM(CASE WHEN t.TransactionType = 1 THEN t.Amount ELSE 0 END) AS BuyAmount,
            SUM(CASE WHEN t.TransactionType = 2 THEN t.Units ELSE 0 END) AS SellUnits,
            SUM(CASE WHEN t.TransactionType = 2 THEN t.Amount ELSE 0 END) AS SellAmount
        FROM Transactions t
        WHERE t.UserId = @UserId
        GROUP BY t.AssetTypeId, t.AssetId
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
            CASE WHEN (BuyUnits - SellUnits) > 0 THEN (BuyAmount - SellAmount) ELSE 0 END AS NetInvestedAmount, -- Simplified logic
             -- More accurate Avg Price calculation usually involves FIFO, but for summary this might suffice or need improvement
            CASE WHEN BuyUnits > 0 THEN (BuyAmount / BuyUnits) ELSE 0 END AS AvgBuyPrice 
        FROM AssetMetrics
    ),
    CurrentPrices AS (
        SELECT 
            cm.AssetTypeId,
            cm.AssetId,
            COALESCE(
                (SELECT TOP 1 Price FROM Transactions t2 
                 WHERE t2.AssetId = cm.AssetId AND t2.AssetTypeId = cm.AssetTypeId AND t2.UserId = @UserId
                 ORDER BY t2.TransactionDate DESC), 
                 cm.AvgBuyPrice -- Fallback to AvgBuyPrice if no latest price found (e.g. no transactions)
            ) AS LatestPrice
        FROM CalculatedMetrics cm
        WHERE cm.NetUnits > 0
    )
    INSERT INTO @Summary (InvestedAmount, CurrentValue, RealizedReturns, StockInvestedAmount, StockCurrentValue, MFInvestedAmount, MFCurrentValue)
    SELECT 
        SUM(cm.NetUnits * cm.AvgBuyPrice) AS TotalInvested,
        SUM(cm.NetUnits * cp.LatestPrice) AS TotalCurrent,
        SUM(CASE WHEN cm.SellUnits > 0 THEN (cm.SellAmount - (cm.SellUnits * cm.AvgBuyPrice)) ELSE 0 END) AS RealizedReturns,
        
        -- Granular Stocks
        SUM(CASE WHEN cm.AssetTypeId = 1 THEN (cm.NetUnits * cm.AvgBuyPrice) ELSE 0 END) AS StockInvested,
        SUM(CASE WHEN cm.AssetTypeId = 1 THEN (cm.NetUnits * cp.LatestPrice) ELSE 0 END) AS StockCurrent,
        
        -- Granular Mutual Funds
        SUM(CASE WHEN cm.AssetTypeId = 2 THEN (cm.NetUnits * cm.AvgBuyPrice) ELSE 0 END) AS MFInvested,
        SUM(CASE WHEN cm.AssetTypeId = 2 THEN (cm.NetUnits * cp.LatestPrice) ELSE 0 END) AS MFCurrent
        
    FROM CalculatedMetrics cm
    JOIN CurrentPrices cp ON cm.AssetTypeId = cp.AssetTypeId AND cm.AssetId = cp.AssetId;

    -- Return Result
    SELECT 
        ISNULL(InvestedAmount, 0) AS InvestedAmount,
        ISNULL(CurrentValue, 0) AS CurrentValue,
        ISNULL(((CurrentValue - InvestedAmount) + RealizedReturns), 0) AS TotalReturns,
        CASE WHEN InvestedAmount > 0 THEN (((CurrentValue - InvestedAmount) + RealizedReturns) / InvestedAmount) * 100 ELSE 0 END AS AbsReturns,
        
        -- Granular
        ISNULL(StockInvestedAmount, 0) AS StockInvestedAmount,
        ISNULL(StockCurrentValue, 0) AS StockCurrentValue,
        ISNULL(MFInvestedAmount, 0) AS MFInvestedAmount,
        ISNULL(MFCurrentValue, 0) AS MFCurrentValue,

        0 AS XIRR, -- Placeholder
        0 AS DayChange, -- Placeholder
        0 AS DayChangePercentage -- Placeholder
    FROM @Summary;
END
GO

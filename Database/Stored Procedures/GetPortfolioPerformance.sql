CREATE PROCEDURE [dbo].[GetPortfolioPerformance]
    @UserId INT,
    @PortfolioId INT = 0,
    @Months INT = 6
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Date DATETIME = GETDATE();

    WITH AssetMetrics AS (
        SELECT 
            t.AssetTypeId,
            t.AssetId,
            SUM(CASE WHEN t.TransactionType = 1 THEN t.Units ELSE 0 END) AS BuyUnits,
            SUM(CASE WHEN t.TransactionType = 1 THEN t.Amount ELSE 0 END) AS BuyAmount,
            SUM(CASE WHEN t.TransactionType = 2 THEN t.Units ELSE 0 END) AS SellUnits
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
    SELECT 
        @Date AS Date,
        ISNULL(SUM(cm.NetUnits * cm.AvgBuyPrice), 0) AS InvestedAmount,
        ISNULL(SUM(cm.NetUnits * cp.LatestPrice), 0) AS CurrentValue
    FROM CalculatedMetrics cm
    LEFT JOIN CurrentPrices cp ON cm.AssetTypeId = cp.AssetTypeId AND cm.AssetId = cp.AssetId;
END
GO

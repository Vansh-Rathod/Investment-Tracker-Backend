-- Fetches by UserId (portfolio removed). Exchanges table uses Name, ExchangeId.
CREATE PROCEDURE [dbo].[GetStockHoldings]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    WITH Holdings AS (
        SELECT 
            s.StockId,
            s.StockName,
            s.Symbol,
            e.Name AS ExchangeName,
            SUM(CASE WHEN t.TransactionType = 1 THEN t.Units ELSE -t.Units END) AS Quantity,
            SUM(CASE WHEN t.TransactionType = 1 THEN t.Amount ELSE -t.Amount END) AS InvestedAmount
        FROM Transactions t
        JOIN Stocks s ON t.AssetId = s.StockId
        LEFT JOIN Exchanges e ON s.ExchangeId = e.ExchangeId
        WHERE t.UserId = @UserId
          AND t.AssetTypeId = 1
        GROUP BY s.StockId, s.StockName, s.Symbol, e.Name
        HAVING SUM(CASE WHEN t.TransactionType = 1 THEN t.Units ELSE -t.Units END) > 0.001
    ),
    EnrichedHoldings AS (
        SELECT 
            h.StockId,
            h.StockName,
            h.Symbol,
            h.ExchangeName,
            h.Quantity,
            (h.InvestedAmount / NULLIF(h.Quantity, 0)) AS AveragePrice,
            h.InvestedAmount,
            COALESCE((SELECT TOP 1 Price FROM Transactions WHERE AssetId = h.StockId AND AssetTypeId = 1 AND UserId = @UserId ORDER BY TransactionDate DESC), (h.InvestedAmount / NULLIF(h.Quantity, 0))) AS CurrentPrice
        FROM Holdings h
    )
    SELECT 
        StockId,
        StockName,
        Symbol,
        ExchangeName,
        Quantity,
        AveragePrice,
        CurrentPrice,
        InvestedAmount,
        (Quantity * CurrentPrice) AS CurrentValue,
        0 AS DayChange,
        0 AS DayChangePercentage,
        ((Quantity * CurrentPrice) - InvestedAmount) AS TotalReturn,
        CASE WHEN InvestedAmount > 0 THEN (((Quantity * CurrentPrice) - InvestedAmount) / InvestedAmount) * 100 ELSE 0 END AS TotalReturnPercentage
    FROM EnrichedHoldings;
END
GO

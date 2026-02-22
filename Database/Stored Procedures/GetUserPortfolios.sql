CREATE PROCEDURE [dbo].[GetUserPortfolios]
    @UserId INT,
    @PortfolioId INT = 0,
    @PortfolioType INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    WITH PortfolioValues AS (
        SELECT 
            t.PortfolioId,
            SUM(CASE WHEN t.TransactionType = 1 THEN t.Amount ELSE -t.Amount END) AS InvestedAmount,
            -- Current Value needs fetch from current prices. Simplified here.
            SUM(
                CASE 
                    WHEN t.TransactionType = 1 THEN t.Units * (
                        COALESCE((SELECT TOP 1 Price FROM Transactions t2 WHERE t2.AssetId = t.AssetId AND t2.AssetTypeId = t.AssetTypeId ORDER BY t2.TransactionDate DESC), t.Price)
                    )
                    ELSE -t.Units * (
                        COALESCE((SELECT TOP 1 Price FROM Transactions t2 WHERE t2.AssetId = t.AssetId AND t2.AssetTypeId = t.AssetTypeId ORDER BY t2.TransactionDate DESC), t.Price)
                    )
                END
            ) AS CurrentValue
        FROM Transactions t
        JOIN Portfolios p ON t.PortfolioId = p.PortfolioId
        WHERE p.UserId = @UserId
        GROUP BY t.PortfolioId
    )
    SELECT 
        p.PortfolioId,
        p.Name AS PortfolioName,
        p.PortfolioType,
        CASE WHEN p.PortfolioType = 1 THEN 'Stock' WHEN p.PortfolioType = 2 THEN 'Mutual Fund' ELSE 'Unknown' END AS PortfolioTypeName,
        p.UserId,
        ISNULL(pv.CurrentValue, 0) AS CurrentValue,
        ISNULL(pv.CurrentValue - pv.InvestedAmount, 0) AS TotalReturns,
        p.CreatedDate,
        p.ModifiedDate
    FROM Portfolios p
    LEFT JOIN PortfolioValues pv ON p.PortfolioId = pv.PortfolioId
    WHERE p.UserId = @UserId
      AND (@PortfolioId = 0 OR p.PortfolioId = @PortfolioId)
      AND (@PortfolioType = 0 OR p.PortfolioType = @PortfolioType);
END
GO

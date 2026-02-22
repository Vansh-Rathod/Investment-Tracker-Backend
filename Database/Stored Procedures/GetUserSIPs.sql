-- Fetches by UserId (portfolio removed). SIPs table must have UserId column (see migration).
CREATE PROCEDURE [dbo].[GetUserSIPs]
    @UserId INT,
    @SipId INT = 0,
    @SipStatus INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        s.SipId,
        s.UserId,
        s.AssetTypeId,
        CASE WHEN s.AssetTypeId = 1 THEN 'Stock' WHEN s.AssetTypeId = 2 THEN 'Mutual Fund' ELSE 'Unknown' END AS AssetTypeName,
        s.AssetId,
        CASE WHEN s.AssetTypeId = 1 THEN st.StockName WHEN s.AssetTypeId = 2 THEN mf.FundName ELSE 'Unknown' END AS AssetName,
        CASE WHEN s.AssetTypeId = 2 THEN amc.Name ELSE NULL END AS AMCName,
        CASE WHEN s.AssetTypeId = 2 THEN c.CategoryName ELSE NULL END AS CategoryName,
        s.SipAmount,
        s.Frequency,
        s.SipDate,
        s.StartDate,
        s.EndDate,
        s.Status AS SipStatus,
        s.CreatedDate,
        s.ModifiedDate,
        NULL AS NextSipDate
    FROM SIPs s
    LEFT JOIN Stocks st ON s.AssetTypeId = 1 AND s.AssetId = st.StockId
    LEFT JOIN MutualFunds mf ON s.AssetTypeId = 2 AND s.AssetId = mf.FundId
    LEFT JOIN AMCs amc ON mf.AmcId = amc.AmcId
    LEFT JOIN Categories c ON mf.Category = c.CategoryId
    WHERE s.UserId = @UserId
      AND (@SipId = 0 OR s.SipId = @SipId)
      AND (@SipStatus = 0 OR s.Status = @SipStatus);
END
GO

/****** Object:  StoredProcedure [dbo].[GetUserSIPs]    Script Date: 02-02-2026 11:32:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetUserSIPs]
(
    @UserId INT,                 -- Required
    @SipId INT = 0,
    @PortfolioId INT = 0,
    @SipStatus INT = 0,            -- 0 = All, 1 = Active, 2 = Paused, 3 = Completed etc
    @PortfolioType INT = 0         -- 0 = All, 1 = Stock, 2 = Mututal Fund
)
AS

--DECLARE @UserId INT = 1      
-- ,@SipId INT = 0    
-- ,@PortfolioId INT = 0
-- ,@SipStatus INT = 0                -- 0 = All, 1 = Active, 2 = Paused, 3 = Completed etc
-- ,@PortfolioType INT = 0            -- 0 = All, 1 = Stock, 2 = Mututal Fund

BEGIN
    SET NOCOUNT ON;

    SELECT
        s.SipId,
        s.PortfolioId,
        pf.Name AS PortfolioName,
        pf.PortfolioType,
        at.Name AS PortfolioTypeName,

        s.AssetTypeId,
        CASE 
            WHEN s.AssetTypeId = 1 THEN 'Stock'
            WHEN s.AssetTypeId = 2 THEN 'Mutual Fund'
            ELSE 'Other'
        END AS AssetTypeName,

        s.AssetId,

        -- Asset Name resolution
        CASE 
            WHEN s.AssetTypeId = 1 THEN st.StockName
            WHEN s.AssetTypeId = 2 THEN mf.FundName
        END AS AssetName,

        -- Extra Mutual Fund info
        amc.Name AS AMCName,
        ct.CategoryName,

        s.SipAmount,
        s.Frequency,
        s.SipDate,
        s.StartDate,
        s.EndDate,
        s.Status AS SipStatus,
        s.CreatedDate,
        s.ModifiedDate

    FROM SIPs s

    INNER JOIN Portfolios pf 
        ON s.PortfolioId = pf.PortfolioId

    INNER JOIN AssetTypes at 
        ON pf.PortfolioType = at.AssetTypeId

    -- Mutual Fund joins
    LEFT JOIN MutualFunds mf 
        ON s.AssetTypeId = 2 AND s.AssetId = mf.FundId
    LEFT JOIN AMCs amc 
        ON mf.AmcId = amc.AmcId
    LEFT JOIN Categories ct 
        ON mf.Category = ct.CategoryId

    -- Stock joins
    LEFT JOIN Stocks st 
        ON s.AssetTypeId = 1 AND s.AssetId = st.StockId

    WHERE
        pf.UserId = @UserId
    AND (ISNULL(@SipId, 0) = 0 OR s.SipId = @SipId)
    AND (ISNULL(@PortfolioId, 0) = 0 OR s.PortfolioId = @PortfolioId)
    AND (ISNULL(@SipStatus, 0) = 0 OR s.Status = @SipStatus)
    AND (ISNULL(@PortfolioType, 0) = 0 OR pf.PortfolioType = @PortfolioType)

    ORDER BY s.CreatedDate DESC;
END;
GO



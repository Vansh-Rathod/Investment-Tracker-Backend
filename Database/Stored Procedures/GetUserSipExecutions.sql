-- Fetches SIP executions by UserId (via SIPs). Generic filters: @SipId, @ExecutionStatus, @FromDate, @ToDate.
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetUserSipExecutions]
    @UserId INT,
    @SipId INT = 0,
    @ExecutionStatus INT = 0,
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        se.SipExecutionId,
        se.SipId,
        se.AssetTypeId,
        CASE WHEN se.AssetTypeId = 1 THEN 'Stock' WHEN se.AssetTypeId = 2 THEN 'Mutual Fund' ELSE 'Unknown' END AS AssetTypeName,
        se.AssetId,
        CASE WHEN se.AssetTypeId = 1 THEN st.StockName WHEN se.AssetTypeId = 2 THEN mf.FundName ELSE NULL END AS AssetName,
        CASE WHEN se.AssetTypeId = 2 THEN amc.Name ELSE NULL END AS AMCName,
        CASE WHEN se.AssetTypeId = 2 THEN c.CategoryName ELSE NULL END AS CategoryName,
        se.SipAmount,
        se.ScheduledDate,
        se.ExecutedDate,
        se.NAVAtExecution,
        se.UnitsAllocated,
        se.OrderReference,
        se.ExecutionStatus,
        CASE 
            WHEN se.ExecutionStatus = 1 THEN 'Success'
            WHEN se.ExecutionStatus = 2 THEN 'Pending'
            WHEN se.ExecutionStatus = 3 THEN 'Failed'
            ELSE 'Unknown'
        END AS ExecutionStatusName,
        se.FailureReason,
        se.CreatedDate,
        se.ModifiedDate
    FROM SipExecutions se
    INNER JOIN SIPs s ON se.SipId = s.SipId
    LEFT JOIN Stocks st ON se.AssetTypeId = 1 AND se.AssetId = st.StockId
    LEFT JOIN MutualFunds mf ON se.AssetTypeId = 2 AND se.AssetId = mf.FundId
    LEFT JOIN AMCs amc ON mf.AmcId = amc.AmcId
    LEFT JOIN Categories c ON mf.Category = c.CategoryId
    WHERE s.UserId = @UserId
      AND (ISNULL(@SipId, 0) = 0 OR se.SipId = @SipId)
      AND (ISNULL(@ExecutionStatus, 0) = 0 OR se.ExecutionStatus = @ExecutionStatus)
      AND (@FromDate IS NULL OR se.ExecutedDate >= @FromDate)
      AND (@ToDate IS NULL OR se.ExecutedDate <= @ToDate)
    ORDER BY se.ExecutedDate DESC;
END
GO

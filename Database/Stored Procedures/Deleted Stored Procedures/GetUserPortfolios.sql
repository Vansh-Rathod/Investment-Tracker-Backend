/****** Object:  StoredProcedure [dbo].[GetUserPortfolios]    Script Date: 02-02-2026 11:30:49 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetUserPortfolios] (
	@PortfolioId INT = 0
	,@UserId INT
	,@PortfolioType INT = 0
	)
AS

--DECLARE @PortfolioId INT = 0    
-- ,@UserId INT = 1  
-- ,@PortfolioType INT = 0  
BEGIN
	SET NOCOUNT ON;

	SELECT pf.PortfolioId
		,pf.Name AS PortfolioName
		,pf.PortfolioType
		,at.Name AS PortfolioTypeName
		,pf.UserId
		,pf.CreatedDate
		,pf.ModifiedDate
	FROM Portfolios pf
	INNER JOIN AssetTypes at ON pf.PortfolioType = at.AssetTypeId
	WHERE pf.UserId = @UserId
		AND (
			ISNULL(@PortfolioId, 0) = 0
			OR pf.PortfolioId = @PortfolioId
			)
		AND (
			ISNULL(@PortfolioType, 0) = 0
			OR pf.PortfolioType = @PortfolioType
			)
	ORDER BY pf.CreatedDate ASC;
END;
GO



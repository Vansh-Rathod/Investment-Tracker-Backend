/****** Object:  StoredProcedure [dbo].[GetMutualFunds]    Script Date: 02-02-2026 11:25:17 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetMutualFunds] @FundId INT = 0
	,@AssetManagementCompanyId INT = 0
	,@FundName NVARCHAR(1000) = NULL
	,@CategoryId INT = 0 -- FlexiCap, LargeCap, MidCap, etc.  
	,@CategoryType INT = 0 -- 1 -> Equity, 2 -> Debt, 3 -> Hybrid, 4 -> Commodities 
	,@Page INT = 1
	,@PageSize INT = 20
	,@SearchText NVARCHAR(200) = ''
	,@SortColumn NVARCHAR(50) = 'FundName' -- FundName, CreatedDate
	,@SortOrder NVARCHAR(5) = 'ASC' -- ASC / DESC
AS

--DECLARE 
--@FundId INT = 0  
-- ,@AssetManagementCompanyId INT = 0  
-- ,@FundName NVARCHAR(1000) = ''  
-- ,@CategoryId INT = 0 -- FlexiCap, LargeCap, MidCap, etc.  
-- ,@CategoryType INT = 4 -- 1 -> Equity, 2 -> Debt, 3 -> Hybrid, 4 -> Commodities
--  ,@Page INT = 1
--    ,@PageSize INT = 10
--    ,@SearchText NVARCHAR(200) = ''
--    ,@SortColumn NVARCHAR(50) = 'FundName'  -- FundName, CreatedDate
--    ,@SortOrder NVARCHAR(5) = 'ASC'          -- ASC / DESC
BEGIN
	SET NOCOUNT ON;

	DECLARE @Offset INT = (@Page - 1) * @PageSize;

	-- Validate SortColumn
	IF @SortColumn NOT IN (
			'FundName'
			,'CreatedDate'
			)
		SET @SortColumn = 'FundName';

	-- Validate SortOrder
	IF UPPER(@SortOrder) NOT IN (
			'ASC'
			,'DESC'
			,'asc'
			,'desc'
			)
		SET @SortOrder = 'asc';
	-- Normalize
	SET @SortColumn = LOWER(@SortColumn);
	SET @SortOrder = LOWER(@SortOrder);

	/* ---- Total Records ---- */
	SELECT COUNT(1) AS TotalRecords
	INTO #TotalCount
	FROM MutualFunds mf
	LEFT JOIN Categories ct ON mf.Category = ct.CategoryId
	WHERE (
			ISNULL(@FundId, 0) = 0
			OR mf.FundId = @FundId
			)
		AND (
			ISNULL(@AssetManagementCompanyId, 0) = 0
			OR mf.AmcId = @AssetManagementCompanyId
			)
		AND (
			ISNULL(@CategoryId, 0) = 0
			OR mf.Category = @CategoryId
			)
		AND (
			ISNULL(@CategoryType, 0) = 0
			OR ct.CategoryType = @CategoryType
			)
		AND (
			ISNULL(@SearchText, '') = ''
			OR mf.FundName LIKE '%' + @SearchText + '%'
			OR mf.ISIN LIKE '%' + @SearchText + '%'
			);

	/* ---- Main Result ---- */
	SELECT mf.FundId
		,mf.FundName
		,mf.AmcId
		,amc.Name AS AssetManagementCompanyName
		,mf.Category AS CategoryId
		,ct.CategoryName
		,ct.CategoryType
		,CASE ct.CategoryType
			WHEN 1
				THEN 'Equity'
			WHEN 2
				THEN 'Debt'
			WHEN 3
				THEN 'Hybrid'
			WHEN 4
				THEN 'Commodities'
			ELSE 'Other'
			END AS CategoryTypeName
		,mf.CreatedDate
		,mf.ModifiedDate
		,tc.TotalRecords
	FROM MutualFunds mf
	LEFT JOIN AMCs amc ON mf.AmcId = amc.AmcId
	LEFT JOIN Categories ct ON mf.Category = ct.CategoryId
	CROSS JOIN #TotalCount tc
	WHERE (
			ISNULL(@FundId, 0) = 0
			OR mf.FundId = @FundId
			)
		AND (
			ISNULL(@AssetManagementCompanyId, 0) = 0
			OR mf.AmcId = @AssetManagementCompanyId
			)
		AND (
			ISNULL(@CategoryId, 0) = 0
			OR mf.Category = @CategoryId
			)
		AND (
			ISNULL(@CategoryType, 0) = 0
			OR ct.CategoryType = @CategoryType
			)
		AND (
			ISNULL(@SearchText, '') = ''
			OR mf.FundName LIKE '%' + @SearchText + '%'
			OR mf.ISIN LIKE '%' + @SearchText + '%'
			)
	ORDER BY CASE 
			WHEN LOWER(@SortColumn) = 'fundname'
				AND LOWER(@SortOrder) = 'asc'
				THEN mf.FundName
			END ASC
		,CASE 
			WHEN LOWER(@SortColumn) = 'fundname'
				AND LOWER(@SortOrder) = 'desc'
				THEN mf.FundName
			END DESC
		,CASE 
			WHEN LOWER(@SortColumn) = 'createddate'
				AND LOWER(@SortOrder) = 'asc'
				THEN mf.CreatedDate
			END ASC
		,CASE 
			WHEN LOWER(@SortColumn) = 'createddate'
				AND LOWER(@SortOrder) = 'desc'
				THEN mf.CreatedDate
			END DESC OFFSET @Offset ROWS

	FETCH NEXT @PageSize ROWS ONLY;

	DROP TABLE #TotalCount;
END
GO



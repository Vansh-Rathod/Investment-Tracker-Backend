/****** Object:  StoredProcedure [dbo].[GetStocks]    Script Date: 02-02-2026 11:27:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetStocks]
(
    @StockId INT = 0,
    @Page INT = 1,
    @PageSize INT = 20,
    @SearchText NVARCHAR(200) = '',
    @SortColumn NVARCHAR(50) = 'StockName', -- StockName, Symbol, CreatedDate
    @SortOrder NVARCHAR(5) = 'ASC',         -- ASC / DESC
    @IsETF BIT = NULL,
    @ExchangeId INT = 0
)
AS

--DECLARE @StockId INT = 0
--	,@Page INT = 1
--	,@PageSize INT = 20
--	,@SearchText NVARCHAR(200) = 'itc'
--	,@SortColumn NVARCHAR(50) = 'StockName'
--	,@SortOrder NVARCHAR(5) = 'ASC'
--	,@IsETF BIT = NULL
--	,@ExchangeId INT = 0

BEGIN
	SET NOCOUNT ON;

	DECLARE @Offset INT = (@Page - 1) * @PageSize;

	-- Validate SortColumn
	IF @SortColumn NOT IN (
			'StockName'
			,'Symbol'
			,'CreatedDate'
			)
		SET @SortColumn = 'StockName';

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

	-- Total Records
	SELECT COUNT(1) AS TotalRecords
	INTO #TotalCount
	FROM Stocks st
	WHERE (
			ISNULL(@StockId, 0) = 0
			OR st.StockId = @StockId
			)
		AND (
			ISNULL(@SearchText, '') = ''
			OR st.StockName LIKE '%' + @SearchText + '%'
			OR st.Symbol LIKE '%' + @SearchText + '%'
			OR st.ISIN LIKE '%' + @SearchText + '%'
			)
		AND (
			@IsETF IS NULL
			OR st.IsETF = @IsETF
			)
		AND (
			ISNULL(@ExchangeId, 0) = 0
			OR st.ExchangeId = @ExchangeId
			);

	-- Main Data Query
	SELECT st.StockId
		,st.StockName
		,st.Symbol
		,st.ISIN
		,st.IsActive
		,st.IsETF
		,st.ExchangeId
		,ex.Name AS ExchangeName
		,ex.ShortName AS ExchangeSymbol
		,st.CreatedDate
		,st.ModifiedDate
		,tc.TotalRecords
	FROM Stocks st
	LEFT JOIN Exchanges ex ON st.ExchangeId = ex.ExchangeId
	CROSS JOIN #TotalCount tc
	WHERE (
			ISNULL(@StockId, 0) = 0
			OR st.StockId = @StockId
			)
		AND (
			ISNULL(@SearchText, '') = ''
			OR st.StockName LIKE '%' + @SearchText + '%'
			OR st.Symbol LIKE '%' + @SearchText + '%'
			OR st.ISIN LIKE '%' + @SearchText + '%'
			)
		AND (
			@IsETF IS NULL
			OR st.IsETF = @IsETF
			)
		AND (
			ISNULL(@ExchangeId, 0) = 0
			OR st.ExchangeId = @ExchangeId
			)
	ORDER BY CASE 
			WHEN LOWER(@SortColumn) = 'stockname'
				AND LOWER(@SortOrder) = 'asc'
				THEN st.StockName
			END ASC
		,CASE 
			WHEN LOWER(@SortColumn) = 'stockname'
				AND LOWER(@SortOrder) = 'desc'
				THEN st.StockName
			END DESC
		,CASE 
			WHEN LOWER(@SortColumn) = 'symbol'
				AND LOWER(@SortOrder) = 'asc'
				THEN st.Symbol
			END ASC
		,CASE 
			WHEN LOWER(@SortColumn) = 'symbol'
				AND LOWER(@SortOrder) = 'desc'
				THEN st.Symbol
			END DESC
		,CASE 
			WHEN LOWER(@SortColumn) = 'createddate'
				AND LOWER(@SortOrder) = 'asc'
				THEN st.CreatedDate
			END ASC
		,CASE 
			WHEN LOWER(@SortColumn) = 'createddate'
				AND LOWER(@SortOrder) = 'desc'
				THEN st.CreatedDate
			END DESC OFFSET @Offset ROWS

	FETCH NEXT @PageSize ROWS ONLY;

	DROP TABLE #TotalCount;
END;
GO



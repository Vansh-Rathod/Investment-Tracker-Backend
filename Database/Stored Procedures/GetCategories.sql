/****** Object:  StoredProcedure [dbo].[GetCategories]    Script Date: 02-02-2026 11:24:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetCategories] @CategoryId INT = 0
 ,@CategoryType INT = 0 -- 1 -> Equity, 2 -> Debt, 3 -> Hybrid, 4 -> Commodities 
 ,@CategoryName NVARCHAR(200) = NULL
AS    
--DECLARE @CategoryId INT = 0
--	,@CategoryType INT = 0 -- 1 -> Equity, 2 -> Debt, 3 -> Hybrid, 4 -> Commodities 
--	,@CategoryName NVARCHAR(200) = 'mid'

BEGIN
	SET NOCOUNT ON;

	SELECT ct.CategoryId
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
		,ct.CategoryName
		,ct.CategoryDescription
		,ct.CreatedDate
		,ct.ModifiedDate
	FROM Categories ct
	WHERE (
			ISNULL(@CategoryId, 0) = 0
			OR @CategoryId = ct.CategoryId
			)
		AND (
			ISNULL(@CategoryType, 0) = 0
			OR @CategoryType = ct.CategoryType
			)
		AND (
			ISNULL(@CategoryName, '') = ''
			OR ct.CategoryName LIKE '%' + @CategoryName + '%'
			)
	ORDER BY ct.ModifiedDate DESC
		,ct.CreatedDate DESC
END
GO



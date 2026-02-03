/****** Object:  StoredProcedure [dbo].[GetAssetManagementCompanies]    Script Date: 02-02-2026 11:23:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetAssetManagementCompanies] @AssetManagementCompanyId INT = 0
 ,@AssetManagementCompanyName NVARCHAR(1000) = NULL
AS    
--DECLARE @AssetManagementCompanyId INT = 0
--	,@AssetManagementCompanyName NVARCHAR(1000) = 'hdfc'

BEGIN
	SET NOCOUNT ON;

	SELECT amc.AmcId
		,amc.Name
		,amc.CreatedDate
		,amc.ModifiedDate
	FROM AMCs amc
	WHERE (
			ISNULL(@AssetManagementCompanyId, 0) = 0
			OR @AssetManagementCompanyId = amc.AmcId
			)
		AND (
			ISNULL(@AssetManagementCompanyName, '') = ''
			OR amc.Name LIKE '%' + @AssetManagementCompanyName + '%'
			)
	ORDER BY amc.ModifiedDate DESC
		,amc.CreatedDate DESC
END
GO



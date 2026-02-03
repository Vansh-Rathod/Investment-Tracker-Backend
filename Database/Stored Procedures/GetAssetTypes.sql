/****** Object:  StoredProcedure [dbo].[GetAssetTypes]    Script Date: 02-02-2026 11:23:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetAssetTypes] @AssetTypeId INT = 0  
 ,@AssetName NVARCHAR(200) = NULL  
AS  
  
--DECLARE  
--@AssetTypeId INT = 0  
--,@AssetName NVARCHAR(200) = ''  
  
BEGIN  
 SET NOCOUNT ON;  
  
 SELECT at.AssetTypeId  
  ,at.Name  
  ,at.CreatedDate  
  ,at.ModifiedDate  
 FROM AssetTypes at  
 WHERE (  
   ISNULL(@AssetTypeId, 0) = 0  
   OR @AssetTypeId = at.AssetTypeId  
   )  
  AND (  
   ISNULL(@AssetName, '') = ''  
   OR @AssetName = at.Name  
   )  
 ORDER BY at.ModifiedDate DESC  
END
GO



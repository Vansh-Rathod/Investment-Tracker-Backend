/****** Object:  StoredProcedure [dbo].[GetExchanges]    Script Date: 02-02-2026 11:24:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetExchanges] @ExchangeId INT = 0    
 ,@ExchangeName NVARCHAR(400) = NULL    
AS    
    
--DECLARE    
--@ExchangeId INT = 0    
--,@ExchangeName NVARCHAR(400) = ''    
    
BEGIN    
 SET NOCOUNT ON;    
    
 SELECT ex.ExchangeId    
  ,ex.Name
  ,ex.ShortName 
  ,ex.CreatedDate    
  ,ex.ModifiedDate    
 FROM Exchanges ex    
 WHERE (    
   ISNULL(@ExchangeId, 0) = 0    
   OR @ExchangeId = ex.ExchangeId    
   )    
  AND (    
   ISNULL(@ExchangeName, '') = ''    
   OR @ExchangeName = ex.Name    
   )    
 ORDER BY ex.ModifiedDate DESC, ex.CreatedDate DESC   
END
GO



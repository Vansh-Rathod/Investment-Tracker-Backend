/****** Object:  StoredProcedure [dbo].[InsertUpdateDeleteUserEquity]    Script Date: 02-02-2026 11:33:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[InsertUpdateDeleteUserEquity]  
(    
    @OperationType INT,  
    @Id INT,  
    @UserId INT,    
    
    @EquityName NVARCHAR(MAX) = NULL,    
    @EquityShortForm NVARCHAR(100) = NULL,  
    @EquityType INT = NULL,  
    @PurchasePrice DECIMAL(10,2) = NULL,    
    @Quantity DECIMAL(10,2) = NULL,    
    @InvestedAmount DECIMAL(10,2) = NULL,    
    @CurrentPrice DECIMAL(10,2) = NULL,  
    @InvestmentDate DATETIME2 = NULL,  
    @OrderId NVARCHAR(MAX) = NULL,    
    @CompanyName NVARCHAR(256) = NULL,  
    
    @IsActive BIT = 1    
)    
AS    
BEGIN    
    SET NOCOUNT ON;    
    
    -- =========================    
    -- INSERT USER EQUITY   
    -- =========================    
    IF @OperationType = 1    
    BEGIN    
        INSERT INTO UserEquities    
        (    
            UserId,    
            EquityName,    
            EquityShortForm,    
            EquityType,    
            PurchasePrice,    
            Quantity,    
            InvestedAmount,    
            CurrentPrice,  
            InvestmentDate,  
            OrderId,  
            CreatedDate,  
            IsActive,  
            IsDeleted,  
            CompanyName  
        )    
        VALUES    
        (    
            @UserId,    
            @EquityName,    
            @EquityShortForm,    
            @EquityType,    
            @PurchasePrice,    
            @Quantity,    
            @InvestedAmount,  
            @CurrentPrice,  
            @InvestmentDate,  
            @OrderId,  
            GETUTCDATE(),  
            @IsActive,  
            0,  
            @CompanyName  
        );    
    
        SELECT SCOPE_IDENTITY() AS Id;    
        RETURN;    
    END    
    
    -- =========================    
    -- UPDATE USER EQUITY    
    -- =========================    
    IF @OperationType = 2    
    BEGIN    
        UPDATE UserEquities    
        SET    
            EquityName = @EquityName,   
            EquityShortForm = @EquityShortForm,
            EquityType = @EquityType,
            PurchasePrice = @PurchasePrice,    
            Quantity = @Quantity,    
            InvestedAmount = @InvestedAmount,    
            CurrentPrice = @CurrentPrice,  
            InvestmentDate = @InvestmentDate,  
            OrderId = @OrderId,  
            ModifiedDate = GETUTCDATE(),    
            IsActive = @IsActive,    
            CompanyName = @CompanyName  
        WHERE  
            Id = @Id  
            AND UserId = @UserId    
            AND IsDeleted = 0;    
    
        SELECT @Id AS Id;    
        RETURN;    
    END    
    
    -- =========================    
    -- SOFT DELETE USER EQUITY   
    -- =========================    
    IF @OperationType = 3    
    BEGIN    
        UPDATE UserEquities    
        SET    
            IsDeleted = 1,    
            IsActive = 0,    
            ModifiedDate = GETUTCDATE()    
        WHERE    
            Id = @Id  
            AND UserId = @UserId  
            AND IsDeleted = 0;    
    
        SELECT @Id AS Id;    
        RETURN;    
    END    
END 
GO



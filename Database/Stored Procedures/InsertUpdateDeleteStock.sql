CREATE PROCEDURE [dbo].[InsertUpdateDeleteStock]
(
    @OperationType INT,
    @StockId INT = 0,
    @StockName NVARCHAR(500),
    @Symbol NVARCHAR(50),
    @ExchangeId INT,
    @ISIN NVARCHAR(200) = NULL,
    @IsActive BIT = 1,
    @IsETF BIT = 0
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @OperationType = 1
    BEGIN
        INSERT INTO Stocks (StockName, Symbol, ExchangeId, ISIN, IsActive, IsETF, CreatedDate)
        VALUES (@StockName, @Symbol, @ExchangeId, @ISIN, @IsActive, @IsETF, GETDATE());
        SELECT SCOPE_IDENTITY() AS StockId;
    END
    ELSE IF @OperationType = 2
    BEGIN
        UPDATE Stocks
        SET StockName = @StockName,
            Symbol = @Symbol,
            ExchangeId = @ExchangeId,
            ISIN = @ISIN,
            IsActive = @IsActive,
            IsETF = @IsETF,
            ModifiedDate = GETDATE()
        WHERE StockId = @StockId;
        SELECT @StockId AS StockId;
    END
    ELSE IF @OperationType = 3
    BEGIN
        -- Soft Delete
        UPDATE Stocks
        SET IsActive = 0,
            ModifiedDate = GETDATE()
        WHERE StockId = @StockId;
        SELECT @StockId AS StockId;
    END
END
GO

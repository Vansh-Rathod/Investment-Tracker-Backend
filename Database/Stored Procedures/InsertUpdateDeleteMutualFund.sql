CREATE PROCEDURE [dbo].[InsertUpdateDeleteMutualFund]
(
    @OperationType INT,
    @FundId INT = 0,
    @AmcId INT,
    @FundName NVARCHAR(500),
    @Category INT = NULL,
    @ISIN NVARCHAR(200) = NULL,
    @IsActive BIT = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @OperationType = 1
    BEGIN
        INSERT INTO MutualFunds (AmcId, FundName, Category, ISIN, IsActive, CreatedDate)
        VALUES (@AmcId, @FundName, @Category, @ISIN, @IsActive, GETDATE());
        SELECT SCOPE_IDENTITY() AS FundId;
    END
    ELSE IF @OperationType = 2
    BEGIN
        UPDATE MutualFunds
        SET AmcId = @AmcId,
            FundName = @FundName,
            Category = @Category,
            ISIN = @ISIN,
            IsActive = @IsActive,
            ModifiedDate = GETDATE()
        WHERE FundId = @FundId;
        SELECT @FundId AS FundId;
    END
    ELSE IF @OperationType = 3
    BEGIN
        -- Soft Delete
        UPDATE MutualFunds
        SET IsActive = 0,
            ModifiedDate = GETDATE()
        WHERE FundId = @FundId;
        SELECT @FundId AS FundId;
    END
END
GO

CREATE PROCEDURE [dbo].[InsertUpdateDeleteSIP]
(
    @OperationType INT,
    @SipId INT = 0,
    @PortfolioId INT,
    @AssetTypeId INT,
    @AssetId INT,
    @SipAmount DECIMAL(10, 2),
    @Frequency INT,
    @SipDate DATETIME,
    @StartDate DATETIME,
    @EndDate DATETIME = NULL,
    @Status INT = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @OperationType = 1
    BEGIN
        INSERT INTO SIPs (PortfolioId, AssetTypeId, AssetId, SipAmount, Frequency, SipDate, StartDate, EndDate, Status, CreatedDate)
        VALUES (@PortfolioId, @AssetTypeId, @AssetId, @SipAmount, @Frequency, @SipDate, @StartDate, @EndDate, @Status, GETDATE());
        SELECT SCOPE_IDENTITY() AS SipId;
    END
    ELSE IF @OperationType = 2
    BEGIN
        UPDATE SIPs
        SET PortfolioId = @PortfolioId,
            AssetTypeId = @AssetTypeId,
            AssetId = @AssetId,
            SipAmount = @SipAmount,
            Frequency = @Frequency,
            SipDate = @SipDate,
            StartDate = @StartDate,
            EndDate = @EndDate,
            Status = @Status,
            ModifiedDate = GETDATE()
        WHERE SipId = @SipId;
        SELECT @SipId AS SipId;
    END
    ELSE IF @OperationType = 3
    BEGIN
        -- Soft Delete (Status 4 = Delete)
        UPDATE SIPs
        SET Status = 4,
            ModifiedDate = GETDATE()
        WHERE SipId = @SipId;
        SELECT @SipId AS SipId;
    END
END
GO

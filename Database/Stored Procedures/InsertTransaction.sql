-- Uses UserId instead of PortfolioId (portfolio feature removed for fetching by user).
CREATE PROCEDURE [dbo].[InsertTransaction]
(
    @UserId INT,
    @AssetTypeId INT,
    @AssetId INT,
    @TransactionType INT,
    @Units DECIMAL(10, 2),
    @Price DECIMAL(10, 2),
    @Amount DECIMAL(10, 2),
    @TransactionDate DATETIME,
    @SourceType NVARCHAR(200) = NULL,
    @SourceId INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Transactions (UserId, AssetTypeId, AssetId, TransactionType, Units, Price, Amount, TransactionDate, SourceType, SourceId, CreatedDate)
    VALUES (@UserId, @AssetTypeId, @AssetId, @TransactionType, @Units, @Price, @Amount, @TransactionDate, @SourceType, @SourceId, GETDATE());
    
    SELECT SCOPE_IDENTITY() AS TransactionId;
END
GO

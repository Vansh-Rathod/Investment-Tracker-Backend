CREATE PROCEDURE [dbo].[InsertUpdateDeletePortfolio]
(
    @OperationType INT,
    @PortfolioId INT = 0,
    @Name NVARCHAR(200),
    @UserId INT,
    @PortfolioType INT = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @OperationType = 1
    BEGIN
        INSERT INTO Portfolios (Name, UserId, PortfolioType, CreatedDate)
        VALUES (@Name, @UserId, @PortfolioType, GETDATE());
        SELECT SCOPE_IDENTITY() AS PortfolioId;
    END
    ELSE IF @OperationType = 2
    BEGIN
        UPDATE Portfolios
        SET Name = @Name,
            PortfolioType = @PortfolioType,
            ModifiedDate = GETDATE()
        WHERE PortfolioId = @PortfolioId;
        SELECT @PortfolioId AS PortfolioId;
    END
    ELSE IF @OperationType = 3
    BEGIN
        DELETE FROM Portfolios WHERE PortfolioId = @PortfolioId;
        SELECT @PortfolioId AS PortfolioId;
    END
END
GO

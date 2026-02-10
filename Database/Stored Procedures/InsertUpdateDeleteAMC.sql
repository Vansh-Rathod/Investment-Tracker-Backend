CREATE PROCEDURE [dbo].[InsertUpdateDeleteAMC]
(
    @OperationType INT, -- 1: Insert, 2: Update, 3: Delete
    @AmcId INT = 0,
    @Name NVARCHAR(500)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @OperationType = 1
    BEGIN
        INSERT INTO AMCs (Name, CreatedDate)
        VALUES (@Name, GETDATE());
        SELECT SCOPE_IDENTITY() AS AmcId;
    END
    ELSE IF @OperationType = 2
    BEGIN
        UPDATE AMCs
        SET Name = @Name,
            ModifiedDate = GETDATE()
        WHERE AmcId = @AmcId;
        SELECT @AmcId AS AmcId;
    END
    ELSE IF @OperationType = 3
    BEGIN
        DELETE FROM AMCs WHERE AmcId = @AmcId;
        SELECT @AmcId AS AmcId;
    END
END
GO

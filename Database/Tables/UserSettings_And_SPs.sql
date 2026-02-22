CREATE TABLE UserSettings (
    SettingId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    KeyName NVARCHAR(100) NOT NULL,
    Value NVARCHAR(MAX) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME NULL,
    CONSTRAINT FK_UserSettings_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

CREATE PROCEDURE [dbo].[GetUserSettings]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT KeyName, Value FROM UserSettings WHERE UserId = @UserId;
END
GO

CREATE PROCEDURE [dbo].[SaveUserSetting]
    @UserId INT,
    @KeyName NVARCHAR(100),
    @Value NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    MERGE UserSettings AS target
    USING (SELECT @UserId AS UserId, @KeyName AS KeyName) AS source
    ON (target.UserId = source.UserId AND target.KeyName = source.KeyName)
    WHEN MATCHED THEN
        UPDATE SET Value = @Value, ModifiedDate = GETDATE()
    WHEN NOT MATCHED THEN
        INSERT (UserId, KeyName, Value) VALUES (@UserId, @KeyName, @Value);
END
GO

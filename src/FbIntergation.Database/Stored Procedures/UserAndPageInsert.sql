CREATE PROCEDURE [Facebook].[InsertUserAndPage]
(
    @user_id INT,
    @app_scoped_user_id NVARCHAR(50),
    @access_token NVARCHAR(MAX),
    @expires_at DATETIME,
    @email NVARCHAR(50) = NULL,
    @first_name NVARCHAR(50) = NULL,
    @last_name NVARCHAR(50) = NULL,
    @pages [Facebook].[PageType] READONLY
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Insert into User table
    INSERT INTO [Facebook].[User] (
        [user_id],
        [app_scoped_user_id],
        [access_token],
        [expires_at],
        [email],
        [first_name],
        [last_name]
    )
    VALUES (
        @user_id,
        @app_scoped_user_id,
        @access_token,
        @expires_at,
        @email,
        @first_name,
        @last_name
    );

    -- Insert into Page table using the TVP
    INSERT INTO [Facebook].[Page] (
        [user_id],
        [page_id],
        [long_lived_access_token],
        [name]
    )
    SELECT
        SCOPE_IDENTITY() AS [user_id],
        p.[page_id],
        p.[long_lived_access_token],
        p.[name]
    FROM
        @pages p;
END;
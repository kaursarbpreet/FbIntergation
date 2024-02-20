CREATE TABLE [Facebook].[User]
(
	[id]                      [int]      IDENTITY(1,1) NOT NULL,
    [user_id]                 [int]                    NOT NULL,
    [app_scoped_user_id]      [nvarchar](50)           NOT NULL,
    [access_token]            [nvarchar](max)          NOT NULL,
    [expires_at]              [datetime]               NOT NULL,
    [created_at]              [datetime] CONSTRAINT [DF_User_created_at] DEFAULT (getdate()) NOT NULL,
    [updated_at]              [datetime] CONSTRAINT [DF_User_updated_at] DEFAULT (getdate()) NULL,
    [email]                   [nvarchar](50)           NULL,
    [first_name]              [nvarchar](50)           NULL,
    [last_name]               [nvarchar](50)           NULL,
    CONSTRAINT [PK_UserId] PRIMARY KEY CLUSTERED ([id] ASC)
)
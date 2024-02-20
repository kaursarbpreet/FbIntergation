CREATE TABLE [Facebook].[Page]
(
	[id]                      [int]      IDENTITY(1,1) NOT NULL,
    [user_id]                 [int]                    NOT NULL,
    [page_id]                 [nvarchar](50)           NOT NULL,
    [long_lived_access_token] [nvarchar](max)          NOT NULL,
    [name]                    [nvarchar](max)          NOT NULL,
    [created_at]              [datetime] CONSTRAINT [DF_Page_created_at] DEFAULT (getdate()) NOT NULL,
    [updated_at]              [datetime] CONSTRAINT [DF_Page_updated_at] DEFAULT (getdate()) NULL,
   
    CONSTRAINT [PK_PageId] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Page_User_userid] FOREIGN KEY ([user_id]) REFERENCES [Facebook].[User]([id])
)

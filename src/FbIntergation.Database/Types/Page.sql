CREATE TYPE [Facebook].[PageType] AS TABLE
(
    [page_id] NVARCHAR(50),
    [long_lived_access_token] NVARCHAR(MAX),
    [name] NVARCHAR(MAX)
);
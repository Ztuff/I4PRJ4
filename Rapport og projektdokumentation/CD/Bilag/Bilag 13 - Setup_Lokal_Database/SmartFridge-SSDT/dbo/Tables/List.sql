CREATE TABLE [dbo].[Lists] (
    [ListId] INT IDENTITY (1, 1) NOT NULL,
    [ListName] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [pk_List] PRIMARY KEY CLUSTERED ([ListId] ASC)
);


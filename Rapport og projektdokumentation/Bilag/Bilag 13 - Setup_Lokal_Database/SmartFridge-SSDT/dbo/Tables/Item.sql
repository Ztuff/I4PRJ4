CREATE TABLE [dbo].[Items] (
    [ItemId] INT IDENTITY (1, 1) NOT NULL,
    [ItemName] NVARCHAR(MAX) NOT NULL, 
    [StdVolume] INT NULL, 
    [StdUnit] NVARCHAR(MAX) NULL, 
    CONSTRAINT [pk_Item] PRIMARY KEY CLUSTERED ([ItemId] ASC)
);


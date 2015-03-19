CREATE TABLE [dbo].[Item] (
    [ItemId] INT IDENTITY (1, 1) NOT NULL,
    [ItemName] NVARCHAR(MAX) NOT NULL, 
    [StandardVolume] INT NULL, 
    [StandardUnit] NVARCHAR(MAX) NULL, 
    CONSTRAINT [pk_Item] PRIMARY KEY CLUSTERED ([ItemId] ASC)
);


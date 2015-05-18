﻿CREATE TABLE [dbo].[ListItems] (
    [ListId] INT NOT NULL,
    [ItemId] INT NOT NULL,
    [Amount] INT NOT NULL, 
    [Volume] INT NULL, 
    [Unit] NVARCHAR(MAX) NULL, 
    [ShelfLife] DATETIME NULL, 
    CONSTRAINT [pk_ListItems] PRIMARY KEY CLUSTERED ([ListId] ASC, [ItemId] ASC),
    CONSTRAINT [fk_ListItems] FOREIGN KEY ([ListId]) REFERENCES [dbo].[Lists] ([ListId]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [fk_ListItems2] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([ItemId]) ON DELETE CASCADE ON UPDATE CASCADE
);


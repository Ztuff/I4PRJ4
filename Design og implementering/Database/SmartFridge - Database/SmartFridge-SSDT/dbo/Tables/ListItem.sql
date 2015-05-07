CREATE TABLE [dbo].[ListItem] (
    [ListId] INT NOT NULL,
    [ItemId] INT NOT NULL,
    [Amount] INT NOT NULL, 
    [Volume] INT NULL, 
    [Unit] NVARCHAR(MAX) NULL, 
    [ShelfLife] DATETIME NULL, 
    CONSTRAINT [pk_ListItem] PRIMARY KEY CLUSTERED ([ListId] ASC, [ItemId] ASC),
    CONSTRAINT [fk_ListItem] FOREIGN KEY ([ListId]) REFERENCES [dbo].[List] ([ListId]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [fk_ListItem2] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Item] ([ItemId]) ON DELETE CASCADE ON UPDATE CASCADE
);


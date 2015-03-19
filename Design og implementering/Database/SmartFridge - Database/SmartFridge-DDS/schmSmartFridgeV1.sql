--
-- Target: Microsoft SQL Server 
-- Syntax: isql /Uuser /Ppassword /Sserver -i\path\filename.sql
-- Date  : Mar 05 2015 17:45
-- Script Generated by Database Design Studio 2.21.3 
--


--
-- Create Table    : 'List'   
-- ListId          :  
--
CREATE TABLE List (
    ListId         BIGINT IDENTITY(1,1) NOT NULL,
CONSTRAINT pk_List PRIMARY KEY CLUSTERED (ListId))
GO

--
-- Create Table    : 'Item'   
-- ItemId          :  
--
CREATE TABLE Item (
    ItemId         BIGINT IDENTITY(1,1) NOT NULL,
CONSTRAINT pk_Item PRIMARY KEY CLUSTERED (ItemId))
GO

--
-- Create Table    : 'ItemList'   
-- ListId          :  (references List.ListId)
-- ItemId          :  (references Item.ItemId)
--
CREATE TABLE ListItem (
    ListId         BIGINT NOT NULL,
    ItemId         BIGINT NOT NULL,
CONSTRAINT pk_ListItem PRIMARY KEY CLUSTERED (ListId,ItemId),
CONSTRAINT fk_ListItem FOREIGN KEY (ListId)
    REFERENCES List (ListId)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
CONSTRAINT fk_ListItem2 FOREIGN KEY (ItemId)
    REFERENCES Item (ItemId)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
GO


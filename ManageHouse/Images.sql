﻿CREATE TABLE [dbo].[Images] (
    [Id]  UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [URI] NVARCHAR (MAX)   NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

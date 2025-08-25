-- Crear tablas FileItems y FileShares manualmente
-- Primero eliminar las tablas si existen
IF OBJECT_ID('dbo.FileShares', 'U') IS NOT NULL
    DROP TABLE dbo.FileShares;

IF OBJECT_ID('dbo.FileItems', 'U') IS NOT NULL
    DROP TABLE dbo.FileItems;

-- Crear tabla FileItems
CREATE TABLE [dbo].[FileItems] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Path] nvarchar(500) NOT NULL,
    [Type] int NOT NULL,
    [Size] bigint NULL,
    [MimeType] nvarchar(100) NULL,
    [OwnerId] nvarchar(450) NOT NULL,
    [ClientId] nvarchar(450) NULL,
    [ParentFolderId] int NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_FileItems] PRIMARY KEY ([Id])
);

-- Crear tabla FileShares
CREATE TABLE [dbo].[FileShares] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FileItemId] int NOT NULL,
    [SharedWithUserId] nvarchar(450) NOT NULL,
    [SharedByUserId] nvarchar(450) NOT NULL,
    [Permission] int NOT NULL,
    [SharedAt] datetime2 NOT NULL,
    [ExpiresAt] datetime2 NULL,
    CONSTRAINT [PK_FileShares] PRIMARY KEY ([Id])
);

-- Crear índices
CREATE INDEX [IX_FileItems_OwnerId] ON [FileItems] ([OwnerId]);
CREATE INDEX [IX_FileItems_ClientId] ON [FileItems] ([ClientId]);
CREATE INDEX [IX_FileItems_Path] ON [FileItems] ([Path]);
CREATE INDEX [IX_FileItems_Type] ON [FileItems] ([Type]);
CREATE INDEX [IX_FileItems_ParentFolderId] ON [FileItems] ([ParentFolderId]);
CREATE INDEX [IX_FileShares_SharedWithUserId] ON [FileShares] ([SharedWithUserId]);
CREATE INDEX [IX_FileShares_FileItemId] ON [FileShares] ([FileItemId]);

-- Agregar claves foráneas con NO ACTION
ALTER TABLE [FileItems] ADD CONSTRAINT [FK_FileItems_AspNetUsers_OwnerId] 
    FOREIGN KEY ([OwnerId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [FileItems] ADD CONSTRAINT [FK_FileItems_AspNetUsers_ClientId] 
    FOREIGN KEY ([ClientId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [FileItems] ADD CONSTRAINT [FK_FileItems_FileItems_ParentFolderId] 
    FOREIGN KEY ([ParentFolderId]) REFERENCES [FileItems] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [FileShares] ADD CONSTRAINT [FK_FileShares_FileItems_FileItemId] 
    FOREIGN KEY ([FileItemId]) REFERENCES [FileItems] ([Id]) ON DELETE CASCADE;

ALTER TABLE [FileShares] ADD CONSTRAINT [FK_FileShares_AspNetUsers_SharedWithUserId] 
    FOREIGN KEY ([SharedWithUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [FileShares] ADD CONSTRAINT [FK_FileShares_AspNetUsers_SharedByUserId] 
    FOREIGN KEY ([SharedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
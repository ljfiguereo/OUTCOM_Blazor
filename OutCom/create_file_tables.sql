-- Script para crear las tablas FileItems y FileShares de forma simple
-- Sin relaciones de clave foránea problemáticas

-- Crear tabla FileItems
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='FileItems' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[FileItems] (
        [Id] uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [Name] nvarchar(255) NOT NULL,
        [Path] nvarchar(500) NOT NULL,
        [Type] int NOT NULL,
        [Size] bigint NULL,
        [MimeType] nvarchar(100) NULL,
        [OwnerId] nvarchar(450) NOT NULL,
        [ClientId] nvarchar(max) NULL,
        [ParentFolderId] uniqueidentifier NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ModifiedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL DEFAULT 0,
        [Description] nvarchar(max) NULL
    );
    
    -- Solo agregar la clave foránea para Owner con NO ACTION
    ALTER TABLE [dbo].[FileItems]
    ADD CONSTRAINT [FK_FileItems_AspNetUsers_OwnerId] 
    FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[AspNetUsers] ([Id]) 
    ON DELETE NO ACTION;
    
    -- Crear índices
    CREATE INDEX [IX_FileItems_OwnerId] ON [dbo].[FileItems] ([OwnerId]);
END

-- Crear tabla FileShares
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='FileShares' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[FileShares] (
        [Id] uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FileItemId] uniqueidentifier NOT NULL,
        [SharedWithUserId] nvarchar(450) NOT NULL,
        [SharedByUserId] nvarchar(450) NOT NULL,
        [SharedAt] datetime2 NOT NULL,
        [ExpiresAt] datetime2 NULL,
        [CanEdit] bit NOT NULL DEFAULT 0,
        [CanDelete] bit NOT NULL DEFAULT 0
    );
    
    -- Agregar claves foráneas con NO ACTION
    ALTER TABLE [dbo].[FileShares]
    ADD CONSTRAINT [FK_FileShares_FileItems_FileItemId] 
    FOREIGN KEY ([FileItemId]) REFERENCES [dbo].[FileItems] ([Id]) 
    ON DELETE CASCADE;
    
    ALTER TABLE [dbo].[FileShares]
    ADD CONSTRAINT [FK_FileShares_AspNetUsers_SharedWithUserId] 
    FOREIGN KEY ([SharedWithUserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) 
    ON DELETE NO ACTION;
    
    ALTER TABLE [dbo].[FileShares]
    ADD CONSTRAINT [FK_FileShares_AspNetUsers_SharedByUserId] 
    FOREIGN KEY ([SharedByUserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) 
    ON DELETE NO ACTION;
    
    -- Crear índices
    CREATE INDEX [IX_FileShares_FileItemId] ON [dbo].[FileShares] ([FileItemId]);
    CREATE INDEX [IX_FileShares_SharedWithUserId] ON [dbo].[FileShares] ([SharedWithUserId]);
    CREATE INDEX [IX_FileShares_SharedByUserId] ON [dbo].[FileShares] ([SharedByUserId]);
END

PRINT 'Tablas FileItems y FileShares creadas exitosamente';
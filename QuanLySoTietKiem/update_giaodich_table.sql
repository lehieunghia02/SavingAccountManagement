-- SQL Script to add missing columns to GiaoDich table
-- This script checks if columns exist before adding them to avoid errors

-- Check and add LoaiGiaoDich column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GiaoDichs]') AND name = 'LoaiGiaoDich')
BEGIN
    ALTER TABLE [dbo].[GiaoDichs] ADD [LoaiGiaoDich] NVARCHAR(50) NULL;
    PRINT 'Added LoaiGiaoDich column';
END
ELSE
BEGIN
    PRINT 'LoaiGiaoDich column already exists';
END

-- Check and add MoTa column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GiaoDichs]') AND name = 'MoTa')
BEGIN
    ALTER TABLE [dbo].[GiaoDichs] ADD [MoTa] NVARCHAR(255) NULL;
    PRINT 'Added MoTa column';
END
ELSE
BEGIN
    PRINT 'MoTa column already exists';
END

-- Check and add TrangThai column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GiaoDichs]') AND name = 'TrangThai')
BEGIN
    ALTER TABLE [dbo].[GiaoDichs] ADD [TrangThai] NVARCHAR(50) NULL;
    PRINT 'Added TrangThai column';
END
ELSE
BEGIN
    PRINT 'TrangThai column already exists';
END

-- Check and add UserId column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GiaoDichs]') AND name = 'UserId')
BEGIN
    ALTER TABLE [dbo].[GiaoDichs] ADD [UserId] NVARCHAR(450) NULL;
    PRINT 'Added UserId column';
END
ELSE
BEGIN
    PRINT 'UserId column already exists';
END

-- Check if MaSoTietKiem is nullable, if not make it nullable
IF EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[GiaoDichs]') 
    AND name = 'MaSoTietKiem' 
    AND is_nullable = 0
)
BEGIN
    -- Drop existing foreign key constraint if it exists
    DECLARE @constraintName NVARCHAR(128)
    SELECT @constraintName = name
    FROM sys.foreign_keys
    WHERE parent_object_id = OBJECT_ID(N'[dbo].[GiaoDichs]')
    AND referenced_object_id = OBJECT_ID(N'[dbo].[SoTietKiems]')
    
    IF @constraintName IS NOT NULL
    BEGIN
        DECLARE @sql NVARCHAR(MAX) = N'ALTER TABLE [dbo].[GiaoDichs] DROP CONSTRAINT ' + QUOTENAME(@constraintName)
        EXEC sp_executesql @sql
        PRINT 'Dropped foreign key constraint: ' + @constraintName
    END
    
    -- Alter column to be nullable
    ALTER TABLE [dbo].[GiaoDichs] ALTER COLUMN [MaSoTietKiem] INT NULL;
    PRINT 'Made MaSoTietKiem column nullable';
    
    -- Re-add foreign key with NULL allowed
    IF @constraintName IS NOT NULL
    BEGIN
        ALTER TABLE [dbo].[GiaoDichs] ADD CONSTRAINT FK_GiaoDichs_SoTietKiems_MaSoTietKiem
        FOREIGN KEY ([MaSoTietKiem]) REFERENCES [dbo].[SoTietKiems] ([MaSoTietKiem]);
        PRINT 'Re-added foreign key constraint with NULL allowed';
    END
END
ELSE
BEGIN
    PRINT 'MaSoTietKiem column is already nullable';
END

-- Check if SoTien is decimal, if not convert it
IF EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[GiaoDichs]') 
    AND name = 'SoTien' 
    AND system_type_id = 62 -- float
)
BEGIN
    ALTER TABLE [dbo].[GiaoDichs] ALTER COLUMN [SoTien] DECIMAL(18,2) NOT NULL;
    PRINT 'Converted SoTien column from float to decimal(18,2)';
END
ELSE
BEGIN
    PRINT 'SoTien column is already decimal or not float';
END

-- Add foreign key for UserId if it doesn't exist
IF NOT EXISTS (
    SELECT * 
    FROM sys.foreign_keys 
    WHERE parent_object_id = OBJECT_ID(N'[dbo].[GiaoDichs]')
    AND referenced_object_id = OBJECT_ID(N'[dbo].[AspNetUsers]')
)
BEGIN
    -- First check if the index exists
    IF NOT EXISTS (
        SELECT * 
        FROM sys.indexes 
        WHERE object_id = OBJECT_ID(N'[dbo].[GiaoDichs]')
        AND name = 'IX_GiaoDichs_UserId'
    )
    BEGIN
        CREATE INDEX [IX_GiaoDichs_UserId] ON [dbo].[GiaoDichs]([UserId]);
        PRINT 'Created index IX_GiaoDichs_UserId';
    END
    
    -- Then add the foreign key
    ALTER TABLE [dbo].[GiaoDichs] ADD CONSTRAINT FK_GiaoDichs_AspNetUsers_UserId
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]);
    PRINT 'Added foreign key constraint for UserId';
END
ELSE
BEGIN
    PRINT 'Foreign key for UserId already exists';
END

PRINT 'Database update completed successfully'; 
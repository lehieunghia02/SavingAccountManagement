-- SQL Script to update the migration history table
-- This script marks the pending migrations as applied

-- Check if the migrations are already in the history table
IF NOT EXISTS (SELECT * FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = '20250225013528_AddAvatarUrlToUser')
BEGIN
    -- Insert the migration record
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250225013528_AddAvatarUrlToUser', '6.0.16');
    PRINT 'Marked migration 20250225013528_AddAvatarUrlToUser as applied';
END
ELSE
BEGIN
    PRINT 'Migration 20250225013528_AddAvatarUrlToUser is already marked as applied';
END

IF NOT EXISTS (SELECT * FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = '20250225013801_AddAvatarUrlColumn')
BEGIN
    -- Insert the migration record
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250225013801_AddAvatarUrlColumn', '6.0.16');
    PRINT 'Marked migration 20250225013801_AddAvatarUrlColumn as applied';
END
ELSE
BEGIN
    PRINT 'Migration 20250225013801_AddAvatarUrlColumn is already marked as applied';
END

IF NOT EXISTS (SELECT * FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = '20250304190536_AddMissingColumnsToGiaoDich')
BEGIN
    -- Insert the migration record
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250304190536_AddMissingColumnsToGiaoDich', '6.0.16');
    PRINT 'Marked migration 20250304190536_AddMissingColumnsToGiaoDich as applied';
END
ELSE
BEGIN
    PRINT 'Migration 20250304190536_AddMissingColumnsToGiaoDich is already marked as applied';
END

PRINT 'Migration history update completed successfully'; 
-- Created by GitHub Copilot in SSMS - review carefully before executing

DECLARE @DatabaseName SYSNAME = N'BD_BancoFisclase';
DECLARE @TargetDir NVARCHAR(4000) = N'C:\Backups';
DECLARE @Timestamp NVARCHAR(20) = FORMAT(GETDATE(),'yyyyMMdd_HHmmss');
DECLARE @BackupFile NVARCHAR(4000) = @TargetDir + N'\' + @DatabaseName + N'_FULL_' + @Timestamp + N'.bak';

-- Report planned backup file and current recovery model
PRINT N'Planned backup file: ' + @BackupFile;
SELECT name AS DatabaseName, recovery_model_desc AS RecoveryModel
FROM sys.databases
WHERE name = @DatabaseName;

-- Build and execute backup command dynamically to allow variable file name
DECLARE @sql NVARCHAR(MAX) = N'BACKUP DATABASE ' + QUOTENAME(@DatabaseName) +
    N' TO DISK = N''' + @BackupFile + N''' WITH FORMAT, INIT, SKIP, STATS = 10, COMPRESSION, CHECKSUM;';

PRINT N'Executing backup command...';
EXEC sp_executesql @sql;
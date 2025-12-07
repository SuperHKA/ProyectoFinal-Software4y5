-- =====================================================
-- ACTUALIZACIONES NECESARIAS PARA LA PÁGINA WEB
-- Ejecutar este script después de crear la base de datos
-- =====================================================

USE BD_BancoFisclase
GO

-- 1. Crear tabla AuditLog para registrar acciones del sistema
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLog]') AND type in (N'U'))
BEGIN
    CREATE TABLE AuditLog (
        audit_id INT IDENTITY(1,1) NOT NULL,
        accion NVARCHAR(100),
        usuario_id INT,
        detalle NVARCHAR(MAX),
        fecha DATETIME DEFAULT GETDATE(),
        CONSTRAINT PK_AuditLog PRIMARY KEY CLUSTERED (audit_id)
    );
    
    PRINT 'Tabla AuditLog creada exitosamente';
END
ELSE
BEGIN
    PRINT 'La tabla AuditLog ya existe';
END
GO

PRINT 'Script de actualizaciones completado';
PRINT 'Recuerda compilar y ejecutar la API actualizada';
GO

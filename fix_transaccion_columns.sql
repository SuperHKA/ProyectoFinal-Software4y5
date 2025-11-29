-- Corregir nombres de columnas en la tabla Transaccion
-- Primero verificar la estructura actual
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Transaccion' 
  AND COLUMN_NAME IN ('cuenta_destino', 'cuenta_origen', 'cuenta_destino_id', 'cuenta_origen_id')
ORDER BY ORDINAL_POSITION;

-- Si existen las columnas con nombres incorrectos, renombrarlas
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Transaccion' AND COLUMN_NAME = 'cuenta_destino')
BEGIN
    EXEC sp_rename 'Transaccion.cuenta_destino', 'cuenta_destino_id', 'COLUMN';
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Transaccion' AND COLUMN_NAME = 'cuenta_origen')
BEGIN
    EXEC sp_rename 'Transaccion.cuenta_origen', 'cuenta_origen_id', 'COLUMN';
END

-- Verificar que las columnas ahora tienen los nombres correctos
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Transaccion' 
  AND COLUMN_NAME IN ('cuenta_destino_id', 'cuenta_origen_id')
ORDER BY ORDINAL_POSITION;
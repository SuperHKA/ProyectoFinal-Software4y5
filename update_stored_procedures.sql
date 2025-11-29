-- Procedimientos almacenados para transferencias
-- Estos procedimientos deben usar los nombres de columnas correctos

-- Procedimiento para transferencia interna
CREATE OR ALTER PROCEDURE sp_TransferenciaInterna
    @cuenta_origen_id INT,
    @cuenta_destino_id INT,
    @monto DECIMAL(18,2),
    @descripcion NVARCHAR(200) = NULL,
    @usuario_id INT,
    @transaccion_id INT OUTPUT,
    @message NVARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verificar que las cuentas existen y están activas
        DECLARE @saldo_origen DECIMAL(18,2);
        DECLARE @cliente_origen INT;
        DECLARE @cliente_destino INT;
        
        SELECT @saldo_origen = saldo, @cliente_origen = cliente_id
        FROM Cuenta 
        WHERE cuenta_id = @cuenta_origen_id AND activa = 1;
        
        IF @saldo_origen IS NULL
        BEGIN
            SET @message = 'Cuenta de origen no encontrada o inactiva';
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        SELECT @cliente_destino = cliente_id
        FROM Cuenta 
        WHERE cuenta_id = @cuenta_destino_id AND activa = 1;
        
        IF @cliente_destino IS NULL
        BEGIN
            SET @message = 'Cuenta de destino no encontrada o inactiva';
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Verificar saldo suficiente
        IF @saldo_origen < @monto
        BEGIN
            SET @message = 'Saldo insuficiente';
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Crear la transacción
        INSERT INTO Transaccion (tipo_transaccion_id, estado_transaccion_id, monto, descripcion, usuario_id, cuenta_origen_id, cuenta_destino_id)
        VALUES (
            (SELECT tipo_transaccion_id FROM TipoTransaccion WHERE nombre = 'TRANSFER_INTERNO'),
            (SELECT estado_transaccion_id FROM EstadoTransaccion WHERE nombre = 'COMPLETADO'),
            @monto,
            @descripcion,
            @usuario_id,
            @cuenta_origen_id,
            @cuenta_destino_id
        );
        
        SET @transaccion_id = SCOPE_IDENTITY();
        
        -- Actualizar saldos
        UPDATE Cuenta SET saldo = saldo - @monto WHERE cuenta_id = @cuenta_origen_id;
        UPDATE Cuenta SET saldo = saldo + @monto WHERE cuenta_id = @cuenta_destino_id;
        
        -- Crear movimientos
        INSERT INTO Movimiento (cuenta_id, transaccion_id, monto_debito, monto_credito, saldo_resultante)
        VALUES (@cuenta_origen_id, @transaccion_id, @monto, 0, @saldo_origen - @monto);
        
        INSERT INTO Movimiento (cuenta_id, transaccion_id, monto_debito, monto_credito, saldo_resultante)
        VALUES (@cuenta_destino_id, @transaccion_id, 0, @monto, (SELECT saldo FROM Cuenta WHERE cuenta_id = @cuenta_destino_id));
        
        COMMIT TRANSACTION;
        SET @message = 'Transferencia completada exitosamente';
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @message = 'Error: ' + ERROR_MESSAGE();
        SET @transaccion_id = 0;
    END CATCH
END;
GO

-- Procedimiento para transferencia externa
CREATE OR ALTER PROCEDURE sp_TransferenciaExterna
    @cuenta_origen_id INT,
    @numero_cuenta_destino NVARCHAR(20),
    @monto DECIMAL(18,2),
    @descripcion NVARCHAR(200) = NULL,
    @usuario_id INT,
    @transaccion_id INT OUTPUT,
    @message NVARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verificar que la cuenta origen existe y está activa
        DECLARE @saldo_origen DECIMAL(18,2);
        DECLARE @cliente_origen INT;
        
        SELECT @saldo_origen = saldo, @cliente_origen = cliente_id
        FROM Cuenta 
        WHERE cuenta_id = @cuenta_origen_id AND activa = 1;
        
        IF @saldo_origen IS NULL
        BEGIN
            SET @message = 'Cuenta de origen no encontrada o inactiva';
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Verificar saldo suficiente
        IF @saldo_origen < @monto
        BEGIN
            SET @message = 'Saldo insuficiente';
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Crear la transacción (sin cuenta_destino_id para transferencia externa)
        INSERT INTO Transaccion (tipo_transaccion_id, estado_transaccion_id, monto, descripcion, usuario_id, cuenta_origen_id, cuenta_destino_id)
        VALUES (
            (SELECT tipo_transaccion_id FROM TipoTransaccion WHERE nombre = 'TRANSFER_EXTERNO'),
            (SELECT estado_transaccion_id FROM EstadoTransaccion WHERE nombre = 'COMPLETADO'),
            @monto,
            COALESCE(@descripcion, 'Transferencia externa a ' + @numero_cuenta_destino),
            @usuario_id,
            @cuenta_origen_id,
            NULL -- No hay cuenta destino interna
        );
        
        SET @transaccion_id = SCOPE_IDENTITY();
        
        -- Actualizar saldo de origen
        UPDATE Cuenta SET saldo = saldo - @monto WHERE cuenta_id = @cuenta_origen_id;
        
        -- Crear movimiento de débito
        INSERT INTO Movimiento (cuenta_id, transaccion_id, monto_debito, monto_credito, saldo_resultante)
        VALUES (@cuenta_origen_id, @transaccion_id, @monto, 0, @saldo_origen - @monto);
        
        COMMIT TRANSACTION;
        SET @message = 'Transferencia externa completada exitosamente';
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @message = 'Error: ' + ERROR_MESSAGE();
        SET @transaccion_id = 0;
    END CATCH
END;
GO
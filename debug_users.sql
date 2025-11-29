-- Script para verificar y corregir la configuración de usuarios
-- Ejecutar este script para asegurarse de que los usuarios están configurados correctamente

-- 1. Verificar usuarios existentes y su configuración
SELECT 
    u.usuario_id,
    u.username,
    u.es_admin,
    u.bloqueado,
    u.cliente_id,
    c.nombre as cliente_nombre,
    c.email
FROM Usuario u
LEFT JOIN Cliente c ON u.cliente_id = c.cliente_id
ORDER BY u.usuario_id;

-- 2. Asegurarse de que juanperez NO es admin (debería ser es_admin = 0)
UPDATE Usuario 
SET es_admin = 0
WHERE username = 'juanperez' AND es_admin = 1;

-- 3. Verificar que existe al menos un usuario administrador
-- Si no existe, crear uno para pruebas
IF NOT EXISTS (SELECT 1 FROM Usuario WHERE es_admin = 1)
BEGIN
    -- Crear cliente admin si no existe
    IF NOT EXISTS (SELECT 1 FROM Cliente WHERE email = 'admin@fisclase.com')
    BEGIN
        INSERT INTO Cliente (nombre, email, activo, fecha_registro)
        VALUES ('Administrador Sistema', 'admin@fisclase.com', 1, GETDATE());
    END

    DECLARE @adminClienteId INT = (SELECT cliente_id FROM Cliente WHERE email = 'admin@fisclase.com');
    
    -- Crear usuario admin si no existe
    IF NOT EXISTS (SELECT 1 FROM Usuario WHERE username = 'admin')
    BEGIN
        INSERT INTO Usuario (cliente_id, username, password_hash, salt, es_admin, bloqueado, fecha_creacion)
        VALUES (
            @adminClienteId,
            'admin',
            'W6ph5Mm5Pz8GgiULbPgzG37mj9g=', -- password "admin123" con salt "VGVzdFNhbHQ="
            'VGVzdFNhbHQ=',
            1,
            0,
            GETDATE()
        );
    END
END

-- 4. Verificar configuración final
SELECT 
    'RESUMEN DE USUARIOS' as info,
    COUNT(*) as total_usuarios,
    SUM(CASE WHEN es_admin = 1 THEN 1 ELSE 0 END) as total_admins,
    SUM(CASE WHEN es_admin = 0 THEN 1 ELSE 0 END) as total_clientes,
    SUM(CASE WHEN bloqueado = 1 THEN 1 ELSE 0 END) as total_bloqueados
FROM Usuario;

-- 5. Mostrar cuentas del cliente juanperez
SELECT 
    'CUENTAS DE JUANPEREZ' as info,
    c.cuenta_id,
    c.cliente_id,
    c.numero_cuenta,
    tc.nombre as tipo_cuenta,
    c.saldo,
    c.activa
FROM Cuenta c
JOIN Cliente cl ON c.cliente_id = cl.cliente_id
JOIN Usuario u ON u.cliente_id = cl.cliente_id
LEFT JOIN TipoCuenta tc ON c.tipo_cuenta_id = tc.tipo_cuenta_id
WHERE u.username = 'juanperez';

-- 6. Mostrar transacciones recientes de juanperez
SELECT TOP 10
    'TRANSACCIONES DE JUANPEREZ' as info,
    t.transaccion_id,
    t.fecha,
    tt.nombre as tipo,
    t.monto,
    t.descripcion,
    co.numero_cuenta as cuenta_origen,
    cd.numero_cuenta as cuenta_destino
FROM Transaccion t
LEFT JOIN TipoTransaccion tt ON t.tipo_transaccion_id = tt.tipo_transaccion_id
LEFT JOIN Cuenta co ON t.cuenta_origen = co.cuenta_id
LEFT JOIN Cuenta cd ON t.cuenta_destino = cd.cuenta_id
WHERE (co.cliente_id = (SELECT cliente_id FROM Usuario WHERE username = 'juanperez'))
   OR (cd.cliente_id = (SELECT cliente_id FROM Usuario WHERE username = 'juanperez'))
ORDER BY t.fecha DESC;

-- 7. NUEVA: Verificar específicamente la query que usa la API
DECLARE @juanperezClienteId INT = (SELECT cliente_id FROM Usuario WHERE username = 'juanperez');
PRINT 'Cliente ID de juanperez: ' + CAST(@juanperezClienteId AS VARCHAR(10));

-- 8. NUEVA: Test de dashboard data
SELECT 
    'DASHBOARD DATA SIMULATION' as info,
    (SELECT ISNULL(SUM(saldo), 0) FROM Cuenta WHERE cliente_id = @juanperezClienteId AND activa = 1) as saldo_total,
    (SELECT COUNT(*) FROM Cuenta WHERE cliente_id = @juanperezClienteId AND activa = 1) as total_cuentas,
    (SELECT COUNT(*) FROM Transaccion t 
     LEFT JOIN Cuenta co ON t.cuenta_origen = co.cuenta_id
     LEFT JOIN Cuenta cd ON t.cuenta_destino = cd.cuenta_id
     WHERE (co.cliente_id = @juanperezClienteId OR cd.cliente_id = @juanperezClienteId)
     AND CAST(t.fecha AS DATE) = CAST(GETDATE() AS DATE)) as transacciones_hoy;

-- 9. NUEVA: Verificar que los endpoints de la API podrán encontrar los datos
SELECT 'API ENDPOINT TEST' as info;

-- Test GetClienteIdByUsernameAsync
SELECT 'GetClienteIdByUsernameAsync' as endpoint, cliente_id FROM Usuario WHERE username = 'juanperez';

-- Test ObtenerCuentasPorClienteAsync
SELECT 
    'ObtenerCuentasPorClienteAsync' as endpoint,
    c.cuenta_id, c.numero_cuenta, tc.nombre AS tipo, c.saldo, c.activa 
FROM Cuenta c 
LEFT JOIN TipoCuenta tc ON c.tipo_cuenta_id = tc.tipo_cuenta_id 
WHERE c.cliente_id = @juanperezClienteId AND c.activa = 1 
ORDER BY c.cuenta_id;

-- Test ObtenerTransaccionesClienteAsync
SELECT TOP 5
    'ObtenerTransaccionesClienteAsync' as endpoint,
    t.transaccion_id, 
    t.fecha, 
    tt.nombre AS tipo_transaccion, 
    et.nombre AS estado, 
    t.monto, 
    t.descripcion,
    co.numero_cuenta AS cuenta_origen,
    cd.numero_cuenta AS cuenta_destino
FROM Transaccion t 
LEFT JOIN TipoTransaccion tt ON t.tipo_transaccion_id = tt.tipo_transaccion_id 
LEFT JOIN EstadoTransaccion et ON t.estado_transaccion_id = et.estado_transaccion_id 
LEFT JOIN Cuenta co ON t.cuenta_origen = co.cuenta_id
LEFT JOIN Cuenta cd ON t.cuenta_destino = cd.cuenta_id
WHERE (co.cliente_id = @juanperezClienteId OR cd.cliente_id = @juanperezClienteId)
ORDER BY t.fecha DESC;
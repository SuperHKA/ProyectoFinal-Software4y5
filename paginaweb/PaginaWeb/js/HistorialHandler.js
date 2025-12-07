document.addEventListener('DOMContentLoaded', async function() {
    if (!authService.isLoggedIn()) {
        window.location.href = 'Login.html';
        return;
    }

    try {
        // Obtener cuentas y transacciones recientes
        const cuentasResp = await authService.getCuentas(); // Ya devuelve sólo el array de cuentas
        // Intentamos obtener transacciones recientes (endpoint genérico)
        const resp = await fetch(`${authService.baseURL}/banco/transacciones-recientes?page=1&pageSize=50`, {
            headers: { 'Authorization': `Bearer ${authService.getAuthToken()}` }
        });

        const transData = resp.ok ? await resp.json() : { items: [] };
        const transacciones = transData.items || transData || [];

        // Actualizar resumen del mes (suma simple de movimientos)
        let ingresos = 0, egresos = 0;
        transacciones.forEach(t => {
            const amount = parseFloat(t.monto || t.cantidad || 0);
            if (amount >= 0) ingresos += amount; else egresos += Math.abs(amount);
        });

        const totalTrans = transacciones.length;
        const saldoActual = (cuentasResp && cuentasResp[0] && cuentasResp[0].saldo) ? cuentasResp[0].saldo : 0;

        // Rellenar estadísticas
        const saldoEl = document.querySelector('.resumen-mes .saldo-actual .stat-value') || document.querySelector('.stat-value');
        if (saldoEl) saldoEl.textContent = `$${Number(saldoActual).toFixed(2)}`;
        const ingresosEl = document.querySelector('.resumen-mes .ingresos .stat-value');
        if (ingresosEl) ingresosEl.textContent = `+$${Number(ingresos).toFixed(2)}`;
        const egresosEl = document.querySelector('.resumen-mes .egresos .stat-value');
        if (egresosEl) egresosEl.textContent = `-$${Number(egresos).toFixed(2)}`;
        const totalEl = document.querySelector('.resumen-mes .transacciones .stat-value');
        if (totalEl) totalEl.textContent = `${totalTrans}`;

        // Tarjeta de cuenta
        const nomCuenta = document.querySelector('.tarjeta-header .nom-cuenta');
        const numCuenta = document.querySelector('.tarjeta-header .num-cuenta');
        if (cuentasResp && cuentasResp.length > 0) {
            const cuenta = cuentasResp[0];
            if (nomCuenta) nomCuenta.textContent = cuenta.alias || (`Cuenta ${cuenta.tipoCuenta || ''}`);
            if (numCuenta) numCuenta.textContent = cuenta.numeroCuenta || cuenta.id || '';
            const saldoItems = document.querySelectorAll('.saldo-info .saldo');
            if (saldoItems && saldoItems.length >= 2) {
                saldoItems[0].textContent = `${Number(cuenta.saldo || 0).toFixed(2)} USD`;
                saldoItems[1].textContent = `${Number(cuenta.saldoDisponible || cuenta.saldo || 0).toFixed(2)} USD`;
            }
        }

        // Últimas transacciones
        const transList = document.querySelector('.transacciones-lista');
        if (transList) {
            transList.innerHTML = '';
            transacciones.slice(0,10).forEach(t => {
                const item = document.createElement('div');
                item.className = 'transaccion-item';
                const merchant = t.descripcion || t.merchant || t.tipo || 'Transacción';
                const fecha = t.fecha ? new Date(t.fecha).toLocaleDateString('es-MX') : '';
                const monto = Number(t.monto || t.cantidad || 0).toFixed(2);
                const claseMonto = Number(t.monto || t.cantidad || 0) >= 0 ? 'positivo' : 'negativo';

                item.innerHTML = `
                    <div class="transaccion-info">
                        <div class="merchant">${merchant}</div>
                        <div class="fecha">${fecha}</div>
                    </div>
                    <div class="monto ${claseMonto}">${Number(t.monto || t.cantidad || 0) >=0 ? '+' : '-'}${Math.abs(monto)} USD</div>
                `;

                transList.appendChild(item);
            });
        }

        // Historial general (tabla)
        const tbody = document.querySelector('.historial-tabla tbody');
        if (tbody) {
            tbody.innerHTML = '';
            transacciones.forEach(t => {
                const tr = document.createElement('tr');
                const fecha = t.fecha ? new Date(t.fecha).toLocaleDateString('es-MX') : '';
                const descripcion = t.descripcion || t.tipo || '';
                const monto = Number(t.monto || t.cantidad || 0).toFixed(2);
                const saldo = t.saldo ? Number(t.saldo).toFixed(2) : '';
                tr.innerHTML = `
                    <td>${fecha}</td>
                    <td>${descripcion}</td>
                    <td class="${Number(t.monto||0) >=0 ? 'monto-positivo' : 'monto-negativo'}">${Number(t.monto||0) >=0 ? '+' : '-'}$${Math.abs(monto)}</td>
                    <td>${saldo ? `$${saldo} USD` : ''}</td>
                `;
                tbody.appendChild(tr);
            });
        }

    } catch (error) {
        console.error('Error al cargar historial:', error);
    }
});

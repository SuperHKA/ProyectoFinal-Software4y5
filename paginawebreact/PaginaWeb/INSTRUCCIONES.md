# Instrucciones para ejecutar la Página Web con la API

## 1. Iniciar la API

### Opción A: Desde Visual Studio
1. Abre el proyecto `API_BancoFisclase` en Visual Studio
2. Presiona F5 o haz clic en "Iniciar" (botón verde)
3. La API se ejecutará en `http://localhost:5202`

### Opción B: Desde PowerShell
```powershell
cd "c:\Visual Studio Projects\ProyectoFinal-Software-4-y-5\API_BancoFisclase"
dotnet run
```

## 2. Verificar que la API esté funcionando

Abre tu navegador y visita:
- `http://localhost:5202` - Debería mostrar Swagger UI
- `http://localhost:5202/api/banco/health` - Debería devolver `{"status":"healthy"}`

## 3. Abrir la Página Web

### Usando Live Server (Recomendado)
1. Instala la extensión "Live Server" en VS Code
2. Haz clic derecho en `PaginaWeb/html/Pagina_Principal.html`
3. Selecciona "Open with Live Server"

### Directamente en el navegador
1. Navega a: `c:\Visual Studio Projects\ProyectoFinal-Software-4-y-5\paginawebreact\PaginaWeb\html\Pagina_Principal.html`
2. Haz doble clic para abrir en tu navegador

## 4. Usar la aplicación

### Registro de nuevo usuario
1. En la página principal, haz clic en "Cuenta" → "Registrarse"
2. Completa el formulario con tus datos
3. Haz clic en "Registrar"

### Iniciar Sesión
1. En la página principal, haz clic en "Cuenta" → "Ingresar"
2. Usa las credenciales que registraste
3. Serás redirigido al Dashboard

### Funcionalidades disponibles
- **Dashboard**: Ver resumen de tus cuentas y transacciones
- **Cuentas**: Ver detalles de tus cuentas bancarias
- **Transferencias**: Realizar transferencias entre cuentas
- **Transacciones**: Ver historial de transacciones
- **Perfil**: Ver y editar tu información personal

## 5. Solución de problemas

### La API no inicia
- Verifica que no haya otra aplicación usando el puerto 5202
- Verifica la cadena de conexión en `appsettings.json`
- Asegúrate de que SQL Server esté ejecutándose

### Errores de CORS
- La API ya está configurada para aceptar peticiones desde localhost
- Si usas un puerto diferente, agrégalo en `APIProgram.cs` en la sección CORS

### No se muestran los estilos
- Verifica que las rutas en los archivos HTML sean correctas (../css/...)
- Asegúrate de estar abriendo los archivos desde la carpeta `html`

### Errores de autenticación
- Limpia el localStorage del navegador (F12 → Application → Local Storage → Clear)
- Verifica que la base de datos tenga datos de prueba

## 6. Archivos principales actualizados

Los siguientes archivos han sido creados/actualizados para funcionar con la API:

### JavaScript
- `js/AuthServiceNew.js` - Servicio de autenticación mejorado
- `js/DashboardHandlerNew.js` - Manejador del dashboard
- `js/TransferenciasHandler.js` - Manejador de transferencias
- `js/ApiClient.js` - Cliente HTTP para la API

### HTML
- Todos los archivos HTML actualizados con las rutas correctas de CSS/JS
- Referencias actualizadas a los nuevos archivos JavaScript

## 7. Endpoints de la API disponibles

### Autenticación
- `POST /api/banco/login` - Login de usuario
- `POST /api/banco/clientes` - Registro de nuevo cliente

### Cuentas
- `GET /api/banco/clientes/por-usuario?username={username}` - Obtener cuentas del usuario

### Transferencias
- `POST /api/banco/transfer/internal` - Realizar transferencia interna

### Transacciones
- `GET /api/banco/transacciones-recientes` - Obtener transacciones recientes

### Salud
- `GET /api/banco/health` - Verificar estado de la API

## 8. Credenciales de prueba

Si tienes datos de prueba en la base de datos, puedes usar esas credenciales.
Para crear un nuevo usuario, usa la funcionalidad de registro.

## Notas importantes

1. **La API debe estar ejecutándose** antes de usar la página web
2. Los archivos JavaScript nuevos (con sufijo "New") son los correctos
3. Asegúrate de tener una base de datos SQL Server configurada
4. La configuración de CORS permite localhost en los puertos 3000, 5000, 5001, 5202, 7102

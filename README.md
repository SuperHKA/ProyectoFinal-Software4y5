# Proyecto Final Software 4 y 5

Bienvenido al repositorio del proyecto. Aquí encontrarás una descripción clara y útil para entender la solución, sus objetivos y cómo usarla sin perder el toque humano.

## 1. Introducción
Este proyecto implementa una solución bancaria educativa con dos partes:
- Aplicación de escritorio (WinForms, .NET Framework 4.7.2): panel administrativo, gestión de clientes, cuentas y transacciones.
- API REST (ASP.NET Core, .NET 10): expone servicios para autenticación, clientes, cuentas, reportes y transferencias.

La propuesta busca simular operaciones bancarias comunes, facilitar la administración y ofrecer una base sólida para prácticas de integración frontend/backend.

## 2. Objetivos y alcance
Objetivos principales:
- Gestionar clientes (crear, actualizar, bloquear/desbloquear).
- Administrar cuentas bancarias (crear cuentas, consultar saldo, ver historial).
- Registrar y consultar transacciones (depósitos, retiros, transferencias internas/externas).
- Generar reportes y paneles con estadísticas del sistema.
- Proveer endpoints REST seguros y consistentes.

Alcance del proyecto:
- Módulos de autenticación para usuarios y administradores.
- Panel administrativo con vistas: clientes, transacciones recientes, movimientos del día, análisis y resúmenes.
- API documentada por rutas y contratos (DTOs) compartidos.

## 3. Diagrama de la estructura del sistema (diagrama de clases)
A nivel conceptual, la estructura principal se compone de:
- `Usuario`, `Cliente`, `Cuenta`, `Transaccion` como entidades base.
- `ApiAdmin` y `ServicioBancoApi` como adaptadores/servicios de acceso a la API.
- DTOs para transferencia de datos: `ClienteDto`, `TransaccionDto`, `PaginatedResponse`, `LoginResultDto`, etc.

Sugerencia: incluir un diagrama UML en futuras versiones (Clases y relaciones: Cliente 1..* Cuenta, Cuenta 1..* Transaccion, Usuario 1..1 Cliente opcional, Servicios consumen/transforman DTOs).

## 4. Guía de usuario
- Inicio: ejecutar la aplicación WinForms (`ProyectoFinal-Software-4-y-5`). Se mostrará la ventana de login.
- Login: ingresar credenciales. Si el usuario es admin, se habilita el panel completo.
- Panel administrativo:
  - Clientes: ver, crear, actualizar, cambiar contraseña, bloquear/desbloquear.
  - Cuentas: abrir `Ver Cuentas` y seleccionar una para ver su detalle.
  - Historial: desde `Detalle de Cuenta`, abrir `Ver Historial`, filtrar por fechas y ver detalles.
  - Reportes: generar reporte por rango de fechas desde el panel.
- API: levantar el proyecto `API_BancoFisclase` (por defecto en `http://localhost:5202`). La aplicación de escritorio usa esta URL.

Consejos prácticos:
- Si la app no se cierra, usar el cierre seguro del menú o ejecutar `KillProcesses.bat`.
- Los filtros de historial aceptan rangos rápidos (7 días, 30 días, este año).
- Ante errores 400 con la API, revisar credenciales y formato de los datos (logs en Output).

## 5. Funcionalidades y características
- Autenticación (usuarios y administradores).
- Gestión de clientes (CRUD parcial, cambio de contraseña, bloqueo).
- Gestión de cuentas (creación, resumen por cliente, dashboard).
- Transacciones (depósitos, retiros, transferencias internas/externas).
- Historial con filtros por fecha y detalles por transacción.
- Reportes por rango de fechas y estadísticas diarias.
- Manejo robusto del cierre de aplicación (limpieza de recursos y procesos zombie).

## 6. Referencias bibliográficas (Norma APA 7)
- Microsoft. (2024). .NET Documentation. https://learn.microsoft.com/dotnet/
- Microsoft. (2024). ASP.NET Core Web API documentation. https://learn.microsoft.com/aspnet/core/web-api
- Freeman, A., & Sanderson, J. (2023). Pro ASP.NET Core 7. Apress.
- Albahari, J., & Albahari, B. (2022). C# 10 in a Nutshell. O’Reilly Media.

---

Sugerencias futuras:
- Agregar UML formal y capturas de pantalla del flujo.
- Incluir pruebas automatizadas y autenticación avanzada.
- Documentar endpoints con Swagger/OpenAPI.

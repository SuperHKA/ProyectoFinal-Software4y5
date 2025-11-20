# 🚀 Inicio Rápido - Portal Web Banco Fisclase

## ⚡ Instalación en 3 Pasos

### Paso 1: Abrir Terminal
```powershell
# Navega a la carpeta del proyecto
cd "c:\Users\andre\OneDrive\Documentos\Visual Studio\ProyectoFinal-Software-4-y-5\Banco"
```

### Paso 2: Instalar Dependencias
```powershell
npm install
```

### Paso 3: Iniciar Servidor
```powershell
npm run dev
```

## 🌐 Acceder al Portal

Una vez iniciado, abre tu navegador en:
```
http://localhost:3000
```

---

## 🎯 Uso Rápido

### 1. Página Principal
- Visita `http://localhost:3000`
- Explora los servicios bancarios
- Haz clic en "Iniciar Sesión" o "Registrarse"

### 2. Crear Cuenta
- Clic en "Registrarse"
- Completa el formulario
- Envía el registro

### 3. Iniciar Sesión
- Clic en "Iniciar Sesión"
- Ingresa usuario y contraseña
- Accede al dashboard

### 4. Explorar Dashboard
- Ver estadísticas
- Revisar movimientos del día
- Acceder a acciones rápidas

### 5. Gestionar Cuentas
- Menú: "Cuentas"
- Ver todas tus cuentas
- Mostrar/ocultar saldos

### 6. Ver Transacciones
- Menú: "Transacciones"
- Buscar transacciones
- Filtrar por tipo

### 7. Realizar Transferencias
- Menú: "Transferencias"
- Seleccionar cuenta origen
- Ingresar destino y monto

### 8. Editar Perfil
- Menú: "Perfil"
- Editar información
- Cambiar contraseña

---

## 🛠️ Comandos Útiles

| Comando | Descripción |
|---------|-------------|
| `npm run dev` | Inicia servidor de desarrollo |
| `npm run build` | Crea build de producción |
| `npm run preview` | Vista previa del build |
| `Ctrl + C` | Detener servidor |

---

## 📱 Probar Responsive

### En el Navegador:
1. Presiona `F12` (DevTools)
2. Haz clic en el icono de dispositivo móvil
3. Selecciona diferentes dispositivos
4. Prueba el menú hamburguesa

---

## ⚠️ Problemas Comunes

### Error: "npm no se reconoce"
**Solución**: Instala Node.js desde https://nodejs.org/

### Error: "Cannot find module"
**Solución**: Ejecuta `npm install` de nuevo

### Error: "Port 3000 already in use"
**Solución**: 
- Cierra otros procesos en el puerto 3000
- O usa: `npm run dev -- --port 3001`

### La API no responde
**Solución**: 
- Verifica que la API esté corriendo en puerto 5000
- Revisa la URL en `vite.config.js`

---

## 🎨 Personalización Rápida

### Cambiar Colores
Edita `src/styles/index.css`:
```css
:root {
  --primary-color: #TU_COLOR;
  --secondary-color: #TU_COLOR;
}
```

### Cambiar Logo
Reemplaza:
- `src/assets/logo.svg`
- `src/assets/logo.png`

### Cambiar Título
Edita `index.html`:
```html
<title>Tu Banco - Portal Cliente</title>
```

---

## 📊 Estructura de Navegación

```
Home (/)
├── Login (/login)
└── Register (/register)

Dashboard (/dashboard) [Protegido]
├── Accounts (/accounts) [Protegido]
├── Transactions (/transactions) [Protegido]
├── Transfer (/transfer) [Protegido]
└── Profile (/profile) [Protegido]
```

---

## 🔑 Credenciales de Prueba

Para probar el login, usa las credenciales configuradas en tu API:
```
Usuario: admin
Contraseña: [tu contraseña]
```

---

## 💡 Tips

1. **Desarrollo**: Usa `npm run dev` y el hot-reload actualizará automáticamente
2. **Producción**: Usa `npm run build` y despliega la carpeta `dist/`
3. **Debug**: Abre la consola del navegador (F12) para ver logs
4. **Estado**: Revisa el LocalStorage para ver la sesión
5. **API**: Verifica la URL de la API en `src/services/apiService.js`

---

## 🎉 ¡Listo!

Tu portal está completamente funcional. ¡Disfruta desarrollando!

**Si tienes dudas**, revisa:
- `README.md` - Documentación completa
- `INSTALLATION.md` - Guía detallada de instalación
- `PROJECT_SUMMARY.md` - Resumen del proyecto

---

**¡Feliz Coding! 🚀**

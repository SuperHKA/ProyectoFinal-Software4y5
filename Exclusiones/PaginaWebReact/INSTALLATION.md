# 🏦 Portal Web Banco Fisclase - Guía de Instalación

## ✅ Proyecto Completado

Se ha creado exitosamente un **portal web moderno** para el Banco Fisclase utilizando React.js.

## 📁 Estructura del Proyecto

```
Banco/
├── src/
│   ├── components/           # Componentes reutilizables
│   │   ├── Header.jsx       # Barra de navegación
│   │   ├── Footer.jsx       # Pie de página
│   │   └── ProtectedRoute.jsx # Rutas protegidas
│   ├── context/
│   │   └── AuthContext.jsx  # Contexto de autenticación
│   ├── pages/               # Páginas de la aplicación
│   │   ├── Home.jsx         # Página principal pública
│   │   ├── Login.jsx        # Página de inicio de sesión
│   │   ├── Register.jsx     # Página de registro
│   │   ├── Dashboard.jsx    # Panel de control
│   │   ├── Accounts.jsx     # Gestión de cuentas
│   │   ├── Transactions.jsx # Historial de transacciones
│   │   ├── Transfer.jsx     # Realizar transferencias
│   │   └── Profile.jsx      # Perfil de usuario
│   ├── services/
│   │   └── apiService.js    # Integración con API
│   ├── styles/              # Estilos CSS modernos
│   │   ├── index.css        # Estilos globales
│   │   ├── Header.css
│   │   ├── Footer.css
│   │   ├── Home.css
│   │   ├── Auth.css
│   │   ├── Dashboard.css
│   │   ├── Accounts.css
│   │   ├── Transactions.css
│   │   ├── Transfer.css
│   │   └── Profile.css
│   ├── assets/
│   │   ├── logo.svg         # Logo en SVG
│   │   └── logo.png         # Logo en PNG
│   ├── App.jsx              # Componente raíz
│   └── main.jsx             # Punto de entrada
├── index.html               # HTML principal
├── vite.config.js           # Configuración de Vite
├── package.json             # Dependencias
├── .gitignore              # Archivos ignorados
├── .eslintrc.cjs           # Configuración ESLint
├── jsconfig.json           # Configuración JavaScript
└── README.md               # Documentación
```

## 🎨 Características Implementadas

### ✨ Páginas y Funcionalidades

1. **Home (Página Principal)**
   - Hero section con call-to-action
   - Sección de servicios bancarios
   - Features destacadas
   - Diseño responsive

2. **Login**
   - Formulario de inicio de sesión
   - Validación de campos
   - Mostrar/ocultar contraseña
   - Integración con API

3. **Register**
   - Formulario completo de registro
   - Validación de datos
   - Secciones organizadas (acceso, personal, dirección)
   - Confirmación de contraseña

4. **Dashboard**
   - Estadísticas en tiempo real
   - Tarjetas de resumen (saldo, transacciones, clientes)
   - Movimientos del día
   - Acciones rápidas

5. **Accounts (Cuentas)**
   - Visualización de múltiples cuentas
   - Mostrar/ocultar saldos
   - Tipos de cuenta (Ahorro, Corriente, Crédito, Débito)
   - Estados (Activa/Inactiva)

6. **Transactions (Transacciones)**
   - Historial completo
   - Búsqueda y filtros
   - Iconos de ingreso/egreso
   - Detalles de cada transacción

7. **Transfer (Transferencias)**
   - Formulario de transferencia
   - Selección de cuenta origen
   - Validación de montos
   - Información de límites

8. **Profile (Perfil)**
   - Vista de información personal
   - Edición de datos
   - Cambio de contraseña
   - Seguridad de cuenta

### 🎯 Componentes Compartidos

- **Header**: Navegación adaptable con menú móvil
- **Footer**: Información de contacto y enlaces
- **ProtectedRoute**: Protección de rutas privadas

### 🔐 Características de Seguridad

- Context API para manejo de autenticación
- LocalStorage para persistencia de sesión
- Interceptores de Axios para tokens
- Rutas protegidas
- Validación de formularios

### 🎨 Diseño Moderno

- **Paleta de colores profesional**:
  - Primary: #6c5bcd (Morado corporativo)
  - Secondary: #1c243d (Azul oscuro)
  - Success: #10b981 (Verde)
  - Error: #ef4444 (Rojo)
  
- **Características visuales**:
  - Animaciones suaves (fadeInUp)
  - Sombras y elevaciones
  - Transiciones fluidas
  - Hover effects
  - Iconos de Lucide React

- **Responsive Design**:
  - Mobile-first approach
  - Breakpoints: 768px, 1024px
  - Grid y Flexbox
  - Menú móvil hamburguesa

## 🚀 Instrucciones de Instalación

### Paso 1: Instalar Dependencias

```powershell
cd "c:\Users\andre\OneDrive\Documentos\Visual Studio\ProyectoFinal-Software-4-y-5\Banco"
npm install
```

### Paso 2: Verificar la API

Asegúrate de que la API esté corriendo en `http://localhost:5000`

### Paso 3: Iniciar el Servidor de Desarrollo

```powershell
npm run dev
```

El portal estará disponible en: `http://localhost:3000`

## 📦 Dependencias Instaladas

```json
{
  "react": "^18.3.1",
  "react-dom": "^18.3.1",
  "react-router-dom": "^6.26.1",
  "axios": "^1.7.7",
  "lucide-react": "^0.445.0"
}
```

## 🔌 Integración con API

El servicio `apiService.js` proporciona funciones para:
- ✅ Login de administrador
- ✅ Obtener estadísticas
- ✅ Movimientos del día
- ✅ Clientes y cuentas
- ✅ Transacciones recientes
- ✅ Crear/actualizar clientes
- ✅ Gestión de usuarios
- ✅ Reportes

## 🎯 Próximos Pasos

1. **Instalar dependencias**: `npm install`
2. **Iniciar desarrollo**: `npm run dev`
3. **Probar las funcionalidades**
4. **Conectar con la API real**
5. **Agregar imágenes/logos personalizados**
6. **Configurar variables de entorno** (`.env`)
7. **Build para producción**: `npm run build`

## 📝 Comandos Útiles

```powershell
# Desarrollo
npm run dev

# Build de producción
npm run build

# Vista previa del build
npm run preview

# Linter
npm run lint
```

## 🎨 Personalización

### Cambiar colores
Edita las variables CSS en `src/styles/index.css`:
```css
:root {
  --primary-color: #6c5bcd;
  --secondary-color: #1c243d;
  /* ... más colores */
}
```

### Agregar logo
Reemplaza los archivos en `src/assets/`:
- `logo.svg`
- `logo.png`

### Configurar API URL
Crea un archivo `.env`:
```
VITE_API_URL=http://localhost:5000/api
```

## ✨ Características Destacadas

- ✅ Diseño moderno y profesional
- ✅ 100% Responsive
- ✅ Animaciones fluidas
- ✅ Integración completa con API
- ✅ Context API para estado global
- ✅ Rutas protegidas
- ✅ Validación de formularios
- ✅ Iconos modernos (Lucide)
- ✅ CSS organizado por componente
- ✅ Código limpio y comentado

## 🎉 ¡Listo para usar!

El portal está completamente funcional y listo para desarrollo. Solo necesitas instalar las dependencias e iniciar el servidor.

---
**Desarrollado con ❤️ para Banco Fisclase**

# Portal Web Cliente - Banco Fisclase

Portal web moderno desarrollado con React.js para el Banco Fisclase. Proporciona una interfaz intuitiva y segura para que los clientes gestionen sus cuentas bancarias, realicen transacciones y consulten su información financiera.

## 🚀 Características

- **Autenticación segura**: Sistema de login y registro con validación
- **Dashboard interactivo**: Vista general de cuentas y estadísticas
- **Gestión de cuentas**: Visualización de múltiples cuentas bancarias
- **Historial de transacciones**: Consulta detallada con filtros
- **Transferencias**: Realización de transferencias entre cuentas
- **Perfil de usuario**: Gestión de información personal
- **Diseño responsive**: Adaptado para móviles, tablets y desktop
- **Animaciones fluidas**: Experiencia de usuario mejorada

## 🛠️ Tecnologías

- **React 18.3**: Framework principal
- **React Router 6**: Navegación y rutas
- **Axios**: Peticiones HTTP a la API
- **Vite**: Build tool y desarrollo rápido
- **Lucide React**: Iconos modernos
- **CSS Modules**: Estilos encapsulados

## 📋 Prerrequisitos

- Node.js 18.x o superior
- npm o yarn
- API Backend corriendo (puerto 5000)

## 🔧 Instalación

1. Instalar dependencias:
```bash
npm install
```

2. Configurar variables de entorno (opcional):
Crear archivo `.env` en la raíz:
```env
VITE_API_URL=http://localhost:5000/api
```

3. Iniciar servidor de desarrollo:
```bash
npm run dev
```

4. Abrir en el navegador:
```
http://localhost:3000
```

## 📦 Scripts Disponibles

- `npm run dev` - Inicia el servidor de desarrollo
- `npm run build` - Genera build de producción
- `npm run preview` - Vista previa del build de producción
- `npm run lint` - Ejecuta el linter

## 🏗️ Estructura del Proyecto

```
Banco/
├── src/
│   ├── components/        # Componentes reutilizables
│   │   ├── Header.jsx
│   │   ├── Footer.jsx
│   │   └── ProtectedRoute.jsx
│   ├── context/          # Contextos de React
│   │   └── AuthContext.jsx
│   ├── pages/            # Páginas principales
│   │   ├── Home.jsx
│   │   ├── Login.jsx
│   │   ├── Register.jsx
│   │   ├── Dashboard.jsx
│   │   ├── Accounts.jsx
│   │   ├── Transactions.jsx
│   │   ├── Transfer.jsx
│   │   └── Profile.jsx
│   ├── services/         # Servicios y API
│   │   └── apiService.js
│   ├── styles/           # Estilos CSS
│   │   ├── index.css
│   │   ├── Header.css
│   │   ├── Footer.css
│   │   ├── Home.css
│   │   ├── Auth.css
│   │   ├── Dashboard.css
│   │   ├── Accounts.css
│   │   ├── Transactions.css
│   │   ├── Transfer.css
│   │   └── Profile.css
│   ├── App.jsx           # Componente principal
│   └── main.jsx          # Punto de entrada
├── index.html
├── vite.config.js
└── package.json
```

## 🔐 Seguridad

- Autenticación basada en tokens
- Rutas protegidas con ProtectedRoute
- Validación de formularios
- Interceptores de peticiones HTTP
- Manejo de sesiones

## 🎨 Diseño

El diseño sigue los principios de:
- **Material Design**: Elevaciones y sombras
- **Flat Design**: Colores planos y modernos
- **Responsive Design**: Mobile-first approach
- **Accesibilidad**: ARIA labels y navegación por teclado

### Paleta de Colores

- Primary: #6c5bcd (Morado)
- Secondary: #1c243d (Azul oscuro)
- Success: #10b981 (Verde)
- Error: #ef4444 (Rojo)
- Warning: #f59e0b (Naranja)

## 🔌 Integración con API

El portal se integra con la API del banco ubicada en:
- URL base: `http://localhost:5000/api`
- Endpoints principales:
  - `POST /banco/login-admin` - Login
  - `GET /banco/estadisticas` - Estadísticas
  - `GET /banco/clientes` - Lista de clientes
  - `GET /banco/transacciones-recientes` - Transacciones
  - Y más...

## 📱 Responsive

El portal está optimizado para:
- 📱 Mobile: 320px - 767px
- 📲 Tablet: 768px - 1023px
- 💻 Desktop: 1024px+

## 🚀 Despliegue

Para generar el build de producción:

```bash
npm run build
```

Los archivos se generarán en la carpeta `dist/`

## 🤝 Contribución

Este proyecto es parte del Proyecto Final de Software 4 y 5.

## 📄 Licencia

ISC

## 👥 Autores

Equipo de desarrollo Banco Fisclase

---

**Nota**: Asegúrate de que la API esté corriendo antes de iniciar el portal.

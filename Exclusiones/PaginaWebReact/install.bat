@echo off
echo ========================================
echo Portal Web Banco Fisclase - Instalacion
echo ========================================
echo.

echo [1/3] Verificando Node.js...
node --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Node.js no esta instalado
    echo Por favor instala Node.js desde https://nodejs.org/
    pause
    exit /b 1
)
echo Node.js instalado correctamente
echo.

echo [2/3] Instalando dependencias...
call npm install
if errorlevel 1 (
    echo ERROR: Fallo la instalacion de dependencias
    pause
    exit /b 1
)
echo.

echo [3/3] Dependencias instaladas correctamente
echo.
echo ========================================
echo Instalacion completada!
echo ========================================
echo.
echo Para iniciar el servidor de desarrollo, ejecuta:
echo   npm run dev
echo.
echo El portal estara disponible en:
echo   http://localhost:3000
echo.
pause

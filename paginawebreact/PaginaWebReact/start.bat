@echo off
echo ========================================
echo Iniciando Portal Web Banco Fisclase
echo ========================================
echo.
echo Verificando dependencias...
if not exist "node_modules" (
    echo ERROR: Dependencias no instaladas
    echo Por favor ejecuta install.bat primero
    pause
    exit /b 1
)
echo.
echo Iniciando servidor de desarrollo...
echo El portal estara disponible en: http://localhost:3000
echo.
echo Presiona Ctrl+C para detener el servidor
echo.
call npm run dev

using ProyectoFinal_Software_4_y_5.Entidades;

namespace ProyectoFinal_Software_4_y_5.Servicios
{
    public class ControladorSession
    {
        private static ControladorSession _instancia;
        public Usuario UsuarioActual { get; private set; }

        private ControladorSession() { }

        public static ControladorSession ObtenerInstancia()
        {
            if (_instancia == null)
            {
                _instancia = new ControladorSession();
            }
            return _instancia;
        }

        public void SetUsuarioActual(Usuario usuario)
        {
            UsuarioActual = usuario;
        }

        public void CerrarSesion()
        {
            UsuarioActual = null;
        }

        public bool HayUsuarioLogueado()
        {
            return UsuarioActual != null;
        }

        public bool EsAdmin()
        {
            return UsuarioActual?.Admin ?? false;
        }
    }
}
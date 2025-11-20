using System;
using System.Threading;
using API_BancoFisclase.Servicios;

namespace API_BancoFisclase.Controllers
{
    // Lightweight session model kept inside the API assembly
    public class SessionUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
    }

    public interface IControladorSesion : IDisposable
    {
        SessionUser UsuarioActual { get; }
        void SetUsuarioActual(UserDto user, bool isAdmin = false);
        void CerrarSesion();
        bool HayUsuarioLogueado();
        bool EsAdmin();
    }

    // Thread-safe, DI-friendly replacement for the original singleton
    public class ControladorSesion : IControladorSesion
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private SessionUser _usuarioActual;

        public SessionUser UsuarioActual
        {
            get
            {
                _lock.EnterReadLock();
                try { return _usuarioActual; }
                finally { _lock.ExitReadLock(); }
            }
        }

        public void SetUsuarioActual(UserDto user, bool isAdmin = false)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            _lock.EnterWriteLock();
            try
            {
                _usuarioActual = new SessionUser
                {
                    Id = user.Id,
                    Username = user.Username,
                    IsAdmin = isAdmin
                };
            }
            finally { _lock.ExitWriteLock(); }
        }

        public void CerrarSesion()
        {
            _lock.EnterWriteLock();
            try { _usuarioActual = null; }
            finally { _lock.ExitWriteLock(); }
        }

        public bool HayUsuarioLogueado()
        {
            _lock.EnterReadLock();
            try { return _usuarioActual != null; }
            finally { _lock.ExitReadLock(); }
        }

        public bool EsAdmin()
        {
            _lock.EnterReadLock();
            try { return _usuarioActual?.IsAdmin ?? false; }
            finally { _lock.ExitReadLock(); }
        }

        public void Dispose()
        {
            _lock?.Dispose();
        }
    }
}
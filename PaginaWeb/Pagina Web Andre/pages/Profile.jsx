import { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import Header from '../components/Header';
import Footer from '../components/Footer';
import { User, Mail, Phone, MapPin, Lock } from 'lucide-react';
import '../styles/Profile.css';

const Profile = () => {
  const { user } = useAuth();
  const [editing, setEditing] = useState(false);
  const [changingPassword, setChangingPassword] = useState(false);
  const [profileData, setProfileData] = useState({
    nombreCompleto: 'Usuario Demo',
    email: 'usuario@fisclase.com',
    telefono: '(555) 123-4567',
    direccion: 'Calle Principal 123, Ciudad, Estado',
  });

  const [passwordData, setPasswordData] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  });

  const handleProfileChange = (e) => {
    setProfileData({
      ...profileData,
      [e.target.name]: e.target.value,
    });
  };

  const handlePasswordChange = (e) => {
    setPasswordData({
      ...passwordData,
      [e.target.name]: e.target.value,
    });
  };

  const handleProfileSubmit = (e) => {
    e.preventDefault();
    // Aquí iría la llamada al API para actualizar el perfil
    setEditing(false);
    alert('Perfil actualizado exitosamente');
  };

  const handlePasswordSubmit = (e) => {
    e.preventDefault();
    
    if (passwordData.newPassword !== passwordData.confirmPassword) {
      alert('Las contraseñas no coinciden');
      return;
    }
    
    // Aquí iría la llamada al API para cambiar la contraseña
    setChangingPassword(false);
    setPasswordData({
      currentPassword: '',
      newPassword: '',
      confirmPassword: '',
    });
    alert('Contraseña cambiada exitosamente');
  };

  return (
    <div className="profile-page">
      <Header />
      
      <main className="profile-content">
        <div className="profile-container">
          <div className="profile-header">
            <div className="profile-avatar">
              <User size={64} />
            </div>
            <h1>{user?.username}</h1>
            <p className="profile-role">{user?.isAdmin ? 'Administrador' : 'Cliente'}</p>
          </div>

          <div className="profile-sections">
            <section className="profile-section">
              <div className="section-header">
                <h2>Información Personal</h2>
                {!editing && (
                  <button className="btn-edit" onClick={() => setEditing(true)}>
                    Editar
                  </button>
                )}
              </div>

              {editing ? (
                <form onSubmit={handleProfileSubmit} className="profile-form">
                  <div className="form-group">
                    <label>
                      <User size={20} />
                      <span>Nombre Completo</span>
                    </label>
                    <input
                      type="text"
                      name="nombreCompleto"
                      value={profileData.nombreCompleto}
                      onChange={handleProfileChange}
                    />
                  </div>

                  <div className="form-group">
                    <label>
                      <Mail size={20} />
                      <span>Correo Electrónico</span>
                    </label>
                    <input
                      type="email"
                      name="email"
                      value={profileData.email}
                      onChange={handleProfileChange}
                    />
                  </div>

                  <div className="form-group">
                    <label>
                      <Phone size={20} />
                      <span>Teléfono</span>
                    </label>
                    <input
                      type="tel"
                      name="telefono"
                      value={profileData.telefono}
                      onChange={handleProfileChange}
                    />
                  </div>

                  <div className="form-group">
                    <label>
                      <MapPin size={20} />
                      <span>Dirección</span>
                    </label>
                    <input
                      type="text"
                      name="direccion"
                      value={profileData.direccion}
                      onChange={handleProfileChange}
                    />
                  </div>

                  <div className="form-actions">
                    <button type="submit" className="btn-save">
                      Guardar Cambios
                    </button>
                    <button type="button" className="btn-cancel" onClick={() => setEditing(false)}>
                      Cancelar
                    </button>
                  </div>
                </form>
              ) : (
                <div className="profile-info">
                  <div className="info-item">
                    <User size={20} />
                    <div>
                      <p className="info-label">Nombre Completo</p>
                      <p className="info-value">{profileData.nombreCompleto}</p>
                    </div>
                  </div>

                  <div className="info-item">
                    <Mail size={20} />
                    <div>
                      <p className="info-label">Correo Electrónico</p>
                      <p className="info-value">{profileData.email}</p>
                    </div>
                  </div>

                  <div className="info-item">
                    <Phone size={20} />
                    <div>
                      <p className="info-label">Teléfono</p>
                      <p className="info-value">{profileData.telefono}</p>
                    </div>
                  </div>

                  <div className="info-item">
                    <MapPin size={20} />
                    <div>
                      <p className="info-label">Dirección</p>
                      <p className="info-value">{profileData.direccion}</p>
                    </div>
                  </div>
                </div>
              )}
            </section>

            <section className="profile-section">
              <div className="section-header">
                <h2>Seguridad</h2>
                {!changingPassword && (
                  <button className="btn-edit" onClick={() => setChangingPassword(true)}>
                    Cambiar Contraseña
                  </button>
                )}
              </div>

              {changingPassword ? (
                <form onSubmit={handlePasswordSubmit} className="profile-form">
                  <div className="form-group">
                    <label>
                      <Lock size={20} />
                      <span>Contraseña Actual</span>
                    </label>
                    <input
                      type="password"
                      name="currentPassword"
                      value={passwordData.currentPassword}
                      onChange={handlePasswordChange}
                      required
                    />
                  </div>

                  <div className="form-group">
                    <label>
                      <Lock size={20} />
                      <span>Nueva Contraseña</span>
                    </label>
                    <input
                      type="password"
                      name="newPassword"
                      value={passwordData.newPassword}
                      onChange={handlePasswordChange}
                      required
                    />
                  </div>

                  <div className="form-group">
                    <label>
                      <Lock size={20} />
                      <span>Confirmar Nueva Contraseña</span>
                    </label>
                    <input
                      type="password"
                      name="confirmPassword"
                      value={passwordData.confirmPassword}
                      onChange={handlePasswordChange}
                      required
                    />
                  </div>

                  <div className="form-actions">
                    <button type="submit" className="btn-save">
                      Cambiar Contraseña
                    </button>
                    <button 
                      type="button" 
                      className="btn-cancel" 
                      onClick={() => setChangingPassword(false)}
                    >
                      Cancelar
                    </button>
                  </div>
                </form>
              ) : (
                <div className="security-info">
                  <p>Tu contraseña fue actualizada hace 30 días</p>
                  <p className="security-tip">
                    💡 Recomendación: Cambia tu contraseña regularmente para mantener tu cuenta segura
                  </p>
                </div>
              )}
            </section>
          </div>
        </div>
      </main>
      
      <Footer />
    </div>
  );
};

export default Profile;

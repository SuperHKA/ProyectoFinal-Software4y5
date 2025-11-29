import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import Header from '../components/Header';
import Footer from '../components/Footer';
import { User, Mail, Phone, MapPin, Lock } from 'lucide-react';
import apiService from '../services/apiService';
import '../styles/Profile.css';

const Profile = () => {
  const { user } = useAuth();
  const [editing, setEditing] = useState(false);
  const [changingPassword, setChangingPassword] = useState(false);
  const [loading, setLoading] = useState(true);
  const [profileData, setProfileData] = useState({
    nombreCompleto: '',
    email: '',
    telefono: '',
    calle: '',
    numero: '',
    ciudad: '',
    estadoProvincia: '',
    codigoPostal: '',
    pais: '',
  });

  const [passwordData, setPasswordData] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  });

  useEffect(() => {
    const loadProfile = async () => {
      if (!user?.id) { setLoading(false); return; }
      try {
        const res = await apiService.getClientes();
        const all = res?.data || [];
        const me = all.find(c => Number(c.usuario_id) === Number(user.id));
        if (me) {
          setProfileData({
            nombreCompleto: me.nombre_completo ?? '',
            email: me.email ?? '',
            telefono: me.telefono ?? '',
            calle: me.calle ?? '',
            numero: me.numero ?? '',
            ciudad: me.ciudad ?? '',
            estadoProvincia: me.estado_provincia ?? '',
            codigoPostal: me.codigo_postal ?? '',
            pais: me.pais ?? ''
          });
        }
      } catch (err) {
        console.error('Error loading profile', err);
      } finally {
        setLoading(false);
      }
    };
    loadProfile();
  }, [user]);

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

  const handleProfileSubmit = async (e) => {
    e.preventDefault();
    if (!user?.id) return alert('Usuario no identificado');

    try {
      const payload = {
        Nombre: profileData.nombreCompleto,
        Email: profileData.email,
        Telefono: profileData.telefono,
        Calle: profileData.calle,
        Numero: profileData.numero,
        Ciudad: profileData.ciudad,
        EstadoProvincia: profileData.estadoProvincia,
        CodigoPostal: profileData.codigoPostal,
        Pais: profileData.pais
      };
      const res = await apiService.actualizarCliente(user.id, payload);
      if (res?.data === true || res?.status === 200) {
        setEditing(false);
        alert('Perfil actualizado exitosamente');
      } else {
        alert('Error actualizando perfil');
      }
    } catch (err) {
      console.error('update profile error', err);
      alert('Error al actualizar perfil');
    }
  };

  const handlePasswordSubmit = async (e) => {
    e.preventDefault();

    if (passwordData.newPassword !== passwordData.confirmPassword) {
      alert('Las contraseñas no coinciden');
      return;
    }

    if (!user?.id) return alert('Usuario no identificado');

    try {
      const res = await apiService.cambiarContrasena(user.id, passwordData.newPassword);
      // API returns body true on success
      if (res?.data === true || res?.status === 200) {
        setChangingPassword(false);
        setPasswordData({ currentPassword: '', newPassword: '', confirmPassword: '' });
        alert('Contraseña cambiada exitosamente');
      } else {
        alert('Error al cambiar la contraseña');
      }
    } catch (err) {
      console.error('change password error', err);
      alert('Error al cambiar la contraseña');
    }
  };

  if (loading) return (
    <div className="profile-page">
      <Header />
      <div className="loading-container"><div className="spinner"></div><p>Cargando...</p></div>
      <Footer />
    </div>
  );

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
                      <span>Calle</span>
                    </label>
                    <input
                      type="text"
                      name="calle"
                      value={profileData.calle}
                      onChange={handleProfileChange}
                    />
                  </div>

                  <div className="form-row">
                    <div className="form-group">
                      <label>Numero</label>
                      <input type="text" name="numero" value={profileData.numero} onChange={handleProfileChange} />
                    </div>

                    <div className="form-group">
                      <label>Ciudad</label>
                      <input type="text" name="ciudad" value={profileData.ciudad} onChange={handleProfileChange} />
                    </div>
                  </div>

                  <div className="form-row">
                    <div className="form-group">
                      <label>Estado/Provincia</label>
                      <input type="text" name="estadoProvincia" value={profileData.estadoProvincia} onChange={handleProfileChange} />
                    </div>

                    <div className="form-group">
                      <label>Código Postal</label>
                      <input type="text" name="codigoPostal" value={profileData.codigoPostal} onChange={handleProfileChange} />
                    </div>
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
                      <p className="info-value">{`${profileData.calle} ${profileData.numero}, ${profileData.ciudad}`}</p>
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

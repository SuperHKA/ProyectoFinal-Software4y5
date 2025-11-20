import { Link } from 'react-router-dom';
import Header from '../components/Header';
import Footer from '../components/Footer';
import { Shield, Clock, Smartphone, TrendingUp } from 'lucide-react';
import '../styles/Home.css';

const Home = () => {
  return (
    <div className="home-page">
      <Header />
      
      <section className="hero-section">
        <div className="hero-content">
          <h1>Bienvenido a Banco Fisclase</h1>
          <p className="hero-subtitle">Con Fisclase, tus ahorros están seguros</p>
          <p className="hero-description">Siempre estamos para ti.</p>
          <div className="hero-buttons">
            <Link to="/login" className="btn-primary">Iniciar Sesión</Link>
            <Link to="/register" className="btn-secondary">Abrir Cuenta</Link>
          </div>
        </div>
      </section>

      <section className="services-section">
        <h2 className="section-title">Nuestros Servicios Digitales</h2>
        
        <div className="services-grid">
          <div className="service-card">
            <div className="service-icon">
              <Shield size={48} />
            </div>
            <h3>Banca en Línea</h3>
            <p>Accede a tus servicios bancarios desde cualquier lugar, de forma rápida y segura.</p>
            <ul className="service-features">
              <li>✓ Seguridad: Protege tu información con las mejores tecnologías.</li>
              <li>✓ Accesibilidad: Conéctate cuando lo necesites.</li>
              <li>✓ Eficiencia: Realiza operaciones con facilidad.</li>
            </ul>
          </div>

          <div className="service-card">
            <div className="service-icon">
              <TrendingUp size={48} />
            </div>
            <h3>Transferencias y Pagos</h3>
            <p>Realiza pagos y transferencias de manera segura y sin complicaciones.</p>
            <ul className="service-features">
              <li>✓ Rapidez: Envía dinero al instante.</li>
              <li>✓ Comodidad: Hazlo todo desde tu dispositivo.</li>
              <li>✓ Flexibilidad: Transfiere entre cuentas y paga tus servicios fácilmente.</li>
            </ul>
          </div>

          <div className="service-card">
            <div className="service-icon">
              <Clock size={48} />
            </div>
            <h3>Consultas de Saldo e Historial</h3>
            <p>Consulta tu saldo y revisa el historial de tus transacciones al instante.</p>
            <ul className="service-features">
              <li>✓ Control: Mantén un registro claro de tus finanzas.</li>
              <li>✓ Transparencia: Consulta tu historial en cualquier momento.</li>
              <li>✓ Facilidad: Todo lo que necesitas al alcance de tu mano.</li>
            </ul>
          </div>
        </div>
      </section>

      <section className="features-section">
        <h2 className="section-title">¿Por qué elegir Fisclase?</h2>
        <div className="features-grid">
          <div className="feature-item">
            <Smartphone size={40} />
            <h4>App Móvil</h4>
            <p>Gestiona tus finanzas desde tu smartphone</p>
          </div>
          <div className="feature-item">
            <Shield size={40} />
            <h4>Seguridad Avanzada</h4>
            <p>Protección de última generación para tus datos</p>
          </div>
          <div className="feature-item">
            <Clock size={40} />
            <h4>Disponible 24/7</h4>
            <p>Accede a tus cuentas en cualquier momento</p>
          </div>
          <div className="feature-item">
            <TrendingUp size={40} />
            <h4>Mejores Tasas</h4>
            <p>Rendimientos competitivos para tus ahorros</p>
          </div>
        </div>
      </section>

      <Footer />
    </div>
  );
};

export default Home;

import { useState } from 'react';
import Header from '../components/Header';
import Footer from '../components/Footer';
import { Send } from 'lucide-react';
import '../styles/Transfer.css';

const Transfer = () => {
  const [formData, setFormData] = useState({
    cuentaOrigen: '',
    cuentaDestino: '',
    monto: '',
    concepto: '',
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
    setError('');
    setSuccess(false);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess(false);

    if (!formData.cuentaOrigen || !formData.cuentaDestino || !formData.monto) {
      setError('Por favor completa todos los campos obligatorios');
      return;
    }

    if (parseFloat(formData.monto) <= 0) {
      setError('El monto debe ser mayor a cero');
      return;
    }

    setLoading(true);

    try {
      // Aquí iría la llamada al API para realizar la transferencia
      // await apiService.realizarTransferencia(formData);
      
      // Simulación
      await new Promise(resolve => setTimeout(resolve, 1500));
      
      setSuccess(true);
      setFormData({
        cuentaOrigen: '',
        cuentaDestino: '',
        monto: '',
        concepto: '',
      });
    } catch (err) {
      setError('Error al realizar la transferencia. Intenta de nuevo.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="transfer-page">
      <Header />
      
      <main className="transfer-content">
        <div className="transfer-container">
          <div className="transfer-header">
            <Send size={48} className="transfer-icon" />
            <h1>Realizar Transferencia</h1>
            <p>Envía dinero de forma rápida y segura</p>
          </div>

          <form onSubmit={handleSubmit} className="transfer-form">
            {error && <div className="error-message">{error}</div>}
            {success && <div className="success-message">¡Transferencia realizada exitosamente!</div>}

            <div className="form-group">
              <label htmlFor="cuentaOrigen">Cuenta de Origen *</label>
              <select
                id="cuentaOrigen"
                name="cuentaOrigen"
                value={formData.cuentaOrigen}
                onChange={handleChange}
                required
              >
                <option value="">Selecciona una cuenta</option>
                <option value="1234-5678-9012">Cuenta Ahorro - ****9012 ($5,000.00)</option>
                <option value="9876-5432-1098">Cuenta Corriente - ****1098 ($12,500.00)</option>
              </select>
            </div>

            <div className="form-group">
              <label htmlFor="cuentaDestino">Cuenta de Destino *</label>
              <input
                type="text"
                id="cuentaDestino"
                name="cuentaDestino"
                value={formData.cuentaDestino}
                onChange={handleChange}
                placeholder="Ingresa el número de cuenta destino"
                required
              />
            </div>

            <div className="form-group">
              <label htmlFor="monto">Monto *</label>
              <div className="input-with-prefix">
                <span className="prefix">$</span>
                <input
                  type="number"
                  id="monto"
                  name="monto"
                  value={formData.monto}
                  onChange={handleChange}
                  placeholder="0.00"
                  step="0.01"
                  min="0"
                  required
                />
              </div>
            </div>

            <div className="form-group">
              <label htmlFor="concepto">Concepto</label>
              <textarea
                id="concepto"
                name="concepto"
                value={formData.concepto}
                onChange={handleChange}
                placeholder="Descripción de la transferencia (opcional)"
                rows="3"
              />
            </div>

            <button type="submit" className="btn-transfer" disabled={loading}>
              {loading ? 'Procesando...' : 'Realizar Transferencia'}
            </button>
          </form>

          <div className="transfer-info">
            <h3>Información Importante</h3>
            <ul>
              <li>Las transferencias se procesan de forma inmediata</li>
              <li>Verifica que la cuenta destino sea correcta</li>
              <li>Límite diario de transferencias: $50,000.00</li>
              <li>Comisión: $0.00 (sin costo)</li>
            </ul>
          </div>
        </div>
      </main>
      
      <Footer />
    </div>
  );
};

export default Transfer;

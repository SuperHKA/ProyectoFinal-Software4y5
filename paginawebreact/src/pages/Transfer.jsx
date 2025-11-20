import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import Header from '../components/Header';
import Footer from '../components/Footer';
import { Send } from 'lucide-react';
import apiService from '../services/apiService';
import '../styles/Transfer.css';

const Transfer = () => {
  const { user } = useAuth();

  const [formData, setFormData] = useState({
    tipo: 'internal',
    cuentaOrigen: '',
    cuentaDestino: '',
    monto: '',
    concepto: '',
  });
  const [loading, setLoading] = useState(false);
  const [fieldErrors, setFieldErrors] = useState({});
  const [toasts, setToasts] = useState([]);
  const [success, setSuccess] = useState(null);

  useEffect(() => {
    // auto-dismiss toasts after 4s
    if (toasts.length === 0) return;
    const t = setTimeout(() => setToasts(prev => prev.slice(1)), 4000);
    return () => clearTimeout(t);
  }, [toasts]);

  const pushToast = (message, type = 'info') => {
    setToasts(prev => [...prev, { id: Date.now(), message, type }]);
  };

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
    setFieldErrors(prev => ({ ...prev, [e.target.name]: null }));
    setSuccess(null);
  };

  // Accept either numeric id, 10-digit account number or grouped with dashes (4-4-4 etc)
  const isValidAccountIdentifier = (value) => {
    if (!value) return false;
    const trimmed = value.trim();
    // numeric id
    if (/^\d+$/.test(trimmed)) return true;
    // 10 digits
    if (/^\d{10}$/.test(trimmed)) return true;
    // groups like 4-4-4 or 4-4-4-2 etc
    if (/^\d{2,4}(-\d{2,4})+$/.test(trimmed)) return true;
    return false;
  };

  const validate = () => {
    const errs = {};
    if (!formData.cuentaOrigen) errs.cuentaOrigen = 'Cuenta de origen requerida';
    else if (!isValidAccountIdentifier(formData.cuentaOrigen)) errs.cuentaOrigen = 'Formato de cuenta inválido';

    if (!formData.cuentaDestino) errs.cuentaDestino = 'Cuenta de destino requerida';
    else if (!isValidAccountIdentifier(formData.cuentaDestino)) errs.cuentaDestino = 'Formato de cuenta inválido';

    if (!formData.monto) errs.monto = 'Monto requerido';
    else if (isNaN(parseFloat(formData.monto)) || parseFloat(formData.monto) <= 0) errs.monto = 'Monto debe ser mayor a 0';

    setFieldErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSuccess(null);
    setToasts([]);

    if (!validate()) {
      pushToast('Corrige los errores del formulario', 'error');
      return;
    }

    setLoading(true);

    try {
      const payload = {
        CuentaOrigen: formData.cuentaOrigen.trim(),
        CuentaDestino: formData.cuentaDestino.trim(),
        Monto: parseFloat(formData.monto),
        Descripcion: formData.concepto,
        UsuarioId: user?.id ?? null
      };

      let res;
      if (formData.tipo === 'internal') {
        res = await apiService.transferInternal(payload);
      } else {
        res = await apiService.transferExternal(payload);
      }

      if (res?.data?.success) {
        setSuccess(`Transferencia realizada. ID: ${res.data.transaccionId}`);
        pushToast('Transferencia realizada correctamente', 'success');
        setFormData({ tipo: 'internal', cuentaOrigen: '', cuentaDestino: '', monto: '', concepto: '' });
      } else {
        const msg = res?.data?.message || 'Error en la transferencia';
        pushToast(msg, 'error');
        setFieldErrors(prev => ({ ...prev, form: msg }));
      }
    } catch (err) {
      console.error('transfer error', err);
      const message = err?.response?.data?.message ?? err?.response?.data ?? err.message ?? 'Error al realizar la transferencia';
      pushToast(message, 'error');
      setFieldErrors(prev => ({ ...prev, form: message }));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="transfer-page">
      <Header />

      {/* Toasts */}
      <div className="toasts-container" aria-live="polite">
        {toasts.map(t => (
          <div key={t.id} className={`toast toast-${t.type}`}>
            {t.message}
          </div>
        ))}
      </div>

      <main className="transfer-content">
        <div className="transfer-container">
          <div className="transfer-header">
            <Send size={48} className="transfer-icon" />
            <h1>Realizar Transferencia</h1>
            <p>Envía dinero de forma rápida y segura</p>
          </div>

          <form onSubmit={handleSubmit} className="transfer-form" noValidate>
            {fieldErrors.form && <div className="error-message">{fieldErrors.form}</div>}
            {success && <div className="success-message">{success}</div>}

            <div className="form-group">
              <label>Tipo de transferencia</label>
              <select name="tipo" value={formData.tipo} onChange={handleChange} disabled={loading}>
                <option value="internal">Interna</option>
                <option value="external">Externa</option>
              </select>
            </div>

            <div className="form-group">
              <label htmlFor="cuentaOrigen">Cuenta de Origen *</label>
              <input
                id="cuentaOrigen"
                name="cuentaOrigen"
                value={formData.cuentaOrigen}
                onChange={handleChange}
                placeholder="Número o Id de cuenta origen"
                required
                disabled={loading}
                aria-invalid={!!fieldErrors.cuentaOrigen}
              />
              {fieldErrors.cuentaOrigen && <div className="field-error">{fieldErrors.cuentaOrigen}</div>}
            </div>

            <div className="form-group">
              <label htmlFor="cuentaDestino">Cuenta de Destino *</label>
              <input
                type="text"
                id="cuentaDestino"
                name="cuentaDestino"
                value={formData.cuentaDestino}
                onChange={handleChange}
                placeholder="Número o Id de cuenta destino"
                required
                disabled={loading}
                aria-invalid={!!fieldErrors.cuentaDestino}
              />
              {fieldErrors.cuentaDestino && <div className="field-error">{fieldErrors.cuentaDestino}</div>}
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
                  disabled={loading}
                  aria-invalid={!!fieldErrors.monto}
                />
              </div>
              {fieldErrors.monto && <div className="field-error">{fieldErrors.monto}</div>}
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
                disabled={loading}
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

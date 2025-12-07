document.addEventListener('DOMContentLoaded', function() {
    
    // Obtiene el elemento canvas donde se dibujará la gráfica
    const ctx = document.getElementById('monthlyChart');
    
    // Verifica que el elemento exista antes de crear la gráfica
    if (!ctx) {
        console.error('No se encontró el elemento canvas con id "monthlyChart"');
        return;
    }

    // Configuración de la gráfica de barras
    const monthlyChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            datasets: [
                {
                    label: 'Ingresos',
                    data: [3200, 2800, 3500, 4100, 3800, 4200, 3900, 4500, 4200, 4800, 5240, 0],
                    backgroundColor: 'rgba(40, 167, 69, 0.7)',
                    borderColor: 'rgba(40, 167, 69, 1)',
                    borderWidth: 2,
                    borderRadius: 8,
                    hoverBackgroundColor: 'rgba(40, 167, 69, 0.9)'
                },
                {
                    label: 'Gastos',
                    data: [2400, 2100, 2800, 3200, 2900, 3100, 2700, 3400, 3100, 3600, 3890, 0],
                    backgroundColor: 'rgba(220, 53, 69, 0.7)',
                    borderColor: 'rgba(220, 53, 69, 1)',
                    borderWidth: 2,
                    borderRadius: 8,
                    hoverBackgroundColor: 'rgba(220, 53, 69, 0.9)'
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                intersect: false,
                mode: 'index'
            },
            plugins: {
                legend: {
                    position: 'top',
                    labels: {
                        font: {
                            size: 14,
                            family: 'Poppins',
                            weight: '600'
                        },
                        padding: 20,
                        usePointStyle: true,
                        pointStyle: 'circle'
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    padding: 12,
                    titleFont: {
                        size: 14,
                        family: 'Poppins',
                        weight: '600'
                    },
                    bodyFont: {
                        size: 13,
                        family: 'Poppins'
                    },
                    borderColor: 'rgba(255, 255, 255, 0.2)',
                    borderWidth: 1,
                    callbacks: {
                        label: function(context) {
                            let label = context.dataset.label || '';
                            if (label) {
                                label += ': ';
                            }
                            label += '$' + context.parsed.y.toLocaleString('es-PA', {
                                minimumFractionDigits: 2,
                                maximumFractionDigits: 2
                            });
                            return label;
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function(value) {
                            return '$' + value.toLocaleString('es-PA');
                        },
                        font: {
                            size: 12,
                            family: 'Poppins'
                        },
                        color: '#666'
                    },
                    grid: {
                        color: 'rgba(0, 0, 0, 0.05)',
                        drawBorder: false
                    }
                },
                x: {
                    ticks: {
                        font: {
                            size: 12,
                            family: 'Poppins'
                        },
                        color: '#666'
                    },
                    grid: {
                        display: false,
                        drawBorder: false
                    }
                }
            }
        }
    });

    // Opcional: Función para actualizar los datos de la gráfica dinámicamente
    window.updateChartData = function(newIncomeData, newExpensesData) {
        monthlyChart.data.datasets[0].data = newIncomeData;
        monthlyChart.data.datasets[1].data = newExpensesData;
        monthlyChart.update();
    };

    console.log('Gráfica de resumen mensual cargada correctamente');
});

// Password Strength Indicator and Toggle Visibility Functions

/**
 * Calcula la fortaleza de una contraseña basada en varios criterios
 * @param {string} password - La contraseña a evaluar
 * @returns {object} - Objeto con score, level y color
 */
function calculatePasswordStrength(password) {
    if (!password) {
        return {
            score: 0,
            level: 'Sin contraseña',
            color: '#dc3545',
            percentage: 0
        };
    }

    let score = 0;
    let feedback = [];

    // Longitud mínima (0-25 puntos)
    if (password.length >= 8) {
        score += 25;
    } else if (password.length >= 6) {
        score += 15;
        feedback.push('Muy corta');
    } else {
        feedback.push('Demasiado corta');
    }

    // Contiene minúsculas (0-15 puntos)
    if (/[a-z]/.test(password)) {
        score += 15;
    } else {
        feedback.push('Falta minúsculas');
    }

    // Contiene mayúsculas (0-15 puntos)
    if (/[A-Z]/.test(password)) {
        score += 15;
    } else {
        feedback.push('Falta mayúsculas');
    }

    // Contiene números (0-15 puntos)
    if (/[0-9]/.test(password)) {
        score += 15;
    } else {
        feedback.push('Falta números');
    }

    // Contiene caracteres especiales (0-20 puntos)
    if (/[^A-Za-z0-9]/.test(password)) {
        score += 20;
    } else {
        feedback.push('Falta símbolos');
    }

    // Longitud extra (0-10 puntos)
    if (password.length >= 12) {
        score += 10;
    }

    // Determinar nivel y color
    let level, color;
    if (score >= 80) {
        level = 'Muy fuerte';
        color = '#198754'; // Verde
    } else if (score >= 60) {
        level = 'Fuerte';
        color = '#20c997'; // Verde claro
    } else if (score >= 40) {
        level = 'Moderada';
        color = '#ffc107'; // Amarillo
    } else if (score >= 20) {
        level = 'Débil';
        color = '#fd7e14'; // Naranja
    } else {
        level = 'Muy débil';
        color = '#dc3545'; // Rojo
    }

    return {
        score: Math.min(score, 100),
        level: level,
        color: color,
        percentage: Math.min(score, 100),
        feedback: feedback
    };
}

/**
 * Actualiza el indicador visual de fortaleza de contraseña
 * @param {string} password - La contraseña a evaluar
 * @param {string} barId - ID del elemento de la barra de progreso
 * @param {string} textId - ID del elemento de texto
 */
function updatePasswordStrength(password, barId = 'passwordStrengthBar', textId = 'passwordStrengthText') {
    const strengthData = calculatePasswordStrength(password);
    const bar = document.getElementById(barId);
    const text = document.getElementById(textId);

    if (bar) {
        bar.style.width = strengthData.percentage + '%';
        bar.style.backgroundColor = strengthData.color;
        bar.setAttribute('aria-valuenow', strengthData.percentage);
        
        // Agregar clase de transición suave
        bar.style.transition = 'width 0.3s ease, background-color 0.3s ease';
    }

    if (text) {
        text.textContent = strengthData.level;
        text.style.color = strengthData.color;
        text.style.fontWeight = '600';
    }
}

/**
 * Alterna la visibilidad de un campo de contraseña
 * @param {string} inputId - ID del campo de contraseña
 * @param {string} iconId - ID del icono (opcional)
 */
function togglePasswordVisibility(inputId, iconId = null) {
    const input = document.getElementById(inputId);
    if (!input) return;

    const isPassword = input.type === 'password';
    input.type = isPassword ? 'text' : 'password';

    // Cambiar icono si se proporciona
    if (iconId) {
        const icon = document.getElementById(iconId);
        if (icon) {
            if (isPassword) {
                icon.className = 'fa fa-eye-slash';
                icon.title = 'Ocultar contraseña';
            } else {
                icon.className = 'fa fa-eye';
                icon.title = 'Mostrar contraseña';
            }
        }
    }
}

/**
 * Inicializa los event listeners para el indicador de fortaleza
 * @param {string} passwordInputId - ID del campo de contraseña
 * @param {string} barId - ID de la barra de progreso
 * @param {string} textId - ID del texto de fortaleza
 */
function initPasswordStrength(passwordInputId, barId = 'passwordStrengthBar', textId = 'passwordStrengthText') {
    const passwordInput = document.getElementById(passwordInputId);
    if (!passwordInput) return;

    // Event listener para actualizar en tiempo real
    passwordInput.addEventListener('input', function() {
        updatePasswordStrength(this.value, barId, textId);
    });

    // Event listener para cuando se enfoca el campo
    passwordInput.addEventListener('focus', function() {
        const strengthContainer = document.querySelector('.password-strength-container');
        if (strengthContainer) {
            strengthContainer.style.display = 'block';
        }
    });

    // Inicializar con valor actual
    updatePasswordStrength(passwordInput.value, barId, textId);
}

/**
 * Limpia el indicador de fortaleza
 * @param {string} barId - ID de la barra de progreso
 * @param {string} textId - ID del texto de fortaleza
 */
function clearPasswordStrength(barId = 'passwordStrengthBar', textId = 'passwordStrengthText') {
    const bar = document.getElementById(barId);
    const text = document.getElementById(textId);

    if (bar) {
        bar.style.width = '0%';
        bar.style.backgroundColor = '#e9ecef';
        bar.setAttribute('aria-valuenow', '0');
    }

    if (text) {
        text.textContent = '-';
        text.style.color = '#6c757d';
        text.style.fontWeight = 'normal';
    }
}

// Funciones globales para uso desde Blazor
window.passwordStrength = {
    calculate: calculatePasswordStrength,
    update: updatePasswordStrength,
    init: initPasswordStrength,
    clear: clearPasswordStrength,
    toggleVisibility: togglePasswordVisibility
};
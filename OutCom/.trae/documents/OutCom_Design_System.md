# Sistema de Diseño - OutCom File Manager

## 1. Visión General del Sistema de Diseño

Este documento define el sistema de diseño completo para el rediseño de OutCom, estableciendo los fundamentos visuales, componentes y patrones de interacción que garantizan una experiencia de usuario consistente y moderna.

### Principios de Diseño
- **Minimalismo**: Interfaces limpias que eliminan elementos innecesarios
- **Claridad**: Jerarquía visual clara y contenido fácil de escanear
- **Consistencia**: Patrones reutilizables en toda la aplicación
- **Accesibilidad**: Cumplimiento con WCAG 2.1 AA
- **Responsividad**: Experiencia óptima en todos los dispositivos

## 2. Tokens de Diseño

### 2.1 Paleta de Colores

```css
:root {
  /* Colores Primarios */
  --color-primary-50: #eff6ff;
  --color-primary-100: #dbeafe;
  --color-primary-200: #bfdbfe;
  --color-primary-300: #93c5fd;
  --color-primary-400: #60a5fa;
  --color-primary-500: #3b82f6;
  --color-primary-600: #2563eb;
  --color-primary-700: #1d4ed8;
  --color-primary-800: #1e40af;
  --color-primary-900: #1e3a8a;
  
  /* Colores Neutros */
  --color-gray-50: #f9fafb;
  --color-gray-100: #f3f4f6;
  --color-gray-200: #e5e7eb;
  --color-gray-300: #d1d5db;
  --color-gray-400: #9ca3af;
  --color-gray-500: #6b7280;
  --color-gray-600: #4b5563;
  --color-gray-700: #374151;
  --color-gray-800: #1f2937;
  --color-gray-900: #111827;
  
  /* Colores Semánticos */
  --color-success: #10b981;
  --color-warning: #f59e0b;
  --color-error: #ef4444;
  --color-info: #3b82f6;
  
  /* Colores de Fondo */
  --bg-primary: #ffffff;
  --bg-secondary: #f9fafb;
  --bg-tertiary: #f3f4f6;
  --bg-overlay: rgba(0, 0, 0, 0.5);
}
```

### 2.2 Tipografía

```css
:root {
  /* Familia de Fuentes */
  --font-family-primary: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
  --font-family-mono: 'JetBrains Mono', 'Fira Code', monospace;
  
  /* Tamaños de Fuente */
  --font-size-xs: 0.75rem;    /* 12px */
  --font-size-sm: 0.875rem;   /* 14px */
  --font-size-base: 1rem;     /* 16px */
  --font-size-lg: 1.125rem;   /* 18px */
  --font-size-xl: 1.25rem;    /* 20px */
  --font-size-2xl: 1.5rem;    /* 24px */
  --font-size-3xl: 1.875rem;  /* 30px */
  --font-size-4xl: 2.25rem;   /* 36px */
  
  /* Pesos de Fuente */
  --font-weight-normal: 400;
  --font-weight-medium: 500;
  --font-weight-semibold: 600;
  --font-weight-bold: 700;
  
  /* Altura de Línea */
  --line-height-tight: 1.25;
  --line-height-normal: 1.5;
  --line-height-relaxed: 1.75;
}
```

### 2.3 Espaciado y Dimensiones

```css
:root {
  /* Espaciado Base (8px system) */
  --space-1: 0.25rem;   /* 4px */
  --space-2: 0.5rem;    /* 8px */
  --space-3: 0.75rem;   /* 12px */
  --space-4: 1rem;      /* 16px */
  --space-5: 1.25rem;   /* 20px */
  --space-6: 1.5rem;    /* 24px */
  --space-8: 2rem;      /* 32px */
  --space-10: 2.5rem;   /* 40px */
  --space-12: 3rem;     /* 48px */
  --space-16: 4rem;     /* 64px */
  
  /* Radios de Borde */
  --radius-sm: 0.25rem;  /* 4px */
  --radius-md: 0.5rem;   /* 8px */
  --radius-lg: 0.75rem;  /* 12px */
  --radius-xl: 1rem;     /* 16px */
  --radius-full: 9999px;
  
  /* Sombras */
  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
  --shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
  --shadow-xl: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
}
```

## 3. Componentes Base

### 3.1 Botones

```css
/* Botón Base */
.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: var(--space-2);
  padding: var(--space-3) var(--space-4);
  font-family: var(--font-family-primary);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
  line-height: var(--line-height-tight);
  border: 1px solid transparent;
  border-radius: var(--radius-md);
  cursor: pointer;
  transition: all 0.2s ease-in-out;
  text-decoration: none;
  user-select: none;
}

/* Variantes de Botón */
.btn--primary {
  background-color: var(--color-primary-600);
  color: white;
  border-color: var(--color-primary-600);
}

.btn--primary:hover {
  background-color: var(--color-primary-700);
  border-color: var(--color-primary-700);
}

.btn--secondary {
  background-color: white;
  color: var(--color-gray-700);
  border-color: var(--color-gray-300);
}

.btn--secondary:hover {
  background-color: var(--color-gray-50);
  border-color: var(--color-gray-400);
}

.btn--ghost {
  background-color: transparent;
  color: var(--color-gray-600);
  border-color: transparent;
}

.btn--ghost:hover {
  background-color: var(--color-gray-100);
  color: var(--color-gray-700);
}

/* Tamaños de Botón */
.btn--sm {
  padding: var(--space-2) var(--space-3);
  font-size: var(--font-size-xs);
}

.btn--lg {
  padding: var(--space-4) var(--space-6);
  font-size: var(--font-size-base);
}
```

### 3.2 Inputs y Formularios

```css
/* Input Base */
.input {
  width: 100%;
  padding: var(--space-3) var(--space-4);
  font-family: var(--font-family-primary);
  font-size: var(--font-size-sm);
  line-height: var(--line-height-normal);
  color: var(--color-gray-900);
  background-color: white;
  border: 1px solid var(--color-gray-300);
  border-radius: var(--radius-md);
  transition: all 0.2s ease-in-out;
}

.input:focus {
  outline: none;
  border-color: var(--color-primary-500);
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.input:disabled {
  background-color: var(--color-gray-50);
  color: var(--color-gray-500);
  cursor: not-allowed;
}

/* Label */
.label {
  display: block;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
  color: var(--color-gray-700);
  margin-bottom: var(--space-2);
}

/* Grupo de Campo */
.field-group {
  margin-bottom: var(--space-6);
}

/* Mensaje de Error */
.field-error {
  margin-top: var(--space-2);
  font-size: var(--font-size-xs);
  color: var(--color-error);
}
```

### 3.3 Cards y Contenedores

```css
/* Card Base */
.card {
  background-color: white;
  border: 1px solid var(--color-gray-200);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  overflow: hidden;
}

.card:hover {
  box-shadow: var(--shadow-md);
}

/* Header de Card */
.card__header {
  padding: var(--space-6);
  border-bottom: 1px solid var(--color-gray-200);
}

.card__title {
  font-size: var(--font-size-lg);
  font-weight: var(--font-weight-semibold);
  color: var(--color-gray-900);
  margin: 0;
}

/* Contenido de Card */
.card__content {
  padding: var(--space-6);
}

/* Footer de Card */
.card__footer {
  padding: var(--space-4) var(--space-6);
  background-color: var(--color-gray-50);
  border-top: 1px solid var(--color-gray-200);
}
```

## 4. Layout y Grid System

### 4.1 Container System

```css
/* Container Principal */
.container {
  width: 100%;
  max-width: 1280px;
  margin: 0 auto;
  padding: 0 var(--space-4);
}

@media (min-width: 640px) {
  .container {
    padding: 0 var(--space-6);
  }
}

@media (min-width: 1024px) {
  .container {
    padding: 0 var(--space-8);
  }
}

/* Container Fluido */
.container-fluid {
  width: 100%;
  padding: 0 var(--space-4);
}
```

### 4.2 Grid System

```css
/* Grid Base */
.grid {
  display: grid;
  gap: var(--space-6);
}

/* Columnas Responsivas */
.grid--cols-1 { grid-template-columns: repeat(1, 1fr); }
.grid--cols-2 { grid-template-columns: repeat(2, 1fr); }
.grid--cols-3 { grid-template-columns: repeat(3, 1fr); }
.grid--cols-4 { grid-template-columns: repeat(4, 1fr); }

@media (min-width: 640px) {
  .grid--sm-cols-2 { grid-template-columns: repeat(2, 1fr); }
  .grid--sm-cols-3 { grid-template-columns: repeat(3, 1fr); }
}

@media (min-width: 768px) {
  .grid--md-cols-2 { grid-template-columns: repeat(2, 1fr); }
  .grid--md-cols-3 { grid-template-columns: repeat(3, 1fr); }
  .grid--md-cols-4 { grid-template-columns: repeat(4, 1fr); }
}

@media (min-width: 1024px) {
  .grid--lg-cols-3 { grid-template-columns: repeat(3, 1fr); }
  .grid--lg-cols-4 { grid-template-columns: repeat(4, 1fr); }
  .grid--lg-cols-6 { grid-template-columns: repeat(6, 1fr); }
}
```

### 4.3 Flexbox Utilities

```css
/* Flex Container */
.flex {
  display: flex;
}

.flex--col {
  flex-direction: column;
}

.flex--wrap {
  flex-wrap: wrap;
}

/* Justify Content */
.justify-start { justify-content: flex-start; }
.justify-center { justify-content: center; }
.justify-end { justify-content: flex-end; }
.justify-between { justify-content: space-between; }
.justify-around { justify-content: space-around; }

/* Align Items */
.items-start { align-items: flex-start; }
.items-center { align-items: center; }
.items-end { align-items: flex-end; }
.items-stretch { align-items: stretch; }

/* Gap */
.gap-2 { gap: var(--space-2); }
.gap-4 { gap: var(--space-4); }
.gap-6 { gap: var(--space-6); }
.gap-8 { gap: var(--space-8); }
```

## 5. Componentes Específicos del File Manager

### 5.1 File Explorer

```css
/* Contenedor Principal del Explorador */
.file-explorer {
  display: grid;
  grid-template-columns: 280px 1fr;
  height: calc(100vh - 64px); /* Altura menos header */
  background-color: var(--bg-primary);
}

@media (max-width: 768px) {
  .file-explorer {
    grid-template-columns: 1fr;
  }
  
  .file-explorer__sidebar {
    display: none;
  }
  
  .file-explorer__sidebar--mobile-open {
    display: block;
    position: fixed;
    top: 64px;
    left: 0;
    width: 280px;
    height: calc(100vh - 64px);
    z-index: 50;
    background-color: white;
    box-shadow: var(--shadow-lg);
  }
}

/* Sidebar del Árbol de Archivos */
.file-explorer__sidebar {
  background-color: var(--bg-secondary);
  border-right: 1px solid var(--color-gray-200);
  overflow-y: auto;
}

/* Área Principal de Contenido */
.file-explorer__main {
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

/* Toolbar */
.file-explorer__toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-4) var(--space-6);
  background-color: white;
  border-bottom: 1px solid var(--color-gray-200);
}

/* Vista de Archivos */
.file-explorer__content {
  flex: 1;
  padding: var(--space-6);
  overflow-y: auto;
}
```

### 5.2 File Tree

```css
/* Árbol de Archivos */
.file-tree {
  padding: var(--space-4);
}

.file-tree__item {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  padding: var(--space-2) var(--space-3);
  border-radius: var(--radius-md);
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.file-tree__item:hover {
  background-color: var(--color-gray-100);
}

.file-tree__item--active {
  background-color: var(--color-primary-50);
  color: var(--color-primary-700);
}

.file-tree__icon {
  width: 20px;
  height: 20px;
  color: var(--color-primary-500);
}

.file-tree__name {
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
  color: var(--color-gray-700);
}

.file-tree__item--active .file-tree__name {
  color: var(--color-primary-700);
}
```

### 5.3 File Grid

```css
/* Grid de Archivos */
.file-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: var(--space-4);
}

@media (max-width: 640px) {
  .file-grid {
    grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
    gap: var(--space-3);
  }
}

/* Item de Archivo */
.file-item {
  display: flex;
  flex-direction: column;
  padding: var(--space-4);
  background-color: white;
  border: 1px solid var(--color-gray-200);
  border-radius: var(--radius-lg);
  cursor: pointer;
  transition: all 0.2s ease;
}

.file-item:hover {
  border-color: var(--color-primary-300);
  box-shadow: var(--shadow-md);
  transform: translateY(-2px);
}

.file-item__thumbnail {
  width: 100%;
  height: 120px;
  background-color: var(--color-gray-100);
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: var(--space-3);
}

.file-item__icon {
  width: 48px;
  height: 48px;
  color: var(--color-gray-400);
}

.file-item__name {
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
  color: var(--color-gray-900);
  text-align: center;
  word-break: break-word;
}

.file-item__meta {
  font-size: var(--font-size-xs);
  color: var(--color-gray-500);
  text-align: center;
  margin-top: var(--space-1);
}
```

## 6. Estados y Animaciones

### 6.1 Estados de Carga

```css
/* Skeleton Loader */
.skeleton {
  background: linear-gradient(90deg, var(--color-gray-200) 25%, var(--color-gray-100) 50%, var(--color-gray-200) 75%);
  background-size: 200% 100%;
  animation: skeleton-loading 1.5s infinite;
}

@keyframes skeleton-loading {
  0% {
    background-position: 200% 0;
  }
  100% {
    background-position: -200% 0;
  }
}

/* Spinner */
.spinner {
  width: 24px;
  height: 24px;
  border: 2px solid var(--color-gray-200);
  border-top: 2px solid var(--color-primary-600);
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
```

### 6.2 Transiciones

```css
/* Transiciones Globales */
.transition {
  transition: all 0.2s ease-in-out;
}

.transition-fast {
  transition: all 0.1s ease-in-out;
}

.transition-slow {
  transition: all 0.3s ease-in-out;
}

/* Efectos de Hover */
.hover-lift:hover {
  transform: translateY(-2px);
  box-shadow: var(--shadow-lg);
}

.hover-scale:hover {
  transform: scale(1.05);
}
```

## 7. Accesibilidad

### 7.1 Focus States

```css
/* Estados de Foco */
.focus-ring:focus {
  outline: none;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.3);
}

/* Solo mostrar focus ring cuando se navega con teclado */
.js-focus-visible .focus-ring:focus:not(.focus-visible) {
  box-shadow: none;
}
```

### 7.2 Screen Reader Support

```css
/* Texto solo para lectores de pantalla */
.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border: 0;
}
```

## 8. Responsive Breakpoints

```css
/* Breakpoints del Sistema */
:root {
  --breakpoint-sm: 640px;
  --breakpoint-md: 768px;
  --breakpoint-lg: 1024px;
  --breakpoint-xl: 1280px;
  --breakpoint-2xl: 1536px;
}

/* Media Queries Estándar */
@media (min-width: 640px) { /* sm */ }
@media (min-width: 768px) { /* md */ }
@media (min-width: 1024px) { /* lg */ }
@media (min-width: 1280px) { /* xl */ }
@media (min-width: 1536px) { /* 2xl */ }
```
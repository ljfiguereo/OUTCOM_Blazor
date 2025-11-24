## Objetivo
- Mantener 6 imágenes por página sin que se deformen.
- Conservar un encabezado limpio y el nombre del archivo debajo de cada imagen.

## Causa del estirado
- En el script publicado `wwwroot/js/pdf-generator.js` se usa `pdf.addImage(imageData, 'JPEG', x, y, imageWidth, imageHeight - 10)` con ancho/alto fijos, lo que deforma las imágenes.
- Referencia: `wwwroot/js/pdf-generator.js:97-101` (versión publicada). En la versión de desarrollo ya existe lógica para mantener el aspecto (`scaleFactor` y centrado).

## Cambios propuestos
- Sustituir la inserción con tamaño fijo por escalado proporcional “contain” y centrado en la celda.
- Mantener la cuadrícula 2x3 y la reserva de espacio de encabezado actual.
- Asegurar que el nombre se trunque de forma consistente y use tipografía pequeña para no invadir la imagen.

## Detalles técnicos
- Calcular `availableWidth = imageWidth` y `availableHeight = imageHeight - 10` (espacio para texto).
- Obtener dimensiones originales al convertir a base64 y calcular `scaleFactor = min(availableWidth / origWidth, availableHeight / origHeight)`.
- Usar `finalWidth = origWidth * scaleFactor` y `finalHeight = origHeight * scaleFactor`.
- Centrar: `x + (availableWidth - finalWidth)/2`, `y + (availableHeight - finalHeight)/2`.
- Referencia de la implementación correcta (ya presente en desarrollo):
  - Cálculo y centrado: `wwwroot/js/pdf-generator.js:134-148`.
  - Encabezado y layout: `wwwroot/js/pdf-generator.js:35-80`, `24-31`.
- Unificar estas mejoras en la versión que carga el sitio (`Components/App.razor:46` incluye `js/pdf-generator.js`).

## Validación
- Probar con imágenes horizontales y verticales, generando PDF desde `Imprimir PDF` en `FileManager.razor`.
- Confirmar 6 imágenes por página y que ninguna se deforma; revisar márgenes y que los nombres no se superponen.
- Referencia de la invocación: `Components/Pages/FileManager/FileManager.razor:2708-2736`.

## Ajustes opcionales (si hiciera falta)
- Reducir visualmente la “sensación de estirado” ajustando la relación de la celda (p. ej., mantener altura disponible razonable y centrado con fondo claro).
- Mantener el subtítulo/encabezado actual; si se requiere más espacio para imágenes, bajar `startY` o ajustar `imageHeight` computado sin cambiar la grilla 2x3.

## Impacto y riesgo
- Cambios acotados al script de cliente; no afecta servidor.
- Bajo riesgo: solo modifica cómo se dibujan imágenes en jsPDF.

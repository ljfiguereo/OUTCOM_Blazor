## Objetivo
- Mejorar cómo se ven las imágenes en el PDF manteniendo 6 por página.
- Evitar cualquier sensación de distorsión y lograr una presentación más limpia y uniforme.

## Opciones de mejora
1. Ajuste proporcional avanzado
- Añadir modos de ajuste por celda: `contain` (actual) y `cover` (relleno con recorte centrado) para evitar grandes bordes en imágenes muy panorámicas.
- Celda con padding configurable (p. ej. 2–3 mm) y fondo blanco para dar respiro visual.

2. Marco y estética
- Dibujar un rectángulo con esquinas redondeadas y borde tenue alrededor de cada imagen.
- Sombra ligera opcional para separar visualmente elementos.

3. Calidad de imagen
- Incrementar calidad de JPEG a `0.92` y conservar PNG como PNG.
- Activar `imageSmoothingQuality = 'high'` en el canvas de re-muestreo.

4. Corrección de orientación EXIF
- Leer orientación EXIF y rotar la imagen en canvas antes de insertarla.
- Implementar con `fetch(url) + ArrayBuffer` y una librería ligera (p. ej. `exif-reader`), solo si el servidor permite CORS en `/api/files/preview`.

5. Texto/caption bajo la imagen
- Soporte de dos líneas con corte inteligente por palabras y elipsis.
- Tipografía consistente y centrado, manteniendo espacio reservado (10 mm).

6. Encabezado y pie
- Reducir espacio del encabezado si es necesario (p. ej. `startY` de 60 → 55) para maximizar área de imagen sin perder identidad.
- Pie de página con numeración “Página X de Y”.

## Cambios técnicos
- `OutCom/wwwroot/js/pdf-generator.js`:
  - Añadir parámetro `fitMode = 'contain' | 'cover'` y `cellPadding`.
  - En el cálculo actual de tamaño y centrado: `...:136-148`, incorporar `cover` cuando esté activo.
  - Dibujar el fondo/marco: rectángulo antes de `addImage` con `rounded` y `setDrawColor`.
  - Calidad y formatos: aumentar factor JPEG y `imageSmoothingQuality` dentro de `loadImageAsBase64`: `...:88-101`.
  - EXIF (opcional): nueva función `getExifOrientation(url)` y rotación en canvas.
  - Numeración de páginas: después de `addHeader()` y al final de cada página, añadir pie con `pdf.text`.
- Invocación sigue igual (Blazor JSInterop): `Components/Pages/FileManager/FileManager.razor:2708-2736`.

## Validación
- Probar carpeta con mix de imágenes horizontales/verticales.
- Validar que:
  - Se mantienen 6 por página.
  - No hay distorsión; bordes y padding uniformes.
  - “cover” mejora casos extremos con panorámicas (sin parecer estiradas).
  - Textos no se solapan y el encabezado/pie se ven limpios.

## Impacto y riesgo
- Cambios aislados al cliente (jsPDF); sin efecto en servidor.
- Riesgo bajo; EXIF depende de CORS: si no es posible, se deja como opcional.

## Entregables
- Script actualizado con configuración visual (fitMode, padding, marco, calidad).
- Comportamiento validado desde el botón “Imprimir PDF”.

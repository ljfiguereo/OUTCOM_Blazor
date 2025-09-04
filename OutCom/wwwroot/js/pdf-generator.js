// Función para generar PDF con imágenes usando jsPDF
window.generateImagesPDF = async function(images, folderName) {
    try {
        // Importar jsPDF
        const { jsPDF } = window.jspdf;
        
        if (!jsPDF) {
            throw new Error('jsPDF no está disponible');
        }

        // Crear nuevo documento PDF
        const pdf = new jsPDF({
            orientation: 'portrait',
            unit: 'mm',
            format: 'a4'
        });

        const pageWidth = pdf.internal.pageSize.getWidth();
        const pageHeight = pdf.internal.pageSize.getHeight();
        const margin = 20;
        const contentWidth = pageWidth - (margin * 2);
        const contentHeight = pageHeight - (margin * 2);

        // Configuración para 6 imágenes por página (2x3)
        const imagesPerPage = 6;
        const cols = 2;
        const rows = 3;
        const imageWidth = (contentWidth - 10) / cols; // 10mm de espacio entre columnas
        const imageHeight = (contentHeight - 80) / rows; // 80mm para encabezados y espacios
        const startY = 60; // Espacio para encabezados

        let currentPage = 1;
        let imageIndex = 0;

        // Función para agregar encabezado a cada página
        function addHeader() {
            // Logo principal OUTCOM
            try {
                // Agregar logo centrado
                const logoWidth = 80; // Ancho del logo en mm
                const logoHeight = 20; // Alto del logo en mm
                const logoX = (pageWidth - logoWidth) / 2; // Centrar horizontalmente
                const logoY = 15; // Posición vertical
                
                pdf.addImage('/LogoOutcom.png', 'PNG', logoX, logoY, logoWidth, logoHeight);
            } catch (error) {
                console.error('Error al cargar logo:', error);
                // Fallback al texto si no se puede cargar la imagen
                pdf.setFontSize(24);
                pdf.setFont('helvetica', 'bold');
                pdf.text('OUTCOM', pageWidth / 2, 30, { align: 'center' });
            }

            // Subtítulo "Reporte de [nombre carpeta]" con fondo gris
            const subtitleText = `REPORTE DE INSTALACION AUTOBUSES OMSA`;
            pdf.setFontSize(20);
            pdf.setFont('helvetica', 'normal');
            
            // Calcular dimensiones del texto para el fondo
            const textWidth = pdf.getTextWidth(subtitleText);
            const textHeight = 8; // Altura aproximada del texto en mm
            const padding = 4; // Padding alrededor del texto
            
            // Dibujar rectángulo negro como fondo
            pdf.setFillColor(0, 0, 0); // Color negro
            const rectX = (pageWidth - textWidth - padding * 2) / 2;
            const rectY = 38;
            pdf.rect(rectX, rectY, textWidth + padding * 2, textHeight + padding, 'F');
            
            // Cambiar color del texto a blanco y agregar el texto
            pdf.setTextColor(255, 255, 255); // Color blanco
            pdf.text(subtitleText, pageWidth / 2, 46, { align: 'center' });
            
            // Restaurar color del texto a negro para el resto del documento
            pdf.setTextColor(0, 0, 0);

            //// Línea separadora
            //pdf.setLineWidth(0.5);
            //pdf.line(margin, 47, pageWidth - margin, 47);
        }

        // Función para cargar imagen como base64
        async function loadImageAsBase64(url) {
            return new Promise((resolve, reject) => {
                const img = new Image();
                img.crossOrigin = 'anonymous';
                img.onload = function() {
                    const canvas = document.createElement('canvas');
                    const ctx = canvas.getContext('2d');
                    canvas.width = img.width;
                    canvas.height = img.height;
                    ctx.drawImage(img, 0, 0);
                    const dataURL = canvas.toDataURL('image/jpeg', 0.8);
                    resolve(dataURL);
                };
                img.onerror = function() {
                    reject(new Error(`No se pudo cargar la imagen: ${url}`));
                };
                img.src = url;
            });
        }

        // Agregar encabezado a la primera página
        addHeader();

        // Procesar cada imagen
        for (let i = 0; i < images.length; i++) {
            const image = images[i];
            
            // Si necesitamos una nueva página
            if (i > 0 && i % imagesPerPage === 0) {
                pdf.addPage();
                currentPage++;
                addHeader();
            }

            // Calcular posición en la grilla
            const positionInPage = i % imagesPerPage;
            const col = positionInPage % cols;
            const row = Math.floor(positionInPage / cols);

            const x = margin + (col * (imageWidth + 5));
            const y = startY + (row * (imageHeight + 15));

            try {
                // Cargar imagen
                const imageData = await loadImageAsBase64(image.url);
                
                // Agregar imagen al PDF
                pdf.addImage(imageData, 'JPEG', x, y, imageWidth, imageHeight - 10);
                
                // Agregar nombre del archivo debajo de la imagen
                pdf.setFontSize(8);
                pdf.setFont('helvetica', 'normal');
                const textY = y + imageHeight - 5;
                
                // Truncar nombre si es muy largo
                let fileName = image.name;
                if (fileName.length > 25) {
                    fileName = fileName.substring(0, 22) + '...';
                }
                
                pdf.text(fileName, x + (imageWidth / 2), textY, { align: 'center' });
                
            } catch (error) {
                console.error(`Error al procesar imagen ${image.name}:`, error);
                
                // Mostrar placeholder para imagen que no se pudo cargar
                pdf.setFillColor(240, 240, 240);
                pdf.rect(x, y, imageWidth, imageHeight - 10, 'F');
                
                pdf.setFontSize(10);
                pdf.setTextColor(100, 100, 100);
                pdf.text('Imagen no disponible', x + (imageWidth / 2), y + (imageHeight / 2), { align: 'center' });
                
                // Nombre del archivo
                pdf.setFontSize(8);
                pdf.setTextColor(0, 0, 0);
                const textY = y + imageHeight - 5;
                let fileName = image.name;
                if (fileName.length > 25) {
                    fileName = fileName.substring(0, 22) + '...';
                }
                pdf.text(fileName, x + (imageWidth / 2), textY, { align: 'center' });
            }
        }

        // Generar nombre del archivo PDF
        const timestamp = new Date().toISOString().slice(0, 19).replace(/:/g, '-');
        const fileName = `Reporte_${folderName}_${timestamp}.pdf`;

        // Descargar el PDF
        pdf.save(fileName);
        
        console.log(`PDF generado exitosamente: ${fileName}`);
        
    } catch (error) {
        console.error('Error al generar PDF:', error);
        alert('Error al generar el PDF: ' + error.message);
    }
};
// Función para generar PDF con imágenes usando jsPDF
window.generateImagesPDF = async function (images, folderName) {
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
        const fitMode = 'contain';
        const cellPadding = 2;
        const showCellBorder = true;
        const captionReserved = 10;

        // Configuración para 4 imágenes por página (2x2)
        const imagesPerPage = 4;
        const cols = 2;
        const rows = 2;
        const imageWidth = (contentWidth - 10) / cols;
        const imageHeight = (contentHeight - 80) / rows;
        const startY = 60;

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
                img.onload = function () {
                    const canvas = document.createElement('canvas');
                    const ctx = canvas.getContext('2d');
                    ctx.imageSmoothingEnabled = true;
                    ctx.imageSmoothingQuality = 'high';
                    canvas.width = img.width;
                    canvas.height = img.height;
                    ctx.drawImage(img, 0, 0);
                    const isPng = /\.png($|\?)/i.test(url);
                    const mime = isPng ? 'image/png' : 'image/jpeg';
                    const dataURL = canvas.toDataURL(mime, 0.92);
                    resolve({
                        data: dataURL,
                        width: img.width,
                        height: img.height,
                        format: isPng ? 'PNG' : 'JPEG'
                    });
                };
                img.onerror = function () {
                    reject(new Error(`No se pudo cargar la imagen: ${url}`));
                };
                img.src = url;
            });
        }

        function addFooter() {
            pdf.setFontSize(9);
            pdf.setFont('helvetica', 'normal');
            const totalPages = Math.ceil(images.length / imagesPerPage);
            pdf.text(`Página ${currentPage} de ${totalPages}`, pageWidth / 2, pageHeight - 10, { align: 'center' });
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
                const imageObj = await loadImageAsBase64(image.url);

                // Calcular dimensiones para mantener aspect ratio
                const availableWidth = imageWidth;
                const availableHeight = imageHeight - captionReserved;
                const innerWidth = availableWidth - cellPadding * 2;
                const innerHeight = availableHeight - cellPadding * 2;

                const scaleContain = Math.min(innerWidth / imageObj.width, innerHeight / imageObj.height);
                const scaleCover = Math.max(innerWidth / imageObj.width, innerHeight / imageObj.height);
                const scaleFactor = fitMode === 'cover' ? scaleCover : scaleContain;

                const finalWidth = imageObj.width * scaleFactor;
                const finalHeight = imageObj.height * scaleFactor;

                // Centrar imagen en su celda
                const xOffset = cellPadding + (innerWidth - finalWidth) / 2;
                const yOffset = cellPadding + (innerHeight - finalHeight) / 2;

                // Agregar imagen al PDF
                if (showCellBorder) {
                    pdf.setDrawColor(220, 220, 220);
                    pdf.rect(x, y, availableWidth, availableHeight, 'S');
                }
                pdf.addImage(imageObj.data, imageObj.format, x + xOffset, y + yOffset, finalWidth, finalHeight);

                // Agregar nombre del archivo debajo de la imagen
                pdf.setFontSize(8);
                pdf.setFont('helvetica', 'normal');
                const textY = y + imageHeight - 5;

                // Truncar nombre si es muy largo
                let fileName = image.name;
                const lines = pdf.splitTextToSize(fileName, imageWidth - 4);
                const twoLines = lines.slice(0, 2);
                if (lines.length > 2) {
                    const last = twoLines[1];
                    twoLines[1] = last.length > 3 ? last.substring(0, last.length - 3) + '...' : '...';
                }
                pdf.text(twoLines, x + (imageWidth / 2), textY, { align: 'center' });

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
                const lines = pdf.splitTextToSize(fileName, imageWidth - 4);
                const twoLines = lines.slice(0, 2);
                if (lines.length > 2) {
                    const last = twoLines[1];
                    twoLines[1] = last.length > 3 ? last.substring(0, last.length - 3) + '...' : '...';
                }
                pdf.text(twoLines, x + (imageWidth / 2), textY, { align: 'center' });
            }
        }

        addFooter();
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
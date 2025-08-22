
window.hideSashLoader = () => {
    // Usamos jQuery para ser consistentes con el tema Sash
    // El "fadeOut" le da una transición suave.
    if ($('#global-loader').length) {
        $('#global-loader').fadeOut('slow');
    }
};

window.addEventListener('DOMContentLoaded', () => {
    const burgerBtn = document.querySelector('.navbar-burger');
    const fsOffCanvas = document.querySelector('fs-off-canvas');
    burgerBtn.addEventListener('click', () => {
        fsOffCanvas.toggleAttribute('is-open');
    });

    fsOffCanvas.addEventListener('fs-close-off-canvas', () => {
        fsOffCanvas.removeAttribute('is-open');
    });
});
import { registerAll } from 'https://cdn.skypack.dev/pin/fsharp-components@v0.0.4-vrcdq1fAiAHCYER7N99I/mode=imports,min/optimized/fsharp-components.js';
registerAll();

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
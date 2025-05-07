function triggerFlash() {
    const flash = document.createElement('div');
    flash.className = 'flash-spot';

    const x = Math.random() * window.innerWidth;
    const y = Math.random() * window.innerHeight;

    flash.style.left = `${x}px`;
    flash.style.top = `${y}px`;

    document.body.appendChild(flash);

    setTimeout(() => {
        flash.remove();
    }, 400);
}

// Pornește bliturile la fiecare 2 secunde
setInterval(triggerFlash, 2000);

document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll('.btn').forEach(button => {
        button.addEventListener('mouseenter', () => {
            for (let i = 0; i < 5; i++) {
                const svgElement = document.createElement('span');
                svgElement.classList.add('heart-floating');

                // Adăugăm SVG-ul extern din fișierul tău
                svgElement.innerHTML = `<img src="/Images/pantof.svg" width="24" height="24" />`;

                const rect = button.getBoundingClientRect();
                const x = Math.random() * rect.width;
                const y = Math.random() * rect.height;

                svgElement.style.position = 'fixed';
                svgElement.style.left = `${rect.left + x}px`;
                svgElement.style.top = `${rect.top + y}px`;

                document.body.appendChild(svgElement);

                setTimeout(() => {
                    svgElement.remove();
                }, 1000);
            }
        });
    });
});


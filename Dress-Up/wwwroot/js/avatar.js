// Flash-uri sclipici
function triggerFlash() {
    const f = document.createElement('div');
    f.className = 'flash-spot';
    f.style.left = `${Math.random() * window.innerWidth}px`;
    f.style.top = `${Math.random() * window.innerHeight}px`;
    document.body.appendChild(f);
    setTimeout(() => f.remove(), 1000);
}
setInterval(triggerFlash, 2500);

// Animație la hover pe butoane
document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll('.btn-avatar').forEach(btn => {
        btn.addEventListener('mouseenter', () => {
            for (let i = 0; i < 4; i++) {
                const sp = document.createElement('span');
                sp.className = 'heart-floating';
                sp.innerHTML = `<img src="/Images/pantof.svg" width="24" height="24">`;
                const r = btn.getBoundingClientRect();
                sp.style.left = `${r.left + Math.random() * r.width}px`;
                sp.style.top = `${r.top + Math.random() * r.height}px`;
                document.body.appendChild(sp);
                setTimeout(() => sp.remove(), 800);
            }
        });
    });
});

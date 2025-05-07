let currentPage = 0;
const pages = document.querySelectorAll(".page");

function nextPage() {
    if (currentPage < pages.length - 1) {
        pages[currentPage].classList.remove("active");
        pages[currentPage].classList.add("previous");
        currentPage++;
        pages[currentPage].classList.add("active");
    }
}

function previousPage() {
    if (currentPage > 0) {
        pages[currentPage].classList.remove("active");
        pages[currentPage].classList.remove("next"); // dacă ai folosit "next"
        currentPage--;
        pages[currentPage].classList.remove("previous"); // eliminăm și clasa previous
        pages[currentPage].classList.add("active");
    }
}

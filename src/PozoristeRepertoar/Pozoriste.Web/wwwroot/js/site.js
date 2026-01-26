// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function updateSeatSummary() {
    const summary = document.querySelector("[data-seat-summary]");
    if (!summary) return;

    const priceRaw = summary.getAttribute("data-price") || "0";
    const price = parseFloat(priceRaw);

    const seats = Array.from(document.querySelectorAll(".seat-checkbox:checked"));
    const count = seats.length;
    const total = Number.isFinite(price) ? price * count : 0;

    const countEl = summary.querySelector("[data-seat-count]");
    if (countEl) countEl.textContent = count.toString();

    const totalEl = summary.querySelector("[data-seat-total]");
    if (totalEl) totalEl.textContent = `${total.toFixed(2)} RSD`;

    const listEl = summary.querySelector("[data-seat-list]");
    if (!listEl) return;

    listEl.innerHTML = "";
    if (count === 0) {
        listEl.textContent = "Nema izabranih sedista.";
        return;
    }

    seats.forEach((seat) => {
        const label = seat.getAttribute("data-seat-label") || seat.value;
        const pill = document.createElement("span");
        pill.className = "seat-pill";
        pill.textContent = label;
        listEl.appendChild(pill);
    });
}

document.addEventListener("DOMContentLoaded", () => {
    updateSeatSummary();

    document.addEventListener("change", (event) => {
        const target = event.target;
        if (target && target.classList && target.classList.contains("seat-checkbox")) {
            updateSeatSummary();
        }
    });
});

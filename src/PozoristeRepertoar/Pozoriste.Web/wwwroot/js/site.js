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
    let total = 0;
    const zoneCounts = new Map();
    const zoneCountEls = Array.from(summary.querySelectorAll("[data-zone-count]"));
    zoneCountEls.forEach((el) => { el.textContent = "0"; });

    const countEl = summary.querySelector("[data-seat-count]");
    if (countEl) countEl.textContent = count.toString();

    seats.forEach((seat) => {
        const multiplierRaw = seat.getAttribute("data-multiplier") || "1";
        const multiplier = parseFloat(multiplierRaw);
        const zone = seat.getAttribute("data-zone") || "Standard";

        const seatPrice = Number.isFinite(price) && Number.isFinite(multiplier)
            ? price * multiplier
            : 0;

        total += seatPrice;
        zoneCounts.set(zone, (zoneCounts.get(zone) || 0) + 1);
    });

    const totalEl = summary.querySelector("[data-seat-total]");
    if (totalEl) totalEl.textContent = `${total.toFixed(2)} RSD`;

    zoneCounts.forEach((value, key) => {
        const zoneEl = summary.querySelector(`[data-zone-count="${key}"]`);
        if (zoneEl) zoneEl.textContent = value.toString();
    });

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

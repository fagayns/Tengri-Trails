(() => {
    const menuButton = document.querySelector(".menu-toggle");
    const navigation = document.querySelector(".main-nav");

    if (menuButton && navigation) {
        menuButton.addEventListener("click", () => {
            const isOpen = menuButton.getAttribute("aria-expanded") === "true";
            menuButton.setAttribute("aria-expanded", String(!isOpen));
            navigation.classList.toggle("is-open", !isOpen);
        });
        navigation.addEventListener("keydown", (event) => {
            if (event.key !== "Escape" || menuButton.getAttribute("aria-expanded") !== "true") return;
            menuButton.setAttribute("aria-expanded", "false");
            navigation.classList.remove("is-open");
            menuButton.focus();
        });
    }

    document.querySelectorAll(".language-link").forEach((link) => {
        link.addEventListener("click", () => {
            const language = link.getAttribute("hreflang");
            if (language) {
                document.cookie = `tengri-lang=${language}; path=/; max-age=31536000; SameSite=Lax`;
            }
        });
    });

    const hero = document.querySelector(".hero");
    if (hero && !window.matchMedia("(prefers-reduced-motion: reduce), (max-width: 760px)").matches) {
        const layers = [
            [".mountain-far", 0.035],
            [".mountain-mid", 0.065],
            [".mountain-front", 0.095]
        ];
        let ticking = false;
        const updateParallax = () => {
            const offset = Math.min(window.scrollY, hero.offsetHeight);
            layers.forEach(([selector, speed]) => {
                const layer = hero.querySelector(selector);
                if (layer) layer.style.transform = `translate3d(0, ${offset * speed}px, 0)`;
            });
            ticking = false;
        };
        window.addEventListener("scroll", () => {
            if (!ticking) {
                window.requestAnimationFrame(updateParallax);
                ticking = true;
            }
        }, { passive: true });
    }

    const galleryThumbnails = [...document.querySelectorAll("[data-gallery-thumbnail]")];
    const galleryDialog = document.querySelector("[data-gallery-dialog]");
    const galleryDialogImage = document.querySelector("[data-gallery-dialog-image]");
    const galleryCounter = document.querySelector("[data-gallery-counter]");

    if (galleryThumbnails.length && galleryDialog && galleryDialogImage && galleryCounter) {
        let currentIndex = 0;
        const selectPhoto = (index) => {
            currentIndex = (index + galleryThumbnails.length) % galleryThumbnails.length;
            const selected = galleryThumbnails[currentIndex];
            const currentImage = document.querySelector("[data-gallery-current]");
            currentImage.src = selected.dataset.src;
            galleryDialogImage.src = selected.dataset.src;
            galleryThumbnails.forEach((thumbnail, thumbnailIndex) => {
                thumbnail.classList.toggle("is-active", thumbnailIndex === currentIndex);
                if (thumbnailIndex === currentIndex) thumbnail.setAttribute("aria-current", "true");
                else thumbnail.removeAttribute("aria-current");
            });
            galleryCounter.textContent = `${currentIndex + 1} / ${galleryThumbnails.length}`;
        };

        galleryThumbnails.forEach((thumbnail) => {
            thumbnail.addEventListener("click", () => selectPhoto(Number(thumbnail.dataset.index)));
        });
        document.querySelector("[data-gallery-open]")?.addEventListener("click", () => {
            selectPhoto(currentIndex);
            galleryDialog.showModal();
        });
        document.querySelector("[data-gallery-close]")?.addEventListener("click", () => galleryDialog.close());
        document.querySelector("[data-gallery-previous]")?.addEventListener("click", () => selectPhoto(currentIndex - 1));
        document.querySelector("[data-gallery-next]")?.addEventListener("click", () => selectPhoto(currentIndex + 1));
        document.addEventListener("keydown", (event) => {
            if (!galleryDialog.open) return;
            if (event.key === "Escape") {
                event.preventDefault();
                galleryDialog.close();
            } else if (event.key === "ArrowLeft") {
                event.preventDefault();
                selectPhoto(currentIndex - 1);
            } else if (event.key === "ArrowRight") {
                event.preventDefault();
                selectPhoto(currentIndex + 1);
            }
        });

        let touchStartX = 0;
        galleryDialog.addEventListener("touchstart", (event) => {
            touchStartX = event.changedTouches[0].screenX;
        }, { passive: true });
        galleryDialog.addEventListener("touchend", (event) => {
            const swipeDistance = event.changedTouches[0].screenX - touchStartX;
            if (Math.abs(swipeDistance) > 45) selectPhoto(currentIndex + (swipeDistance < 0 ? 1 : -1));
        }, { passive: true });
    }

    const bookingForm = document.querySelector("[data-booking-form]");
    if (bookingForm) {
        const guestInputs = [...bookingForm.querySelectorAll("[data-guest-count]")];
        const total = bookingForm.querySelector("[data-booking-total]");
        const guestSummary = bookingForm.querySelector("[data-summary-guests]");
        const departureSelect = bookingForm.querySelector('[name="DepartureId"]');
        const dateSummary = bookingForm.querySelector("[data-summary-date]");
        const unitPrice = Number(bookingForm.dataset.unitPrice);
        const currency = total.dataset.currency;
        const locale = bookingForm.dataset.locale;
        const refreshSummary = () => {
            const guestCount = guestInputs.reduce((sum, input) => sum + Math.max(0, Number(input.value) || 0), 0);
            guestSummary.textContent = String(guestCount);
            total.textContent = `${(unitPrice * guestCount).toLocaleString(locale)} ${currency}`;
            const selectedOption = departureSelect.selectedOptions[0];
            dateSummary.textContent = selectedOption?.dataset.summaryDate || selectedOption?.textContent || "";
        };
        guestInputs.forEach((input) => input.addEventListener("input", refreshSummary));
        departureSelect?.addEventListener("change", refreshSummary);
        refreshSummary();
    }
})();

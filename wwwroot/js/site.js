(() => {
    const menuButton = document.querySelector(".menu-toggle");
    const navigation = document.querySelector(".main-nav");

    if (menuButton && navigation) {
        menuButton.addEventListener("click", () => {
            const isOpen = menuButton.getAttribute("aria-expanded") === "true";
            menuButton.setAttribute("aria-expanded", String(!isOpen));
            navigation.classList.toggle("is-open", !isOpen);
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
    if (hero && !window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
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
})();

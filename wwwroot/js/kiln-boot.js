(() => {
  const DELAY = 520;
  const root = document.documentElement;
  const boot = document.getElementById("kiln-boot");
  const nav = document.getElementById("kiln-nav");
  let shown = false;
  let navTimer = 0;

  const timer = window.setTimeout(() => {
    if (root.dataset.forno === "ready") {
      return;
    }
    shown = true;
    root.classList.add("kiln-booting");
    if (boot) {
      boot.setAttribute("aria-hidden", "false");
      boot.setAttribute("aria-busy", "true");
    }
  }, DELAY);

  function hideBoot() {
    window.clearTimeout(timer);
    root.dataset.forno = "ready";
    root.classList.remove("kiln-booting");

    if (!boot) {
      return;
    }

    boot.setAttribute("aria-busy", "false");
    boot.setAttribute("aria-hidden", "true");

    if (!shown) {
      boot.remove();
      return;
    }

    boot.classList.add("is-out");
    window.setTimeout(() => boot.remove(), 700);
  }

  function navOn() {
    window.clearTimeout(navTimer);
    navTimer = window.setTimeout(() => {
      root.classList.add("kiln-naving");
      nav?.setAttribute("aria-hidden", "false");
    }, 420);
  }

  function navOff() {
    window.clearTimeout(navTimer);
    root.classList.remove("kiln-naving");
    nav?.setAttribute("aria-hidden", "true");
  }

  function listenNav(blazor) {
    if (!blazor || typeof blazor.addEventListener !== "function") {
      return;
    }

    try {
      blazor.addEventListener("enhancednavigationstart", navOn);
      blazor.addEventListener("enhancednavigationend", navOff);
      blazor.addEventListener("enhancedload", navOff);
    } catch {
      /* older runtime */
    }
  }

  async function start() {
    const blazor = window.Blazor;
    if (!blazor || typeof blazor.start !== "function") {
      hideBoot();
      return;
    }

    try {
      await blazor.start();
    } catch {
      /* prerendered page remains usable */
    }

    hideBoot();
    listenNav(window.Blazor);
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", start, { once: true });
  } else {
    start();
  }
})();

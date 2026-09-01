(() => {
  const MIN_SHOW = 720;
  const NAV_DELAY = 180;
  const root = document.documentElement;
  const boot = document.getElementById("kiln-boot");
  const nav = document.getElementById("kiln-nav");
  const navStarted =
    typeof performance !== "undefined" && performance.timeOrigin
      ? performance.timeOrigin
      : Date.now();
  let shown =
    root.classList.contains("kiln-booting") || root.dataset.forno !== "ready";
  let shownAt = shown ? navStarted : 0;
  let navTimer = 0;
  let hiding = false;

  function showBoot() {
    if (root.dataset.forno === "ready" || !boot) {
      return;
    }

    shown = true;
    if (!shownAt) {
      shownAt = Date.now();
    }

    root.classList.add("kiln-booting");
    boot.classList.remove("is-out");
    boot.setAttribute("aria-hidden", "false");
    boot.setAttribute("aria-busy", "true");
  }

  function syncBootAria() {
    if (!boot || !shown) {
      return;
    }

    boot.setAttribute("aria-hidden", "false");
    boot.setAttribute("aria-busy", "true");
  }

  if (boot) {
    showBoot();
    syncBootAria();
  } else {
    document.addEventListener(
      "DOMContentLoaded",
      () => {
        showBoot();
        syncBootAria();
      },
      { once: true }
    );
  }

  async function hideBoot() {
    if (hiding) {
      return;
    }

    hiding = true;

    if (shown) {
      const wait = Math.max(0, MIN_SHOW - (Date.now() - shownAt));
      if (wait) {
        await new Promise((resolve) => window.setTimeout(resolve, wait));
      }
    }

    root.dataset.forno = "ready";
    root.classList.remove("kiln-booting");

    if (!boot) {
      return;
    }

    boot.setAttribute("aria-busy", "false");
    boot.setAttribute("aria-hidden", "true");

    if (!shown) {
      boot.classList.add("is-out");
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
    }, NAV_DELAY);
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
      await hideBoot();
      return;
    }

    try {
      await blazor.start();
    } catch {
      /* prerendered page remains usable */
    }

    await hideBoot();
    listenNav(window.Blazor);
  }

  window.addEventListener(
    "pageshow",
    (event) => {
      if (!event.persisted || root.dataset.forno !== "ready") {
        return;
      }

      delete root.dataset.forno;
      shown = true;
      shownAt = Date.now();
      hiding = false;
      showBoot();
    },
    { passive: true }
  );

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", start, { once: true });
  } else {
    start();
  }
})();

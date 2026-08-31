(() => {
  const KEY = "forno-theme";

  function preferred() {
    return window.matchMedia("(prefers-color-scheme: dark)").matches
      ? "dark"
      : "light";
  }

  function stored() {
    const value = localStorage.getItem(KEY);
    return value === "dark" || value === "light" ? value : null;
  }

  function apply(theme, persist) {
    const next = theme === "dark" ? "dark" : "light";
    document.documentElement.setAttribute("data-theme", next);
    document.documentElement.style.colorScheme = next;

    const meta = document.querySelector('meta[name="theme-color"]');
    if (meta) {
      meta.setAttribute("content", next === "dark" ? "#16110e" : "#f3ead8");
    }

    const button = document.getElementById("theme-toggle");
    if (button) {
      const dark = next === "dark";
      button.setAttribute("aria-pressed", String(dark));
      button.setAttribute(
        "aria-label",
        dark ? "Zapnúť svetlý režim" : "Zapnúť tmavý režim"
      );
    }

    if (persist) {
      localStorage.setItem(KEY, next);
    }
  }

  function current() {
    return document.documentElement.getAttribute("data-theme") === "dark"
      ? "dark"
      : "light";
  }

  window.FornoTheme = {
    get: current,
    set(theme) {
      apply(theme, true);
      return current();
    },
    toggle() {
      return window.FornoTheme.set(current() === "dark" ? "light" : "dark");
    },
  };

  apply(stored() || preferred(), false);

  window.requestAnimationFrame(() => {
    document.documentElement.classList.add("theme-ready");
  });

  document.addEventListener("click", (event) => {
    if (event.target.closest(".theme-toggle")) {
      event.preventDefault();
      window.FornoTheme.toggle();
    }
  });

  const keepButtonInSync = () => apply(current(), false);
  const watch = new MutationObserver(keepButtonInSync);
  const startWatch = () => {
    const mast = document.querySelector(".mast");
    if (mast) {
      watch.observe(mast, { childList: true, subtree: true });
      keepButtonInSync();
    }
  };

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", startWatch);
  } else {
    startWatch();
  }

  window.matchMedia("(prefers-color-scheme: dark)").addEventListener("change", (event) => {
    if (!stored()) {
      apply(event.matches ? "dark" : "light", false);
    }
  });

  window.addEventListener("storage", (event) => {
    if (event.key === KEY && (event.newValue === "dark" || event.newValue === "light")) {
      apply(event.newValue, false);
    }
  });
})();

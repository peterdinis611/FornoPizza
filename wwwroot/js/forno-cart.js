(() => {
  const CART = "forno-cart";
  const LEAF = "forno-leaf";

  function read(key, fallback) {
    try {
      const raw = localStorage.getItem(key);
      return raw == null ? fallback : raw;
    } catch {
      return fallback;
    }
  }

  function write(key, value) {
    try {
      localStorage.setItem(key, value);
    } catch {
      /* private mode */
    }
  }

  window.FornoCart = {
    load() {
      return read(CART, "[]");
    },
    save(json) {
      write(CART, json);
    },
    leafGet(slug) {
      try {
        const all = JSON.parse(read(LEAF, "{}"));
        const row = all[slug];
        if (Array.isArray(row)) {
          return row;
        }
        if (row && Array.isArray(row.extras)) {
          return row.extras;
        }
        return [];
      } catch {
        return [];
      }
    },
    leafQty(slug) {
      try {
        const all = JSON.parse(read(LEAF, "{}"));
        const row = all[slug];
        if (row && typeof row.qty === "number") {
          return Math.min(12, Math.max(1, row.qty | 0));
        }
        return 1;
      } catch {
        return 1;
      }
    },
    leafSet(slug, ids, qty) {
      try {
        const all = JSON.parse(read(LEAF, "{}"));
        all[slug] = {
          extras: Array.isArray(ids) ? ids : [],
          qty: typeof qty === "number" ? Math.min(12, Math.max(1, qty | 0)) : 1,
        };
        write(LEAF, JSON.stringify(all));
      } catch {
        /* private mode */
      }
    },
  };
})();

(() => {
  const DB_NAME = "forno";
  const DB_VERSION = 1;
  const STORE = "kv";
  const CART = "cart";
  const LEAF = "leaf";
  const LEGACY_CART = "forno-cart";
  const LEGACY_LEAF = "forno-leaf";
  const MIGRATED = "forno-idb-migrated";

  let dbPromise = null;

  function openDb() {
    if (dbPromise) {
      return dbPromise;
    }

    dbPromise = new Promise((resolve, reject) => {
      const req = indexedDB.open(DB_NAME, DB_VERSION);
      req.onupgradeneeded = () => {
        const db = req.result;
        if (!db.objectStoreNames.contains(STORE)) {
          db.createObjectStore(STORE);
        }
      };
      req.onsuccess = () => resolve(req.result);
      req.onerror = () => reject(req.error || new Error("IndexedDB open failed"));
    });

    return dbPromise;
  }

  function idbGet(key) {
    return openDb().then(
      (db) =>
        new Promise((resolve, reject) => {
          const tx = db.transaction(STORE, "readonly");
          const req = tx.objectStore(STORE).get(key);
          req.onsuccess = () => resolve(req.result);
          req.onerror = () => reject(req.error);
        })
    );
  }

  function idbSet(key, value) {
    return openDb().then(
      (db) =>
        new Promise((resolve, reject) => {
          const tx = db.transaction(STORE, "readwrite");
          tx.objectStore(STORE).put(value, key);
          tx.oncomplete = () => resolve();
          tx.onerror = () => reject(tx.error);
          tx.onabort = () => reject(tx.error);
        })
    );
  }

  function legacyRead(key, fallback) {
    try {
      const raw = localStorage.getItem(key);
      return raw == null ? fallback : raw;
    } catch {
      return fallback;
    }
  }

  function legacyClear(key) {
    try {
      localStorage.removeItem(key);
    } catch {
      /* private mode */
    }
  }

  let migratePromise = null;

  function migrateOnce() {
    if (migratePromise) {
      return migratePromise;
    }

    migratePromise = (async () => {
      try {
        if (legacyRead(MIGRATED, "") === "1") {
          return;
        }

        const cart = legacyRead(LEGACY_CART, null);
        const leaf = legacyRead(LEGACY_LEAF, null);
        const existingCart = await idbGet(CART);
        const existingLeaf = await idbGet(LEAF);

        if (cart != null && (existingCart == null || existingCart === "")) {
          await idbSet(CART, cart);
        }
        if (leaf != null && (existingLeaf == null || existingLeaf === "")) {
          await idbSet(LEAF, leaf);
        }

        legacyClear(LEGACY_CART);
        legacyClear(LEGACY_LEAF);
        try {
          localStorage.setItem(MIGRATED, "1");
        } catch {
          /* ignore */
        }
      } catch {
        /* IndexedDB unavailable — leave legacy alone */
      }
    })();

    return migratePromise;
  }

  async function readCart() {
    await migrateOnce();
    const value = await idbGet(CART);
    return typeof value === "string" ? value : "[]";
  }

  async function writeCart(json) {
    await migrateOnce();
    await idbSet(CART, typeof json === "string" ? json : "[]");
  }

  async function readLeafAll() {
    await migrateOnce();
    try {
      const raw = await idbGet(LEAF);
      if (typeof raw !== "string" || !raw) {
        return {};
      }
      const parsed = JSON.parse(raw);
      return parsed && typeof parsed === "object" ? parsed : {};
    } catch {
      return {};
    }
  }

  async function writeLeafAll(all) {
    await migrateOnce();
    await idbSet(LEAF, JSON.stringify(all ?? {}));
  }

  window.FornoCart = {
    async load() {
      try {
        return await readCart();
      } catch {
        return "[]";
      }
    },
    async save(json) {
      try {
        await writeCart(json);
      } catch {
        /* private mode / unavailable */
      }
    },
    async leafGet(slug) {
      try {
        const all = await readLeafAll();
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
    async leafQty(slug) {
      try {
        const all = await readLeafAll();
        const row = all[slug];
        if (row && typeof row.qty === "number") {
          return Math.min(12, Math.max(1, row.qty | 0));
        }
        return 1;
      } catch {
        return 1;
      }
    },
    async leafSet(slug, ids, qty) {
      try {
        const all = await readLeafAll();
        all[slug] = {
          extras: Array.isArray(ids) ? ids : [],
          qty: typeof qty === "number" ? Math.min(12, Math.max(1, qty | 0)) : 1,
        };
        await writeLeafAll(all);
      } catch {
        /* private mode / unavailable */
      }
    },
  };
})();

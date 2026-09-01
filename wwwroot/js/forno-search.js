(() => {
  const KEYS = [
    { name: "name", weight: 0.34 },
    { name: "tagline", weight: 0.2 },
    { name: "ingredients", weight: 0.2 },
    { name: "description", weight: 0.14 },
    { name: "tags", weight: 0.12 },
  ];

  const OPTIONS = {
    keys: KEYS,
    threshold: 0.38,
    ignoreLocation: true,
    minMatchCharLength: 2,
    distance: 80,
  };

  let items = [];
  let fuse = null;

  function tagPool(tag) {
    if (!tag) {
      return items;
    }

    const needle = String(tag).toLowerCase();
    return items.filter((row) =>
      String(row.tags || "")
        .toLowerCase()
        .split(/\s+/)
        .includes(needle)
    );
  }

  function buildFuse(pool) {
    return new Fuse(pool, OPTIONS);
  }

  window.FornoSearch = {
    load(json) {
      try {
        items = JSON.parse(json) || [];
      } catch {
        items = [];
      }
      fuse = items.length ? buildFuse(items) : null;
    },

    query(text, tag) {
      const pool = tagPool(tag);
      const q = String(text || "").trim();

      if (!pool.length) {
        return [];
      }

      if (!q) {
        return pool.map((row) => row.slug);
      }

      const engine = tag ? buildFuse(pool) : fuse;
      if (!engine) {
        return [];
      }

      return engine.search(q, { limit: 24 }).map((hit) => hit.item.slug);
    },

    suggest(text, tag, limit = 6) {
      const pool = tagPool(tag);
      const q = String(text || "").trim();

      if (!pool.length || q.length < 2) {
        return [];
      }

      const engine = tag ? buildFuse(pool) : fuse;
      if (!engine) {
        return [];
      }

      return engine.search(q, { limit }).map((hit) => ({
        slug: hit.item.slug,
        name: hit.item.name,
        tagline: hit.item.tagline,
        price: hit.item.price,
        tone: hit.item.tone,
      }));
    },
  };
})();

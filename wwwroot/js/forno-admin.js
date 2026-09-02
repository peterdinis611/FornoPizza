window.FornoAdmin = (() => {
  const KEY = "forno-admin-unlocked";

  return {
    isUnlocked() {
      try {
        return sessionStorage.getItem(KEY) === "1";
      } catch {
        return false;
      }
    },
    unlock() {
      try {
        sessionStorage.setItem(KEY, "1");
      } catch {
        /* private mode */
      }
    },
    lock() {
      try {
        sessionStorage.removeItem(KEY);
      } catch {
        /* private mode */
      }
    },
  };
})();

(() => {
  const reduced = () =>
    window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  const finePointer = () =>
    window.matchMedia("(hover: hover) and (pointer: fine)").matches;

  let homeCleanups = [];
  let booted = false;

  function revealNow() {
    document.documentElement.classList.add("motion-done");
    document.documentElement.classList.remove("has-motion");
  }

  function boot() {
    if (booted) {
      return;
    }
    booted = true;

    setupCursor();
    setupHeatRail();

    if (reduced()) {
      document.documentElement.classList.remove("has-motion");
      return;
    }

    document.documentElement.classList.add("has-motion");
    window.setTimeout(() => {
      if (!document.documentElement.classList.contains("motion-ready")) {
        revealNow();
      }
    }, 2400);

    const fault = document.querySelector(".kiln-fault");
    if (fault) {
      mountFault(fault);
    }
  }

  function setupCursor() {
    if (!finePointer() || reduced()) {
      return;
    }

    const cursor = document.querySelector(".ember-cursor");
    if (!cursor) {
      return;
    }

    const ring = cursor.querySelector(".ember-cursor-ring");
    const core = cursor.querySelector(".ember-cursor-core");
    if (!ring || !core) {
      return;
    }

    const pos = { x: -100, y: -100 };
    const lag = { x: -100, y: -100 };
    let armed = false;

    const place = (el, x, y) => {
      el.style.transform = `translate3d(${x}px, ${y}px, 0)`;
    };

    const move = (event) => {
      pos.x = event.clientX;
      pos.y = event.clientY;
      place(core, pos.x, pos.y);

      if (!armed) {
        armed = true;
        place(ring, pos.x, pos.y);
        lag.x = pos.x;
        lag.y = pos.y;
        document.documentElement.classList.add("has-cursor");
      }
    };

    let frame;
    const tick = () => {
      lag.x += (pos.x - lag.x) * 0.18;
      lag.y += (pos.y - lag.y) * 0.18;
      place(ring, lag.x, lag.y);
      frame = requestAnimationFrame(tick);
    };
    tick();

    const over = (event) => {
      const hot = event.target.closest("a, button, [data-magnetic], [data-tilt]");
      cursor.classList.toggle("is-hot", Boolean(hot));
    };

    document.addEventListener("pointermove", move, { passive: true });
    document.addEventListener("pointerover", over, { passive: true });
    window.addEventListener("pagehide", () => cancelAnimationFrame(frame));
  }

  function setupHeatRail() {
    const bar = document.querySelector(".heat-rail i");
    if (!bar) {
      return;
    }

    const update = () => {
      const max = document.documentElement.scrollHeight - window.innerHeight;
      const p = max > 0 ? Math.min(1, window.scrollY / max) : 0;
      bar.style.transform = `scaleX(${p})`;
    };

    update();
    window.addEventListener("scroll", update, { passive: true });
    window.addEventListener("resize", update);
  }

  function setupMagnetic(scope) {
    if (!finePointer() || !window.anime) {
      return;
    }

    const { animate } = window.anime;
    scope.querySelectorAll("[data-magnetic]").forEach((btn) => {
      const move = (event) => {
        const box = btn.getBoundingClientRect();
        const x = (event.clientX - box.left - box.width / 2) * 0.28;
        const y = (event.clientY - box.top - box.height / 2) * 0.28;
        animate(btn, { x, y, duration: 280, ease: "out(3)" });
      };
      const leave = () => {
        animate(btn, { x: 0, y: 0, duration: 520, ease: "out(4)" });
      };
      btn.addEventListener("pointermove", move);
      btn.addEventListener("pointerleave", leave);
      homeCleanups.push(() => {
        btn.removeEventListener("pointermove", move);
        btn.removeEventListener("pointerleave", leave);
      });
    });
  }

  function tilt(el, strength) {
    if (!finePointer() || !window.anime) {
      return () => {};
    }

    const { animate } = window.anime;
    const move = (event) => {
      const box = el.getBoundingClientRect();
      const px = (event.clientX - box.left) / box.width - 0.5;
      const py = (event.clientY - box.top) / box.height - 0.5;
      animate(el, {
        rotateY: px * strength,
        rotateX: -py * strength,
        duration: 420,
        ease: "out(3)",
      });
    };
    const leave = () => {
      animate(el, { rotateX: 0, rotateY: 0, duration: 700, ease: "out(4)" });
    };

    el.addEventListener("pointermove", move);
    el.addEventListener("pointerleave", leave);
    return () => {
      el.removeEventListener("pointermove", move);
      el.removeEventListener("pointerleave", leave);
    };
  }

  function countStat(el) {
    const to = Number(el.dataset.count || "0");
    const suffix = el.dataset.suffix || "";
    if (!to || !window.anime) {
      return;
    }

    const { animate } = window.anime;
    const state = { v: 0 };
    animate(state, {
      v: to,
      duration: 1400,
      ease: "out(3)",
      modifier: (value) => Math.round(value),
      onUpdate: () => {
        el.textContent = `${Math.round(state.v)}${suffix}`;
      },
    });
  }

  function mount(root) {
    unmount();

    if (!root) {
      revealNow();
      return;
    }

    if (reduced() || !window.anime) {
      revealNow();
      return;
    }

    const { animate, createTimeline, stagger, onScroll } = window.anime;
    document.documentElement.classList.add("has-motion", "motion-ready");

    const chars = root.querySelectorAll(".char");
    const doors = {
      a: root.querySelector(".kiln-door-a"),
      b: root.querySelector(".kiln-door-b"),
    };

    const intro = createTimeline({
      defaults: { ease: "out(3)" },
      onComplete: () => {
        document.documentElement.classList.add("motion-done");
        root.querySelectorAll("[data-count]").forEach(countStat);
      },
    });

    if (chars.length) {
      intro.add(
        chars,
        {
          y: ["110%", "0%"],
          opacity: [0, 1],
          duration: 920,
          ease: "out(4)",
          delay: stagger(58),
        },
        80
      );
    }

    intro.add(
      root.querySelectorAll(".hero-kicker, .lede, .hero-actions, .hearth-stats, .scroll-cue"),
      {
        opacity: [0, 1],
        y: [22, 0],
        duration: 760,
        delay: stagger(90),
      },
      280
    );

    intro.add(
      root.querySelector(".oven-mouth"),
      {
        opacity: [0, 1],
        scale: [0.84, 1],
        duration: 1100,
        ease: "out(4)",
      },
      160
    );

    if (doors.a && doors.b) {
      intro.add(
        doors.a,
        { rotateY: [0, -118], opacity: [1, 0], duration: 880, ease: "inOut(2)" },
        480
      );
      intro.add(
        doors.b,
        { rotateY: [0, 118], opacity: [1, 0], duration: 880, ease: "inOut(2)" },
        480
      );
    }

    intro.add(
      root.querySelector(".oven-seal"),
      { scale: [0.2, 1], rotate: [48, 12], duration: 700, ease: "out(4)" },
      720
    );

    intro.add(
      root.querySelector(".oven-pie"),
      { scale: [0.9, 1], duration: 900, ease: "out(4)" },
      520
    );

    homeCleanups.push(() => intro.revert());
    setupMagnetic(root);

    root.querySelectorAll("[data-scroll]").forEach((el) => {
      const observer = onScroll({
        target: el,
        repeat: false,
        enter: "bottom-=14% top",
        onEnter: () => {
          animate(el, {
            opacity: [0, 1],
            y: [36, 0],
            duration: 860,
            ease: "out(3)",
          });
        },
      });
      homeCleanups.push(() => observer.revert());
    });

    const watermark = root.querySelector(".oven-watermark");
    if (watermark) {
      const observer = onScroll({
        target: root.querySelector(".oven-band") || watermark,
        sync: true,
        onUpdate: (self) => {
          watermark.style.transform = `translate3d(0, ${self.progress * 40 - 8}%, 0)`;
        },
      });
      homeCleanups.push(() => observer.revert());
    }

    root.querySelectorAll("[data-tilt]").forEach((el) => {
      homeCleanups.push(tilt(el, el.classList.contains("oven-mouth") ? 8 : 5));
    });

    const cue = root.querySelector(".scroll-cue b");
    if (cue) {
      const bounce = animate(cue, {
        y: [0, 10],
        duration: 900,
        alternate: true,
        loop: true,
        ease: "inOut(2)",
      });
      homeCleanups.push(() => bounce.revert());
    }
  }

  function mountMenu(root, fresh) {
    unmount();

    if (!root) {
      revealNow();
      return;
    }

    if (reduced() || !window.anime) {
      revealNow();
      return;
    }

    const { animate, createTimeline, stagger } = window.anime;
    document.documentElement.classList.add("has-motion", "motion-ready");

    const chars = root.querySelectorAll(".char");
    const intro = createTimeline({
      defaults: { ease: "out(3)" },
      onComplete: () => {
        document.documentElement.classList.add("motion-done");
        root.querySelectorAll("[data-count]").forEach(countStat);
      },
    });

    if (fresh !== false && chars.length) {
      intro.add(
        chars,
        {
          y: ["110%", "0%"],
          opacity: [0, 1],
          duration: 820,
          ease: "out(4)",
          delay: stagger(62),
        },
        40
      );
    } else {
      chars.forEach((char) => {
        char.style.opacity = "1";
        char.style.transform = "none";
      });
    }

    const copy = root.querySelectorAll(".ledger-copy, .ledger-count, .ledger-tools, .ledger-filter");
    if (copy.length) {
      intro.add(
        copy,
        {
          opacity: [0, 1],
          y: [20, 0],
          duration: 680,
          delay: stagger(70),
        },
        fresh === false ? 0 : 240
      );
    }

    const lead = root.querySelector(".peel-lead");
    if (lead) {
      intro.add(
        lead,
        { opacity: [0, 1], y: [42, 0], duration: 920, ease: "out(4)" },
        fresh === false ? 40 : 300
      );

      const pie = lead.querySelector(".pie-mark");
      if (pie) {
        intro.add(
          pie,
          { rotate: [-26, -11], scale: [0.84, 1], duration: 1100, ease: "out(4)" },
          "<"
        );
      }
    }

    const rows = root.querySelectorAll(".peel-row");
    if (rows.length) {
      intro.add(
        rows,
        {
          opacity: [0, 1],
          x: [-42, 0],
          rotate: [-2.4, 0],
          duration: 740,
          delay: stagger(76),
          ease: "out(3)",
        },
        fresh === false ? 80 : 460
      );
    }

    homeCleanups.push(() => intro.revert());
    setupMagnetic(root);

    root.querySelectorAll("[data-tilt]").forEach((el) => {
      homeCleanups.push(tilt(el, el.classList.contains("peel-lead") ? 7 : 5));
    });
  }

  function mountFault(root) {
    unmount();

    if (!root || reduced() || !window.anime) {
      document.documentElement.classList.add("motion-done");
      return;
    }

    document.documentElement.classList.add("has-motion", "motion-ready", "motion-done");
    setupMagnetic(root);

    root.querySelectorAll("[data-tilt]").forEach((el) => {
      homeCleanups.push(tilt(el, 7));
    });
  }

  function unmount() {
    homeCleanups.splice(0).forEach((fn) => {
      try {
        fn();
      } catch {
        /* ignore */
      }
    });
  }

  window.FornoMotion = { boot, mount, mountMenu, mountFault, unmount };

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", boot);
  } else {
    boot();
  }
})();

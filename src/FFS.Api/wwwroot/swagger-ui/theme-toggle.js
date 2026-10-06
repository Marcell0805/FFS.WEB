(function () {
  var STORAGE_KEY = "ffs-swagger-theme";
  var DARK_CLASS = "swagger-theme-dark";

  function preferredTheme() {
    var saved = localStorage.getItem(STORAGE_KEY);
    if (saved === "dark" || saved === "light") {
      return saved;
    }
    return window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
  }

  function applyTheme(theme) {
    var isDark = theme === "dark";
    document.documentElement.classList.toggle(DARK_CLASS, isDark);
    localStorage.setItem(STORAGE_KEY, theme);
    var button = document.getElementById("ffs-theme-toggle");
    if (button) {
      button.textContent = isDark ? "Light mode" : "Dark mode";
      button.setAttribute("aria-pressed", isDark ? "true" : "false");
      button.title = isDark ? "Switch to light mode" : "Switch to dark mode";
    }
  }

  function toggleTheme() {
    var next = document.documentElement.classList.contains(DARK_CLASS) ? "light" : "dark";
    applyTheme(next);
  }

  function ensureToggle() {
    if (document.getElementById("ffs-theme-toggle")) {
      return;
    }

    var topbar = document.querySelector(".swagger-ui .topbar");
    if (!topbar) {
      return false;
    }

    var button = document.createElement("button");
    button.type = "button";
    button.id = "ffs-theme-toggle";
    button.addEventListener("click", toggleTheme);
    topbar.appendChild(button);
    applyTheme(preferredTheme());
    return true;
  }

  // Apply early to avoid a light flash when dark is preferred.
  applyTheme(preferredTheme());

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", waitForTopbar);
  } else {
    waitForTopbar();
  }

  function waitForTopbar() {
    if (ensureToggle()) {
      return;
    }

    var attempts = 0;
    var timer = setInterval(function () {
      attempts += 1;
      if (ensureToggle() || attempts > 40) {
        clearInterval(timer);
      }
    }, 250);
  }
})();

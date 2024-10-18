document.addEventListener("DOMContentLoaded", function() {
  var themeSwitcher = document.getElementById("theme-switcher");

  // Load theme from local storage
  var savedTheme = localStorage.getItem("theme");
  if (savedTheme) {
      setTheme(savedTheme);
  }

  themeSwitcher.addEventListener("click", function () {
      var themeLink = document.getElementById("theme-link");
      var moonIcon = document.getElementById("moon-icon");

      if (themeLink.getAttribute("href").includes("light-theme")) {
          themeLink.setAttribute("href", "/css/dark-theme.css");
          moonIcon.style.fill = "black";
          localStorage.setItem("theme", "dark"); // Save theme to local storage
      } else {
          themeLink.setAttribute("href", "/css/light-theme.css");
          moonIcon.style.fill = "white";
          localStorage.setItem("theme", "light"); // Save theme to local storage
      }
  });

  function setTheme(theme) {
      var moonIcon = document.getElementById("moon-icon");

      if (theme === "dark") {
          moonIcon.style.fill = "black";
      } else {
          moonIcon.style.fill = "white";
      }
  }
});

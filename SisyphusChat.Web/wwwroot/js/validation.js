document.addEventListener("DOMContentLoaded", function () {
  const passwordInput = document.getElementById("password");
  const emailInput = document.getElementById("Input_Email");

  // Password validation criteria
  const criteria = {
    length: {
      regex: /.{6,}/,
      element: document.getElementById("length-check"),
    },
    uppercase: {
      regex: /[A-Z]/,
      element: document.getElementById("uppercase-check"),
    },
    lowercase: {
      regex: /[a-z]/,
      element: document.getElementById("lowercase-check"),
    },
    number: {
      regex: /[0-9]/,
      element: document.getElementById("number-check"),
    },
    special: {
      regex: /[!@#$%^&*(),.?":{}|<>]/,
      element: document.getElementById("special-check"),
    },
  };

  // Real-time password validation
  passwordInput.addEventListener("input", function () {
    const password = this.value;

    // Check each criterion
    for (const [key, criterion] of Object.entries(criteria)) {
      const isValid = criterion.regex.test(password);
      criterion.element.classList.toggle("valid", isValid);

      // Update icon
      const icon = criterion.element.querySelector("i");
      icon.classList.remove("fa-times", "fa-check");
      icon.classList.add(isValid ? "fa-check" : "fa-times");
    }
  });

  // Email validation to prevent emojis
  emailInput.addEventListener("input", function (e) {
    // Remove emojis using regex
    this.value = this.value.replace(
      /[\u{1F600}-\u{1F64F}\u{1F300}-\u{1F5FF}\u{1F680}-\u{1F6FF}\u{1F1E0}-\u{1F1FF}\u{2600}-\u{26FF}\u{2700}-\u{27BF}]/gu,
      ""
    );
  });
});

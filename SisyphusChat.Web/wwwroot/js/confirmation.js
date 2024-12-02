function showConfirmationModal(title, message, onConfirm) {
  // Remove existing modal if any
  const existingModal = document.getElementById("confirmationModal");
  if (existingModal) {
    existingModal.remove();
  }

  // Create modal HTML
  const modalHTML = `
        <div id="confirmationModal" class="confirmation-modal">
            <div class="confirmation-content">
                <h2>${title}</h2>
                <p>${message}</p>
                <div class="confirmation-actions">
                    <button class="btn-cancel">Cancel</button>
                    <button class="btn-confirm">Confirm</button>
                </div>
            </div>
        </div>
    `;

  // Add modal to body
  document.body.insertAdjacentHTML("beforeend", modalHTML);

  // Get modal elements
  const modal = document.getElementById("confirmationModal");
  const cancelBtn = modal.querySelector(".btn-cancel");
  const confirmBtn = modal.querySelector(".btn-confirm");

  // Show modal with animation
  setTimeout(() => modal.classList.add("show"), 10);

  // Handle button clicks
  cancelBtn.onclick = () => {
    modal.classList.remove("show");
    setTimeout(() => modal.remove(), 300);
  };

  confirmBtn.onclick = () => {
    onConfirm();
    modal.classList.remove("show");
    setTimeout(() => modal.remove(), 300);
  };

  // Close on outside click
  modal.onclick = (e) => {
    if (e.target === modal) {
      modal.classList.remove("show");
      setTimeout(() => modal.remove(), 300);
    }
  };
}

function confirmDeleteUser(userId, chatId) {
  showConfirmationModal(
    "Confirm Deletion",
    "Are you sure you want to delete this user from the chat?",
    () => {
      window.location.href = `/ChatSettings/DeleteUserFromChat?userId=${encodeURIComponent(
        userId
      )}&chatId=${encodeURIComponent(chatId)}`;
    }
  );
}

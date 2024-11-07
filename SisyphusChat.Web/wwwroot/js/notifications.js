// State variables
let showingUnreadOnly = true;
let currentFilter = "all"; // 'all', 'group', 'direct'
let notificationConnection = null;
let currentNotifications = [];

// Initialize when the page loads
document.addEventListener("DOMContentLoaded", () => {
  // Initialize SignalR connection
  notificationConnection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .withAutomaticReconnect()
    .build();

  // Start the connection
  startConnection();

  // Prevent dropdown from closing when clicking inside
  document
    .querySelector(".notification-menu")
    ?.addEventListener("click", function (event) {
      event.stopPropagation();
    });

  // Setup filter button handlers
  document
    .getElementById("showUnreadOnly")
    ?.addEventListener("click", toggleUnreadFilter);
  document
    .getElementById("markAllRead")
    ?.addEventListener("click", markAllNotificationsAsRead);

  updateFilterButtonText();
  setupNotificationFilters();
});

async function startConnection() {
  try {
    await notificationConnection.start();
    console.log("Connected to SignalR hub");
    setupSignalRHandlers();
    updateNotifications(); // Initial fetch
  } catch (err) {
    console.error("Error connecting to hub:", err);
    setTimeout(startConnection, 5000);
  }
}

function setupSignalRHandlers() {
  notificationConnection.on("NotificationsUpdated", () => {
    console.log("Notifications updated");
    updateNotifications();
  });

  notificationConnection.on("ReceiveNotification", (notification) => {
    console.log("Received notification:", notification);
    if (notification) {
      currentNotifications.unshift(notification);
      updateNotificationUI(currentNotifications, true);
      updateNotificationCount();
    }
  });

  notificationConnection.on("ReceiveNotifications", (notifications) => {
    console.log("Received notifications:", notifications);
    if (Array.isArray(notifications)) {
      currentNotifications = notifications;
      updateNotificationUI(notifications);
      updateNotificationCount();
    } else {
      console.error("Received invalid notifications format:", notifications);
      currentNotifications = [];
      updateNotificationUI([]);
    }
  });

  notificationConnection.on("NotificationMarkedAsRead", (notificationId) => {
    const notificationElement = document.querySelector(
      `.notification-box[data-id="${notificationId}"]`
    );
    if (notificationElement) {
      notificationElement.classList.remove("unread");
      notificationElement.classList.add("read");
    }
    updateNotifications(); // Refresh to update counts
  });

  notificationConnection.on("UserBanned", function (isPermanent) {
    if (isPermanent) {
      window.location.href = "/Home/PermanentlyBanned";
    } else {
      window.location.href = "/Home/Banned";
    }
  });
}

function updateNotifications() {
  notificationConnection.invoke("GetNotificationsAsync").catch((err) => {
    console.error("Error fetching notifications:", err);
    updateNotificationUI([]);
  });
}

function updateNotificationUI(notifications, isNewNotification = false) {
  if (!Array.isArray(notifications)) {
    console.error("Invalid notifications format");
    notifications = [];
  }

  const notificationList = document.getElementById("notificationList");

  // Sort notifications
  const sortedNotifications = [...notifications].sort((a, b) => {
    if (a.isRead !== b.isRead) return a.isRead ? 1 : -1;
    return new Date(b.timeCreated) - new Date(a.timeCreated);
  });

  // Filter unread if needed
  let filteredNotifications = showingUnreadOnly
    ? sortedNotifications.filter((n) => !n.isRead)
    : sortedNotifications;

  // Clear existing notifications if not a new notification
  if (!isNewNotification) {
    notificationList.innerHTML = "";
  }

  // Render notifications
  filteredNotifications.forEach((notification) => {
    const box = document.createElement("div");
    box.className = `notification-box ${
      notification.isRead ? "read" : "unread"
    }`;
    box.textContent = notification.message;
    box.dataset.id = notification.id;

    box.addEventListener("click", () => {
      markAsRead(notification.id);
      box.classList.add("read");
    });

    if (isNewNotification) {
      notificationList.insertBefore(box, notificationList.firstChild);
    } else {
      notificationList.appendChild(box);
    }
  });

  updateNotificationCount();
}

function updateNotificationCount() {
  const unreadCount = currentNotifications.filter((n) => !n.isRead).length;
  const badge = document.querySelector(".notification-badge");

  if (badge) {
    if (unreadCount > 0) {
      badge.textContent = unreadCount;
      badge.classList.remove("d-none");
      badge.classList.add("show");
    } else {
      badge.textContent = "";
      badge.classList.add("d-none");
      badge.classList.remove("show");
    }
  }
}

function markAsRead(notificationId) {
  notificationConnection
    .invoke("MarkNotificationAsRead", notificationId)
    .catch((err) => console.error("Error marking notification as read:", err));
}

function markAllNotificationsAsRead() {
  notificationConnection
    .invoke("MarkAllNotificationsAsRead")
    .catch((err) =>
      console.error("Error marking all notifications as read:", err)
    );
}

function toggleUnreadFilter() {
  showingUnreadOnly = !showingUnreadOnly;
  updateFilterButtonText();
  updateNotificationUI(currentNotifications);
}

function updateFilterButtonText() {
  const filterButton = document.getElementById("showUnreadOnly");
  if (filterButton) {
    const buttonText = filterButton.querySelector("span");
    if (buttonText) {
      buttonText.textContent = showingUnreadOnly ? "Show All" : "Show Unread";
    }
  }
}

// Export functions for global use
window.markAsRead = markAsRead;
window.toggleUnreadFilter = toggleUnreadFilter;
window.markAllNotificationsAsRead = markAllNotificationsAsRead;

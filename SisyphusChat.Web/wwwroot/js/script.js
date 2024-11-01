document.addEventListener("DOMContentLoaded", function () {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub") // Adjust the URL as needed
        .build();

    // Start the connection
    connection.start().then(() => {
        console.log('Connected to SignalR');
        fetchAllNotifications(); // Initial fetch of all notifications

        // Listen for notifications from the server
        connection.on('ReceiveNotification', (notification) => {
            renderNotification(notification);
        });

        connection.on('NotificationMarkedAsRead', (notificationId) => {
            const notificationBox = document.querySelector(`.notification-box[data-id="${notificationId}"]`);
            if (notificationBox) {
                notificationBox.classList.add('read'); // Visually mark as read
            }
            updateNotificationCount(); // Update notification count after marking as read
        });

    }).catch(err => console.error(err));

    // Fetch and display all notifications
    function fetchAllNotifications() {
        connection.invoke('GetNotificationsAsync')
            .then(notifications => {
                renderNotifications(notifications);
                updateNotificationCount(); // Update notification count
            })
            .catch(err => console.error(err));
    }

    // Fetch and display unread notifications
    function fetchUnreadNotifications() {
        connection.invoke('GetNotificationsAsync') // Reuse the same method; filter later if needed
            .then(notifications => {
                const unreadNotifications = notifications.filter(notification => !notification.isRead);
                renderNotifications(unreadNotifications);
                updateNotificationCount(); // Update notification count
            })
            .catch(err => console.error(err));
    }

    // Render notifications to the dropdown
    function renderNotifications(notifications) {
        const notificationList = document.getElementById('notificationList');
        notificationList.innerHTML = ''; // Clear existing notifications

        notifications.forEach(notification => {
            const box = document.createElement('div');
            box.className = `notification-box ${notification.isRead ? 'read' : ''}`;
            box.textContent = notification.message;
            box.dataset.id = notification.id; // Store ID for later use

            box.addEventListener('click', () => {
                markAsRead(notification.id);
                box.classList.add('read'); // Visually mark as read
            });

            notificationList.appendChild(box);
        });
    }

    // Render a single notification when received
    function renderNotification(notification) {
        const notificationList = document.getElementById('notificationList');
        const box = document.createElement('div');
        box.className = 'notification-box';
        box.textContent = notification.message;
        box.dataset.id = notification.id; // Store ID for later use

        box.addEventListener('click', () => {
            markAsRead(notification.id);
            box.classList.add('read'); // Visually mark as read
        });

        notificationList.appendChild(box);
    }

    // Mark notification as read
    function markAsRead(notificationId) {
        connection.invoke('MarkNotificationAsRead', notificationId)
            .catch(err => console.error(err));
    }

    // Update notification count in the UI
    function updateNotificationCount() {
        connection.invoke('GetNotificationsAsync')
            .then(notifications => {
                const unreadCount = notifications.filter(notification => !notification.isRead).length;
                document.getElementById('notificationCount').textContent = unreadCount;
            })
            .catch(err => console.error(err));
    }

    // Handle toggle change
    document.getElementById('toggleUnread').addEventListener('change', (event) => {
        if (event.target.checked) {
            fetchUnreadNotifications();
        } else {
            fetchAllNotifications();
        }
    });

    // Toggle notification dropdown
    function toggleNotificationDropdown(event) {
        event.preventDefault(); // Prevent default anchor behavior
        const notificationList = document.getElementById('notificationList');
        notificationList.style.display = notificationList.style.display === 'block' ? 'none' : 'block'; // Toggle visibility
    }
});

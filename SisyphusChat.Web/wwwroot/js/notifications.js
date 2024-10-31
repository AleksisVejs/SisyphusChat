// State variables
let showingUnreadOnly = true;
let currentFilter = 'all'; // 'all', 'group', 'direct'
let notificationConnection = null;

// Initialize notifications
function initializeNotifications(connection) {
    notificationConnection = connection;
    setupNotificationFilters();
    
    connection.on("NotificationsUpdated", () => {
        updateNotifications();
    });
    
    connection.on("ReceiveNotifications", notifications => {
        if (Array.isArray(notifications)) {
            updateNotificationUI(notifications);
        } else {
            console.error("Received invalid notifications format:", notifications);
            updateNotificationUI([]);
        }
    });
    
    updateNotifications();
}

function setupNotificationFilters() {
    const notificationList = document.getElementById("notificationList");
    if (!document.getElementById('notificationFilters')) {
        const filterControls = document.createElement('div');
        filterControls.id = 'notificationFilters';
        filterControls.className = 'notification-filters';
        filterControls.innerHTML = `
            <div class="filter-buttons">
                <button class="filter-btn active" onclick="setTypeFilter('all', event)">All</button>
                <button class="filter-btn" onclick="setTypeFilter('group', event)">Groups</button>
                <button class="filter-btn" onclick="setTypeFilter('direct', event)">Direct</button>
            </div>
        `;
        
        // Prevent dropdown from closing when clicking inside filters
        filterControls.addEventListener('click', (event) => {
            event.stopPropagation();
        });
        
        notificationList.parentElement.insertBefore(filterControls, notificationList);
    }

    // Add showUnreadOnly button functionality
    const showUnreadBtn = document.getElementById('showUnreadOnly');
    if (showUnreadBtn) {
        showUnreadBtn.addEventListener('click', (event) => {
            event.preventDefault();
            event.stopPropagation();
            toggleUnreadFilter();
        });
    }

    // Add markAllRead button functionality
    const markAllReadBtn = document.getElementById('markAllRead');
    if (markAllReadBtn) {
        markAllReadBtn.addEventListener('click', (event) => {
            event.preventDefault();
            event.stopPropagation();
            markAllNotificationsAsRead();
        });
    }
}

function toggleUnreadFilter() {
    showingUnreadOnly = !showingUnreadOnly;
    updateNotifications();
    
    // Update button appearance
    const filterBtn = document.getElementById('showUnreadOnly');
    if (filterBtn) {
        filterBtn.classList.toggle('active', showingUnreadOnly);
    }
}

function markAllNotificationsAsRead() {
    notificationConnection.invoke('MarkAllNotificationsAsRead')
        .then(() => {
            updateNotifications();
        })
        .catch(err => console.error('Error marking all notifications as read:', err));
}

function setTypeFilter(type, event) {
    if (event) {
        event.preventDefault();
        event.stopPropagation();
    }
    
    currentFilter = type;
    
    // Update active state of filter buttons
    document.querySelectorAll('.filter-btn').forEach(btn => {
        btn.classList.toggle('active', 
            btn.innerText.toLowerCase() === type);
    });
    
    updateNotifications();
}

function updateNotificationUI(notifications) {
    if (!Array.isArray(notifications)) {
        console.error("Invalid notifications format");
        notifications = [];
    }
    
    const notificationList = document.getElementById("notificationList");
    const notificationBadge = document.querySelector(".notification-badge");
    
    // Sort notifications
    const sortedNotifications = [...notifications].sort((a, b) => {
        if (a.isRead !== b.isRead) return a.isRead ? 1 : -1;
        return new Date(b.timeCreated) - new Date(a.timeCreated);
    });

    // Filter unread if needed
    let filteredNotifications = showingUnreadOnly 
        ? sortedNotifications.filter(n => !n.isRead)
        : sortedNotifications;
    
    // Group notifications
    const groupedNotifications = groupNotifications(filteredNotifications);
    
    // Apply type filter
    let displayedGroups = groupedNotifications;
    if (currentFilter !== 'all') {
        displayedGroups = displayedGroups.filter(group => group.type === currentFilter);
    }
    
    // Update notification list
    notificationList.innerHTML = "";
    
    // Count unread notifications (using original notifications array)
    const unreadCount = notifications.filter(n => !n.isRead).length;
    
    // Update badge
    if (notificationBadge) {
        if (unreadCount > 0) {
            notificationBadge.textContent = unreadCount;
            notificationBadge.classList.remove("d-none");
            notificationBadge.classList.add("show");
        } else {
            notificationBadge.textContent = "";
            notificationBadge.classList.add("d-none");
            notificationBadge.classList.remove("show");
        }
    }

    // Display filtered notifications
    if (displayedGroups.length > 0) {
        displayedGroups.forEach(group => {
            const notificationGroup = createNotificationGroupElement(group);
            notificationList.appendChild(notificationGroup);
        });
    } else {
        notificationList.innerHTML = '<div class="empty-state">No notifications</div>';
    }
}

function groupNotifications(notifications) {
    const groups = new Map();
    
    notifications.forEach(notification => {
        let groupKey;
        if (notification.message.match(/^\[(.*?)\]/)) {
            // Group chat notifications - group by chat name
            const groupName = notification.message.match(/^\[(.*?)\]/)[1];
            groupKey = `group_${groupName}`;
        } else {
            // Direct messages - group by sender
            groupKey = `user_${notification.senderUsername}`;
        }
        
        if (!groups.has(groupKey)) {
            groups.set(groupKey, {
                key: groupKey,
                type: notification.message.match(/^\[(.*?)\]/) ? 'group' : 'direct',
                name: notification.message.match(/^\[(.*?)\]/) 
                    ? notification.message.match(/^\[(.*?)\]/)[1]
                    : notification.senderUsername,
                notifications: []
            });
        }
        
        groups.get(groupKey).notifications.push(notification);
    });
    
    return Array.from(groups.values());
}

function createNotificationGroupElement(group) {
    const div = document.createElement("div");
    div.className = "notification-group-container";
    
    const hasUnread = group.notifications.some(n => !n.isRead);
    if (hasUnread) {
        div.classList.add("has-unread");
    }
    
    const latestNotification = group.notifications[0];
    const formattedDate = new Date(latestNotification.timeCreated).toLocaleString('en-US', {
        month: 'short',
        day: 'numeric',
        hour: 'numeric',
        minute: '2-digit',
        hour12: true
    });

    div.innerHTML = `
        <div class="notification-group-header">
            <span class="notification-group-name">${group.name}</span>
            <span class="notification-time">${formattedDate}</span>
        </div>
        <div class="notification-group-content">
            ${group.notifications.map(notification => {
                const message = notification.message.replace(/^\[(.*?)\]\s*/, '');
                return `
                    <div class="notification-item ${notification.isRead ? 'read' : 'unread'}" 
                         data-notification-id="${notification.id}"
                         onclick="handleNotificationClick(${notification.type}, '${notification.relatedEntityId}', event)">
                        <div class="notification-content">
                            <p class="notification-message">${message}</p>
                            ${!notification.isRead ? 
                                `<button class="btn btn-sm btn-outline-primary mark-read" 
                                         onclick="markAsRead('${notification.id}', event)">
                                    <i class="fas fa-check"></i>
                                </button>` : ''
                            }
                        </div>
                    </div>
                `;
            }).join('')}
        </div>
    `;
    
    return div;
}

function updateNotifications() {
    notificationConnection.invoke('GetNotificationsAsync')
        .catch(err => {
            console.error("Error fetching notifications:", err);
            updateNotificationUI([]);
        });
}

// Add these functions near the top of the file
function handleNotificationClick(type, relatedEntityId, event) {
    if (event) {
        event.preventDefault();
        event.stopPropagation();
    }

    // Don't handle click if clicking the mark-read button
    if (event.target.classList.contains('mark-read') || 
        event.target.closest('.mark-read')) {
        return;
    }

    // Mark notification as read
    const notificationId = event.currentTarget.dataset.notificationId;
    markAsRead(notificationId);

    // Map numeric type to string
    const notificationTypes = {
        0: 'message',
        1: 'friendrequest',
        2: 'adminmessage'
    };

    const typeString = notificationTypes[type] || 'unknown';

    // Navigate based on notification type
    switch (typeString) {
        case 'message':
            window.location.href = `/Chat/ChatRoom?chatId=${relatedEntityId}`;
            break;
        case 'friendrequest':
            window.location.href = '/Friends';
            break;
        case 'adminmessage':
            // Handle admin messages if needed
            break;
        default:
            console.warn('Unknown notification type:', type);
    }
}

function markAsRead(notificationId, event) {
    if (event) {
        event.preventDefault();
        event.stopPropagation();
    }

    notificationConnection.invoke('MarkNotificationAsRead', notificationId)
        .then(() => {
            // Update UI to show notification as read
            const notificationElement = document.querySelector(
                `.notification-item[data-notification-id="${notificationId}"]`
            );
            if (notificationElement) {
                notificationElement.classList.remove('unread');
                notificationElement.classList.add('read');
            }
            // Refresh notifications to update counts and groups
            updateNotifications();
        })
        .catch(err => console.error('Error marking notification as read:', err));
}
// Export functions that need to be globally accessible
window.setTypeFilter = setTypeFilter;
window.toggleUnreadFilter = toggleUnreadFilter;
window.initializeNotifications = initializeNotifications;
window.handleNotificationClick = handleNotificationClick;
window.markAsRead = markAsRead;


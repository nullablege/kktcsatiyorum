// SignalR Connection and Notification Logic

document.addEventListener('DOMContentLoaded', function () {
    const notificationBadge = document.getElementById("notificationBadge");

    // Only connect if the badge exists (implies user is authenticated)
    if (notificationBadge) {

        // 1. Initial Unread Count Fetch
        fetch('/Member/Notifications/UnreadCount')
            .then(response => response.json())
            .then(data => {
                updateBadge(data.count);
            })
            .catch(error => console.error('Error fetching unread count:', error));

        // 2. SignalR Connection
        // Ensure signalR lib is loaded first (usually via separate script tag)
        if (typeof signalR !== 'undefined') {
            var connection = new signalR.HubConnectionBuilder()
                .withUrl("/hubs/notifications")
                .withAutomaticReconnect()
                .build();

            connection.on("notificationReceived", function (data) {
                console.log("Bildirim yollandı:", data);
                incrementBadge();
            });

            connection.start().catch(function (err) {
                console.error("SignalR Connection Error: ", err.toString());
            });
        }
    }

    function updateBadge(count) {
        if (!notificationBadge) return;

        const countInt = parseInt(count) || 0;
        notificationBadge.innerText = countInt;

        if (countInt > 0) {
            notificationBadge.style.display = "inline-block";
        } else {
            notificationBadge.style.display = "none";
        }
    }

    function incrementBadge() {
        if (!notificationBadge) return;
        let current = parseInt(notificationBadge.innerText) || 0;
        updateBadge(current + 1);
    }
});

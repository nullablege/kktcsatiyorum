document.addEventListener('DOMContentLoaded', function () {
    var deleteConfirmModal = document.getElementById('deleteConfirmModal');
    if (deleteConfirmModal) {
        deleteConfirmModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            var listingId = button.getAttribute('data-listing-id');
            var url = button.getAttribute('data-url');

            var input = deleteConfirmModal.querySelector('#deleteListingId');
            if (input) input.value = listingId;

            var form = deleteConfirmModal.querySelector('form');
            if (form && url) {
                form.action = url;
            }
        });
    }

    var rejectModal = document.getElementById('rejectModal');
    if (rejectModal) {
        rejectModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            var listingId = button.getAttribute('data-id');
            var listingTitle = button.getAttribute('data-title');

            var modalTitle = rejectModal.querySelector('.modal-title');
            var inputId = rejectModal.querySelector('#rejectListingId');
            var reasonInput = rejectModal.querySelector('#RedNedeni');

            if (modalTitle) modalTitle.textContent = 'İlanı Reddet: ' + listingTitle;
            if (inputId) inputId.value = listingId;
            if (reasonInput) reasonInput.value = '';
        });
    }
    var mapEl = document.getElementById('staticMap');
    if (mapEl && window.L) {
        var lat = parseFloat(mapEl.getAttribute('data-lat'));
        var lng = parseFloat(mapEl.getAttribute('data-lng'));
        if (!isNaN(lat) && !isNaN(lng)) {
            var map = L.map('staticMap', {
                zoomControl: false,
                scrollWheelZoom: false,
                dragging: false
            }).setView([lat, lng], 13);
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png').addTo(map);
            L.marker([lat, lng]).addTo(map);
        }
    }
});

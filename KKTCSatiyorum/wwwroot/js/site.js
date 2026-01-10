document.addEventListener('DOMContentLoaded', function () {
    var deleteConfirmModal = document.getElementById('deleteConfirmModal');
    if (deleteConfirmModal) {
        deleteConfirmModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            var listingId = button.getAttribute('data-listing-id') || button.getAttribute('data-id');
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
        var form = rejectModal.querySelector('form');
        var reasonInput = rejectModal.querySelector('#RedNedeni');

        rejectModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;
            var listingId = button.getAttribute('data-id');
            var listingTitle = button.getAttribute('data-title');

            var modalTitle = rejectModal.querySelector('.modal-title');
            var inputId = rejectModal.querySelector('#rejectListingId');

            if (modalTitle) modalTitle.textContent = 'İlanı Reddet: ' + listingTitle;
            if (inputId) inputId.value = listingId;
            if (reasonInput) {
                reasonInput.value = '';
                var err = reasonInput.parentElement.querySelector('.text-danger');
                if (err) err.remove();
            }
        });

        if (form && reasonInput) {
            form.addEventListener('submit', function (e) {
                if (!reasonInput.value.trim()) {
                    e.preventDefault();
                    var existingErr = reasonInput.parentElement.querySelector('.text-danger');
                    if (!existingErr) {
                        var div = document.createElement('div');
                        div.className = 'text-danger small mt-1';
                        div.textContent = 'Lütfen red nedeni giriniz.';
                        reasonInput.parentElement.appendChild(div);
                    }
                }
            });
        }
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

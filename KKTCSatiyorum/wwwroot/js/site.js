document.addEventListener('DOMContentLoaded', function () {
    var deleteConfirmModal = document.getElementById('deleteConfirmModal');
    if (deleteConfirmModal) {
        deleteConfirmModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;

            // Get data attributes
            var url = button.getAttribute('data-url');
            var title = button.getAttribute('data-title');
            var body = button.getAttribute('data-body');
            var btnText = button.getAttribute('data-btn-text');
            var btnClass = button.getAttribute('data-btn-class');

            // Update modal content
            var modalTitle = deleteConfirmModal.querySelector('#deleteModalTitle');
            var modalBody = deleteConfirmModal.querySelector('#deleteModalBody');
            var deleteForm = deleteConfirmModal.querySelector('#deleteForm');
            var confirmBtn = deleteConfirmModal.querySelector('#deleteConfirmBtn');

            if (modalTitle && title) modalTitle.textContent = title;
            if (modalBody && body) modalBody.innerHTML = body; // Allow HTML in body if needed
            if (deleteForm && url) deleteForm.action = url;

            if (confirmBtn) {
                if (btnText) confirmBtn.textContent = btnText;
                if (btnClass) {
                    confirmBtn.className = 'btn ' + btnClass;
                } else {
                    confirmBtn.className = 'btn btn-danger'; // Default
                }
            }
        });
    }
});

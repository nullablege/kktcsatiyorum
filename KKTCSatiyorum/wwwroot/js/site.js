// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function() {
    var genericConfirmModal = document.getElementById('genericConfirmModal');
    if (genericConfirmModal) {
        genericConfirmModal.addEventListener('show.bs.modal', function(event) {
            var button = event.relatedTarget;
            
            // Extract info from data-* attributes
            var title = button.getAttribute('data-confirm-title');
            var body = button.getAttribute('data-confirm-body');
            var actionUrl = button.getAttribute('data-confirm-action');
            var itemId = button.getAttribute('data-confirm-id');
            var btnText = button.getAttribute('data-confirm-btn-text');
            var btnClass = button.getAttribute('data-confirm-btn-class');

            // Update the modal's content.
            var modalTitle = genericConfirmModal.querySelector('.modal-title');
            var modalBody = genericConfirmModal.querySelector('.modal-body');
            var modalForm = genericConfirmModal.querySelector('#genericConfirmModalForm');
            var modalIdInput = genericConfirmModal.querySelector('#genericConfirmModalId');
            var modalSubmitBtn = genericConfirmModal.querySelector('#genericConfirmModalSubmitBtn');

            if (title) modalTitle.textContent = title;
            if (body) modalBody.textContent = body;
            if (actionUrl) modalForm.action = actionUrl;
            if (itemId) modalIdInput.value = itemId;
            if (btnText) modalSubmitBtn.textContent = btnText;
            
            // Reset and set button class
            modalSubmitBtn.className = 'btn';
            if (btnClass) {
                modalSubmitBtn.classList.add(...btnClass.split(' '));
            } else {
                modalSubmitBtn.classList.add('btn-danger'); // default
            }
        });
    }
});

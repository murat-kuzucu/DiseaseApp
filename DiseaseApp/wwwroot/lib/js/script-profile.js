document.addEventListener('DOMContentLoaded', function() {
    var commentLinks = document.querySelectorAll('#comments a');
    commentLinks.forEach(function(link) {
        link.setAttribute('data-bs-toggle', 'tooltip');
        link.setAttribute('title', 'Go to post');
    });

    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function(tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
});

$('').ready(function () {
    const $form = $('#addScheduleForm');
    const action = $form.data('action');

    $form.on('submit', function(e) {
        e.preventDefault();
        
        $.post(action, $form.serialize())
            .done(function(res) {
                if (res.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Success!',
                        text: 'Schedule added successfully!',
                        confirmButtonText: 'OK',
                        timer: 3000,
                        didClose: () => {
                            window.location.href = '/Schedule';
                        }
                    });
                }
            })
            .fail(function(xhr) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Schedule could not be added.',
                    confirmButtonText: 'OK'
                });
            });
    });
});
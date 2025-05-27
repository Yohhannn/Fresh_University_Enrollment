$('').ready(function () {
    const $form = $('#addSessionForm');
    const action = $form.data('action');

    $form.on('submit', function(e) {
        e.preventDefault();
        
        $.post(action, $form.serialize())
            .done(function(res) {
                if (res.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Success!',
                        text: 'Session added successfully!',
                        confirmButtonText: 'OK',
                        timer: 3000
                    }).then(() => {
                        window.location.href = '/Schedule';
                    });


                    const sessionTable = $('#editScheduleModal').find('tbody');
                    const dayMap = {
                        0: "Sunday",
                        1: "Monday",
                        2: "Tuesday",
                        3: "Wednesday",
                        4: "Thursday",
                        5: "Friday",
                        6: "Saturday"
                    };
                    
                    const dayName = dayMap[session.TslDay] ?? "Invalid Day";

                    const row = `
                        <tr>
                            <td>${index + 1}</td>
                            <td>${dayName}</td>
                            <td>${session.TslStartTime}</td>
                            <td>${session.TslEndTime}</td>
                            <td>
                                <button class="btn btn-sm btn-warning edit-session-btn" data-bs-toggle="modal" data-bs-target="#editSessionModal" data-parent="#editScheduleModal">Edit</button>
                                <button class="btn btn-sm btn-danger">Delete</button>
                            </td>
                        </tr>`;
                    sessionTable.append(row);
                }
            })
            .fail(function(xhr) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Schedule could not be added.',
                    confirmButtonText: 'OK',
                    timer: 3000,
                });
            });
    });
});